using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;
using CmlLib.Core.FileExtractors;
using CmlLib.Core.Rules;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;

namespace CmlLib.Core.Installers;

public class TPLGameInstaller : IGameInstaller
{
    private static int? _bestMaxParallelism;
    public static int BestMaxParallelism => _bestMaxParallelism ??= getBestMaxParallelism();
    public static int getBestMaxParallelism()
    {
        // 2 <= p <= 8
        var p = Environment.ProcessorCount;
        p = Math.Max(p, 2);
        p = Math.Min(p, 8);
        return p;
    }

    private readonly int _maxParallelism;

    public TPLGameInstaller(int parallelism) => 
        _maxParallelism = parallelism;

    public async ValueTask Install(
        IEnumerable<IFileExtractor> extractors, 
        MinecraftPath path, 
        IVersion version, 
        RulesEvaluatorContext rulesContext, 
        IProgress<InstallerProgressChangedEventArgs>? fileProgress, 
        IProgress<ByteProgress>? byteProgress, 
        CancellationToken cancellationToken)
    {
        var executor = new TPLGameInstallerExecutor(_maxParallelism, path, version, rulesContext);
        executor.FileProgress += (s, e) => fileProgress?.Report(e);
        executor.ByteProgress += (s, e) => byteProgress?.Report(e);
        await executor.Install(extractors, cancellationToken);
    }
}

class TPLGameInstallerExecutor
{
    private readonly int _maxParallelism;
    private readonly MinecraftPath _path;
    private readonly IVersion _version;
    private readonly RulesEvaluatorContext _rulesContext;

    public TPLGameInstallerExecutor(
        int parallelism,
        MinecraftPath path,
        IVersion version,
        RulesEvaluatorContext rulesContext) =>
        (_maxParallelism, _path, _version, _rulesContext) = 
        (parallelism, path, version, rulesContext);

    public event EventHandler<InstallerProgressChangedEventArgs>? FileProgress;
    public event EventHandler<ByteProgress>? ByteProgress;

    private bool isStarted = false;
    private IDataflowBlock? extractionBlock;
    private ConcurrentDictionary<string, long> sizeStorage = null!; // we don't use null-safety check since the performance is most important part here
    private ThreadLocal<Dictionary<string, ByteProgress>> progressStorage = null!;
    private CancellationToken CancellationToken;
    private int totalTasks = 0;
    private int proceed = 0;

    public async ValueTask Install(
        IEnumerable<IFileExtractor> extractors,
        CancellationToken cancellationToken)
    {
        if (isStarted)
            throw new InvalidOperationException("Already started");

        isStarted = true;
        initializeResources();
        CancellationToken = cancellationToken;

        var extractBlock = createExtractBlock(cancellationToken);
        var executeBlock = createExecuteBlock(extractBlock.Completion);
        extractionBlock = extractBlock;
        extractBlock.LinkTo(executeBlock);

        await Task.WhenAll(extractors.Select(extractor => extractBlock.SendAsync(extractor)));
        extractBlock.Complete();
        await extractBlock.Completion;

        if (proceed == totalTasks)
            return;

        var executeTask = executeBlock.Completion;
        while (!executeTask.IsCompleted)
        {
            reportByteProgress();
            await Task.Delay(200);
        }
        reportByteProgress();
        disposeResources();
    }

    private void initializeResources()
    {
        sizeStorage = new ConcurrentDictionary<string, long>();
        progressStorage = new ThreadLocal<Dictionary<string, ByteProgress>>(
            () => new Dictionary<string, ByteProgress>(),
            true);
        totalTasks = 0;
        proceed = 0;
    }

    private void disposeResources()
    {
        sizeStorage.Clear();
        progressStorage.Dispose();
    }

    private void reportByteProgress()
    {
        long totalBytes = 0;
        long progressedBytes = 0;

        foreach (var dict in progressStorage.Values)
        {
            lock (dict)
            {
                foreach (var kv in dict)
                {
                    totalBytes += kv.Value.TotalBytes;
                    progressedBytes += kv.Value.ProgressedBytes;
                    sizeStorage.TryRemove(kv.Key, out _);
                }
            }
        }

        foreach (var kv in sizeStorage)
        {
            totalBytes += kv.Value;
        }

        fireByteProgress(totalBytes, progressedBytes);
    }

    private BufferBlock<LinkedTask> createExecuteBlock(Task extractTask)
    {
        var buffer = new BufferBlock<LinkedTask>();
        var executor = new TransformBlock<LinkedTask, LinkedTask?>(async task =>
        {
            var progress = new SyncProgress<ByteProgress>(e =>
            {
                lock (progressStorage.Value!)
                {
                    progressStorage.Value![task.Name] = e;
                }
            });

            var nextTask = await task.Execute(progress, CancellationToken);
            if (nextTask == null)
            {
                fireFileProgress(task.Name, TaskStatus.Done);
                var totalSize = task.LinkedSize + task.Size;
                fireByteProgress(totalSize, totalSize);
                Interlocked.Increment(ref proceed);
                if (totalTasks == proceed && (extractionBlock?.Completion.IsCompleted ?? false))
                    buffer.Complete();
            }

            return nextTask;
        }, new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = _maxParallelism
        });

        buffer.LinkTo(executor);
        executor.LinkTo(buffer!, t => t != null);
        executor.LinkTo(DataflowBlock.NullTarget<LinkedTask?>());

        return buffer;
    }

    private IPropagatorBlock<IFileExtractor, LinkedTask> createExtractBlock(CancellationToken cancellationToken)
    {
        var block = new TransformManyBlock<IFileExtractor, LinkedTask>(async extractor =>
        {
            var tasks = await extractor.Extract(_path, _version, _rulesContext, cancellationToken);
            return tasks
                .Where(task => task.First != null)
                .Select(task =>
                {
                    Interlocked.Increment(ref totalTasks);
                    fireFileProgress(task.Name, TaskStatus.Queued);
                    sizeStorage[task.Name] = task.File.Size;

                    return task.First;
                })!;
        }, new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = _maxParallelism
        });

        return block;
    }

    private void fireFileProgress(string name, TaskStatus status)
    {
        FileProgress?.Invoke(this, new InstallerProgressChangedEventArgs(name, status)
        {
            TotalTasks = totalTasks,
            ProceedTasks = proceed
        });
    }

    private void fireByteProgress(long totalBytes, long progressedBytes)
    {
        ByteProgress?.Invoke(this, new ByteProgress
        {
            TotalBytes = totalBytes,
            ProgressedBytes = progressedBytes
        });
    }
}