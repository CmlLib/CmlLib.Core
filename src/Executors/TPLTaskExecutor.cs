using System.Threading.Tasks.Dataflow;
using CmlLib.Core.FileExtractors;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;

namespace CmlLib.Core.Executors;

public class TPLTaskExecutor
{
    private readonly Dictionary<string, long> _sizeStorage;
    private readonly ThreadLocal<Dictionary<string, ByteProgress>> _progressStorage;
    private readonly SemaphoreSlim _semaphore;
    private readonly int _maxParallelism;

    public TPLTaskExecutor(int parallelism)
    {
        _sizeStorage = new Dictionary<string, long>();
        _progressStorage = new ThreadLocal<Dictionary<string, ByteProgress>>(
            () => new Dictionary<string, ByteProgress>(),
            true);

        _semaphore = new SemaphoreSlim(1);
        _maxParallelism = parallelism;
    }

    public event EventHandler<TaskExecutorProgressChangedEventArgs>? FileProgress;
    public event EventHandler<ByteProgress>? ByteProgress;

    private bool isStarted = false;
    private CancellationToken CancellationToken;
    private int totalTasks = 0;
    private int proceed = 0;

    public async ValueTask Install(
        IEnumerable<IFileExtractor> extractors,
        MinecraftPath path,
        IVersion version,
        CancellationToken cancellationToken)
    {
        if (isStarted)
            throw new InvalidOperationException("Already started");
        isStarted = true;

        CancellationToken = cancellationToken;

        var extractBlock = createExtractBlock(version, path);
        var executeBlock = createExecuteBlock(extractBlock.Completion);
        extractBlock.LinkTo(executeBlock);

        await Task.WhenAll(extractors.Select(extractor => extractBlock.SendAsync(extractor)));
        extractBlock.Complete();
        await extractBlock.Completion;

        if (proceed == totalTasks)
            return;

        var executeTask = executeBlock.Completion;
        while (!executeTask.IsCompleted)
        {
            await reportByteProgress();
            await Task.Delay(100);
        }
        await reportByteProgress();
    }

    private async Task reportByteProgress()
    {
        try
        {
            await _semaphore.WaitAsync();

            long totalBytes = 0;
            long progressedBytes = 0;

            foreach (var dict in _progressStorage.Values)
            {
                foreach (var kv in dict)
                {
                    totalBytes += kv.Value.TotalBytes;
                    progressedBytes += kv.Value.ProgressedBytes;
                    _sizeStorage.Remove(kv.Key);
                }
            }

            foreach (var kv in _sizeStorage)
                totalBytes += kv.Value;

            fireByteProgress(totalBytes, progressedBytes);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private BufferBlock<LinkedTask> createExecuteBlock(Task extractTask)
    {
        var buffer = new BufferBlock<LinkedTask>();
        var executor = new TransformBlock<LinkedTask, LinkedTask?>(async task =>
        {
            var progress = new SyncProgress<ByteProgress>(e =>
            {
                bool isLocked = false;
                try
                {
                    if (_semaphore.Wait(0))
                    {
                        isLocked = true;
                        _progressStorage.Value![task.Name] = e;
                    }
                }
                finally
                {
                    if (isLocked)
                        _semaphore.Release();
                }
            });

            var nextTask = await task.Execute(progress, CancellationToken);
            if (nextTask == null)
            {
                try
                {
                    _semaphore.Wait();
                    var previousProgress = _progressStorage.Value![task.Name];
                    _progressStorage.Value![task.Name] = new ByteProgress
                    {
                        TotalBytes = previousProgress.TotalBytes,
                        ProgressedBytes = previousProgress.TotalBytes
                    };
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            return nextTask;
        }, new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = _maxParallelism
        });

        return buffer;
    }

    private IPropagatorBlock<IFileExtractor, LinkedTask> createExtractBlock(
        IVersion version,
        MinecraftPath path)
    {
        var block = new TransformManyBlock<IFileExtractor, LinkedTask>(async extractor =>
        {
            var tasks = await extractor.Extract(path, version);
            return tasks
                .Where(task => task.First != null)
                .Select(task =>
                {
                    Interlocked.Increment(ref totalTasks);
                    fireFileProgress(task.Name, TaskStatus.Queued);
                    _sizeStorage[task.Name] = task.File.Size;

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
        FileProgress?.Invoke(this, new TaskExecutorProgressChangedEventArgs(name, status)
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