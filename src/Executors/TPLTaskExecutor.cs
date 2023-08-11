using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;
using CmlLib.Core.FileExtractors;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;

namespace CmlLib.Core.Executors;

public enum TaskStatus
{
    Queued,
    Processing,
    Done
}

public class TaskExecutorEventArgs
{
    public TaskExecutorEventArgs(string name) =>
        Name = name;

    public int TotalTasks { get; set; }
    public int ProceedTasks { get; set; }
    public TaskStatus EventType { get; set; }
    public string Name { get; set; }

    public void Print()
    {
        //if (status != TaskStatus.Done) return;
        //if (proceed % 100 != 0) return;
        var now = DateTime.Now.ToString("hh:mm:ss.fff");
        Console.WriteLine($"[{now}][{ProceedTasks}/{TotalTasks}][{EventType}] {Name}");
    }
}

public class TPLTaskExecutor
{
    private struct TaskState
    {
        public TaskStatus Status { get; set; }
        public long TotalBytes { get; set; }
        public long ProgressedBytes { get; set; }
    }

    private readonly int _maxParallelism;
    private readonly ConcurrentDictionary<string, TaskState> _runningTasks;

    public TPLTaskExecutor(int parallelism)
    {
        _maxParallelism = parallelism;
        _runningTasks = new ConcurrentDictionary<string, TaskState>(_maxParallelism, 2047);
    }

    public event EventHandler<TaskExecutorEventArgs>? FileProgress;
    public event EventHandler<int>? BytesProgress;

    private long totalBytes = 0;
    private int totalTasks = 0;
    private int proceed = 0;

    public void PrintStatus()
    {
        var runningTasks = _runningTasks
            .Where(kv => kv.Value.Status != TaskStatus.Done)
            .Select(kv => kv.Key);

        foreach (var task in runningTasks)
        {
            Console.WriteLine(task);
        }
    }

    public async ValueTask Install(
        IEnumerable<IFileExtractor> extractors,
        MinecraftPath path,
        IVersion version)
    {
        var extractBlock = createExtractBlock(version, path);
        var executeBlock = createExecuteBlock(extractBlock.Completion);
        extractBlock.LinkTo(executeBlock);

        await Task.WhenAll(extractors.Select(extractor => extractBlock.SendAsync(extractor)));
        extractBlock.Complete();
        await extractBlock.Completion;

        if (proceed == _runningTasks.Count)
            return;
        
        var executeTask = executeBlock.Completion;
        while (!executeTask.IsCompleted)
        {
            long progressedBytes = _runningTasks
                .Select(kv => kv.Value.ProgressedBytes)
                .Sum();
            
            var percent = (int)(progressedBytes / (double)totalBytes * 100);
            BytesProgress?.Invoke(this, percent);
            await Task.Delay(100);
        }
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
                fireEvent(task.Name, TaskStatus.Processing);
                nextTask = await task.Execute();
            }
            catch (Exception ex)
            {
                exception = ex;
                Debug.WriteLine(ex.ToString());
            }

            if (nextTask == null)
            {
                Interlocked.Increment(ref proceed);
                _runningTasks.AddOrUpdate(
                    task.Name, 
                    new TaskState { Status = TaskStatus.Done }, 
                    (key, oldValue) => oldValue with { Status = TaskStatus.Done });

                fireEvent(task.Name, TaskStatus.Done);

                if (proceed == _runningTasks.Count && extractTask.IsCompleted)
                {
                    buffer.Complete();
                }
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

                var state = new TaskState 
                { 
                    Status = TaskStatus.Queued, 
                    TotalBytes = task.File.Size
                };

                if (_runningTasks.TryAdd(task.Name, state))
                {
                    yield return task.First;
                    Interlocked.Increment(ref totalTasks);
                    Interlocked.Add(ref totalBytes, task.File.Size);
                    fireEvent(task.Name, TaskStatus.Queued);
                }
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
        FileProgress?.Invoke(this, new TaskExecutorEventArgs(name)
        {
            EventType = status,
            TotalTasks = totalTasks,
            ProceedTasks = proceed
        });
    }
}