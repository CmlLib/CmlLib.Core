using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;
using CmlLib.Core.FileExtractors;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;

namespace CmlLib.Core.Executors;

public class TPLTaskExecutor
{
    private struct TaskProgress
    {
        public long TotalBytes;
        public long ProgressedBytes;
    }

    //private readonly ConcurrentDictionary<string, TaskProgress> _tasks;
    private readonly ConcurrentDictionary<string, TaskProgress> _totalProgressStorage;
    private readonly ThreadLocal<Dictionary<string, TaskProgress>> _progressPerThread;
    private readonly int _maxParallelism;

    public TPLTaskExecutor(int parallelism)
    {
        _maxParallelism = parallelism;
        _totalProgressStorage = new ConcurrentDictionary<string, TaskProgress>();
        _progressPerThread = new ThreadLocal<Dictionary<string, TaskProgress>>(
            () => new Dictionary<string, TaskProgress>(), true);
    }

    public event EventHandler<TaskExecutorProgressChangedEventArgs>? FileProgress;
    public event EventHandler<ByteProgressEventArgs>? ByteProgress;

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

        foreach (var dict in _progressPerThread.Values)
        {
            lock (dict)
            {
                foreach (var kv in dict)
                {
                    _totalProgressStorage.AddOrUpdate(
                        kv.Key,
                        new TaskProgress
                        {
                            TotalBytes = kv.Value.TotalBytes,
                            ProgressedBytes = kv.Value.ProgressedBytes
                        },
                        (_, old) => new TaskProgress
                        {
                            TotalBytes = kv.Value.TotalBytes,
                            ProgressedBytes = kv.Value.ProgressedBytes
                        });
                }
            }
        }

        foreach (var kv in _totalProgressStorage)
        {
            totalBytes += kv.Value.TotalBytes;
            progressedBytes += kv.Value.ProgressedBytes;
        }

        ByteProgress?.Invoke(this, new ByteProgressEventArgs
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

            var progress = new SyncProgress<ByteProgressEventArgs>(e =>
            {
                var dict = _progressPerThread.Value!;
                lock (dict)
                {
                    dict[task.Name] = new TaskProgress
                    {
                        TotalBytes = e.TotalBytes,
                        ProgressedBytes = e.ProgressedBytes
                    };
                }
                //_progressPerThread.Value!.AddOrUpdate(
                //    task.Name,
                //    new TaskProgress
                //    {
                //        TotalBytes = e.TotalBytes,
                //        ProgressedBytes = e.ProgressedBytes
                //    },
                //    (_, old) => new TaskProgress
                //    {
                //        TotalBytes = e.TotalBytes,
                //        ProgressedBytes = e.ProgressedBytes
                //    });
            });

            var nextTask = await downloadTask.Execute(progress, CancellationToken);
            return finalizeTask(task, nextTask);
        }, new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = _maxParallelism
        });


        LinkedTask? finalizeTask(LinkedTask task, LinkedTask? nextTask)
        {
            if (nextTask == null)
            {
                Interlocked.Increment(ref proceed);
                TaskProgress progress;
                var dict = _progressPerThread.Value!;
                lock (dict)
                {
                    if (!dict.TryGetValue(task.Name, out progress) &&
                        !_totalProgressStorage.TryGetValue(task.Name, out progress))
                        progress = new TaskProgress();
                    dict[task.Name] = progress;
                }
                //_progressPerThread.Value!.AddOrUpdate(
                //    task.Name,
                //    new TaskProgress
                //    {
                //        TotalBytes = 0,
                //        ProgressedBytes = 0
                //    },
                //    (_, old) => old with
                //    {
                //        TotalBytes = old.TotalBytes,
                //        ProgressedBytes = old.TotalBytes
                //    });
                fireEvent(task.Name, TaskStatus.Done);

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
                var state = new TaskProgress
                {
                    TotalBytes = task.File.Size,
                    ProgressedBytes = 0
                };
                _totalProgressStorage.TryAdd(task.Name, state);

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