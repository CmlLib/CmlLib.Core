using CmlLib.Core.FileExtractors;
using CmlLib.Core.Rules;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;
using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;

namespace CmlLib.Core.Installers;

public class TPLGameInstaller : IGameInstaller
{
    private static int? _bestMaxParallelism;
    public static int BestMaxParallelism => _bestMaxParallelism ??= getBestMaxParallelism();
    public static int getBestMaxParallelism()
    {
        // 2 <= p <= 6
        var p = Environment.ProcessorCount;
        p = Math.Max(p, 2);
        p = Math.Min(p, 6);
        return p;
    }

    private readonly int _maxParallelism;

    public TPLGameInstaller(int parallelism) =>
        _maxParallelism = parallelism;

    public async ValueTask Install(
        ITaskFactory taskFactory,
        IEnumerable<IFileExtractor> extractors,
        MinecraftPath path,
        IVersion version,
        RulesEvaluatorContext rulesContext,
        IProgress<InstallerProgressChangedEventArgs>? fileProgress,
        IProgress<ByteProgress>? byteProgress,
        CancellationToken cancellationToken)
    {
        var executor = new TPLGameInstallerExecutor(_maxParallelism, taskFactory, path, version, rulesContext);
        executor.FileProgress += (s, e) => fileProgress?.Report(e);
        executor.ByteProgress += (s, e) => byteProgress?.Report(e);
        await executor.Install(extractors, cancellationToken);
    }
}

public enum GameInstallerExceptionMode
{
    Ignore,
    ThrowException,
    ThrowAggreateException
}

class TPLGameInstallerExecutor
{
    private readonly int _maxParallelism;
    private readonly ITaskFactory _taskFactory;
    private readonly MinecraftPath _path;
    private readonly IVersion _version;
    private readonly RulesEvaluatorContext _rulesContext;

    public TPLGameInstallerExecutor(
        int parallelism,
        ITaskFactory taskFactory,
        MinecraftPath path,
        IVersion version,
        RulesEvaluatorContext rulesContext) =>
        (_maxParallelism, _taskFactory, _path, _version, _rulesContext) =
        (parallelism, taskFactory, path, version, rulesContext);

    public event EventHandler<InstallerProgressChangedEventArgs>? FileProgress;
    public event EventHandler<ByteProgress>? ByteProgress;

    public GameInstallerExceptionMode ExceptionMode { get; set; } = GameInstallerExceptionMode.ThrowAggreateException;

    private ThreadLocal<ByteProgress> progressStorage = null!;
    private bool isStarted = false;
    private CancellationToken CancellationToken;
    private int totalTasks = 0;
    private int proceed = 0;
    private ConcurrentBag<Exception> exceptions = null!;

    public async ValueTask Install(
        IEnumerable<IFileExtractor> extractors,
        CancellationToken cancellationToken)
    {
        if (isStarted)
            throw new InvalidOperationException("Already started");

        isStarted = true;
        initializeResources();
        CancellationToken = cancellationToken;

        var distinctStorage = new HashSet<string>();
        var extractorBlock = createExtractBlock(distinctStorage, cancellationToken);
        var executeBlock = createExecuteBlock(extractorBlock.Completion);
        extractorBlock.LinkTo(executeBlock!, t => t != null);
        extractorBlock.LinkTo(DataflowBlock.NullTarget<LinkedTask?>());

        await Task.WhenAll(extractors.Select(extractor => extractorBlock.SendAsync(extractor)));
        extractorBlock.Complete();
        await extractorBlock.Completion;

        distinctStorage.Clear();

        if (proceed != totalTasks)
        {
            var executeTask = executeBlock.Completion;
            while (!executeTask.IsCompleted)
            {
                reportByteProgress();
                await Task.WhenAny(Task.Delay(200), executeTask);
            }
        }

        reportByteProgress();
        disposeResources();

        if (!exceptions.IsEmpty &&
            ExceptionMode == GameInstallerExceptionMode.ThrowAggreateException)
        {
            throw new AggregateException(exceptions);
        }
    }

    private void initializeResources()
    {
        progressStorage = new ThreadLocal<ByteProgress>(() => new ByteProgress(), true);
        exceptions = new ConcurrentBag<Exception>();
        totalTasks = 0;
        proceed = 0;
    }

    private void disposeResources()
    {
        progressStorage.Dispose();
        progressStorage = null!;
    }

    private void reportByteProgress()
    {
        long totalBytes = 0;
        long progressedBytes = 0;

        foreach (var v in progressStorage.Values)
        {
            totalBytes += v.TotalBytes;
            progressedBytes += v.ProgressedBytes;
        }
        fireByteProgress(totalBytes, progressedBytes);
    }

    private BufferBlock<LinkedTask> createExecuteBlock(Task extractTask)
    {
        var buffer = new BufferBlock<LinkedTask>();
        var executor = createExecuteTransformBlock(extractTask, buffer, _maxParallelism);
        buffer.LinkTo(executor);
        executor.LinkTo(buffer!, t => t != null);
        executor.LinkTo(DataflowBlock.NullTarget<LinkedTask?>());

        return buffer;
    }

    private TransformBlock<LinkedTask, LinkedTask?> createExecuteTransformBlock(Task extractTask, IDataflowBlock buffer, int parallelism)
    {
        return new TransformBlock<LinkedTask, LinkedTask?>(
            t => execute(t, extractTask, buffer),
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = parallelism
            });
    }

    private async Task<LinkedTask?> execute(LinkedTask task, Task extractTask, IDataflowBlock buffer)
    {
        ByteProgress lastProgress = new ByteProgress
        {
            TotalBytes = task.Size,
            ProgressedBytes = 0
        };

        var progress = new SyncProgress<ByteProgress>(e =>
        {
            progressStorage.Value = new ByteProgress
            {
                TotalBytes = progressStorage.Value.TotalBytes + (e.TotalBytes - lastProgress.TotalBytes),
                ProgressedBytes = progressStorage.Value.ProgressedBytes + (e.ProgressedBytes - lastProgress.ProgressedBytes)
            };
            lastProgress = e;
        });

        LinkedTask? nextTask = null;
        try
        {
            await task.Execute(progress, CancellationToken);
        }
        catch (Exception ex)
        {
            exceptions.Add(ex);
            if (ExceptionMode == GameInstallerExceptionMode.ThrowException)
            {
                buffer.Complete();
                throw;
            }
        }

        progress.Report(new ByteProgress
        {
            TotalBytes = lastProgress.TotalBytes,
            ProgressedBytes = lastProgress.TotalBytes
        });

        if (nextTask == null)
        {
            Interlocked.Increment(ref proceed);
            fireFileProgress(task.Name, TaskStatus.Done);
            if (totalTasks == proceed && extractTask.IsCompleted)
                buffer.Complete();
        }

        return nextTask;
    }

    private IPropagatorBlock<IFileExtractor, LinkedTask> createExtractBlock(
        HashSet<string> distinctStorage,
        CancellationToken cancellationToken)
    {
        var extractorBlock = new TransformManyBlock<IFileExtractor, LinkedTask>(async extractor =>
        {
            var tasks = await extractor.Extract(_taskFactory, _path, _version, _rulesContext, cancellationToken);
            return tasks
                .Where(task => task.First != null)
                .Where(task => string.IsNullOrEmpty(task.File.Path) || distinctStorage.Add(task.File.Path))
                .Select(task =>
                {
                    totalTasks++;
                    fireFileProgress(task.Name, TaskStatus.Queued);
                    progressStorage.Value = new ByteProgress
                    {
                        TotalBytes = progressStorage.Value.TotalBytes + task.File.Size,
                        ProgressedBytes = progressStorage.Value.ProgressedBytes
                    };

                    return task.First!;
                });
        }, new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = 1
        });

        return extractorBlock;
    }

    private void fireFileProgress(string name, TaskStatus status)
    {
        FileProgress?.Invoke(this, new InstallerProgressChangedEventArgs(name, status)
        {
            TotalTasks = totalTasks,
            ProgressedTasks = proceed
        });
    }

    private void fireByteProgress(long totalBytes, long progressedBytes)
    {
        var progress = new ByteProgress
        {
            TotalBytes = totalBytes,
            ProgressedBytes = progressedBytes
        };
        ByteProgress?.Invoke(this, progress);
    }
}