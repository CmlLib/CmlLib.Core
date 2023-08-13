using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;
using CmlLib.Core.FileExtractors;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;

namespace CmlLib.Core.Executors;

public class TPLTaskExecutor
{
    private struct WorkerState
    {
        public DateTime LastUpdate;
        public Dictionary<string, ByteProgress> ProgressStorage;
    }

    private readonly ConcurrentDictionary<string, ByteProgress> _progressStorage;
    private readonly ThreadLocal<WorkerState> _progressPerThread;
    private readonly int _maxParallelism;

    public TPLTaskExecutor(int parallelism)
    {
        _maxParallelism = parallelism;
        _progressStorage = new ConcurrentDictionary<string, ByteProgress>();
        _progressPerThread = new ThreadLocal<WorkerState>(
            () =>
            new WorkerState
            {
                LastUpdate = DateTime.MinValue,
                ProgressStorage = new Dictionary<string, ByteProgress>()
            },
            true);
    }

    public event EventHandler<TaskExecutorProgressChangedEventArgs>? FileProgress;
    public event EventHandler<ByteProgress>? ByteProgress;

    private CancellationToken CancellationToken;
    private int totalTasks = 0;
    private int proceed = 0;

    public async ValueTask Install(
        IEnumerable<IFileExtractor> extractors,
        MinecraftPath path,
        IVersion version,
        CancellationToken cancellationToken)
    {
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
            reportByteProgress();
            await Task.Delay(100);
        }
        reportByteProgress();
    }

    private void reportByteProgress()
    {
        long totalBytes = 0;
        long progressedBytes = 0;

        foreach (var kv in _progressStorage)
        {
            totalBytes += kv.Value.TotalBytes;
            progressedBytes += kv.Value.ProgressedBytes;
        }

        ByteProgress?.Invoke(this, new ByteProgress
        {
            TotalBytes = totalBytes,
            ProgressedBytes = progressedBytes
        });
    }

    private BufferBlock<LinkedTask> createExecuteBlock(Task extractTask)
    {
        var buffer = new BufferBlock<LinkedTask>();
        var executor = new TransformBlock<LinkedTask, LinkedTask?>(async task =>
        {
            LinkedTask? nextTask = null;
            Exception? exception = null;
            try
            {
                nextTask = await task.Execute(null, CancellationToken);
            }
            catch (Exception ex)
            {
                exception = ex;
                Debug.WriteLine(ex.ToString());
            }

            return finalizeTask(task, nextTask);

        }, new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = _maxParallelism
        });

        var downloader = new TransformBlock<LinkedTask, LinkedTask?>(async task =>
        {
            var downloadTask = task as DownloadTask;
            if (downloadTask == null)
                return task.NextTask;

            var progress = new SyncProgress<ByteProgress>(e =>
            {
                _progressPerThread.Value.ProgressStorage[task.Name] = e;
                updateLocalProgress();
            });

            var nextTask = await downloadTask.Execute(progress, CancellationToken);
            return finalizeTask(task, nextTask);
        }, new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = _maxParallelism
        });

        void updateLocalProgress()
        {
            if (DateTime.Now > _progressPerThread.Value.LastUpdate.AddSeconds(1))
            {
                var storage = _progressPerThread.Value!.ProgressStorage;
                foreach (var kv in storage)
                {
                    _progressStorage.AddOrUpdate(
                        kv.Key,
                        kv.Value,
                        (_, _) => kv.Value);
                }
                storage.Clear();
                _progressPerThread.Value = new WorkerState
                {
                    LastUpdate = DateTime.Now,
                    ProgressStorage = storage
                };
            }
        }

        LinkedTask? finalizeTask(LinkedTask task, LinkedTask? nextTask)
        {
            if (nextTask == null)
            {
                Interlocked.Increment(ref proceed);
                fireEvent(task.Name, TaskStatus.Done);
                updateLocalProgress();

                if (proceed == totalTasks && extractTask.IsCompleted)
                {
                    buffer.Complete();
                }
            }

            return nextTask;
        }

        buffer.LinkTo(downloader!, t => t is DownloadTask);
        buffer.LinkTo(executor);

        void linkToBuffer(IPropagatorBlock<LinkedTask, LinkedTask?> block, BufferBlock<LinkedTask> buffer)
        {
            block.LinkTo(buffer!, t => t != null);
            block.LinkTo(DataflowBlock.NullTarget<LinkedTask?>());
        }

        linkToBuffer(executor, buffer);
        linkToBuffer(downloader, buffer);

        return buffer;
    }

    private IPropagatorBlock<IFileExtractor, LinkedTask> createExtractBlock(
        IVersion version,
        MinecraftPath path)
    {
        IEnumerable<LinkedTask> fireTasksEvent(IEnumerable<LinkedTaskHead> tasks)
        {
            foreach (var task in tasks)
            {
                if (task.First == null)
                    continue;

                Interlocked.Increment(ref totalTasks);
                var progress = new ByteProgress
                {
                    TotalBytes = task.File.Size,
                    ProgressedBytes = 0
                };
                _progressStorage.TryAdd(task.Name, progress);

                yield return task.First;
                fireEvent(task.Name, TaskStatus.Queued);
            }
        }

        var block = new TransformManyBlock<IFileExtractor, LinkedTask>(async extractor =>
        {
            var tasks = await extractor.Extract(path, version);
            var iterated = fireTasksEvent(tasks);
            return iterated;
        }, new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = _maxParallelism
        });

        return block;
    }

    private void fireEvent(string name, TaskStatus status)
    {
        FileProgress?.Invoke(this, new TaskExecutorProgressChangedEventArgs(name, status)
        {
            TotalTasks = totalTasks,
            ProceedTasks = proceed
        });
    }
}