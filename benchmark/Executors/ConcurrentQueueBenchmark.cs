using System.Collections.Concurrent;
using CmlLib.Core.Tasks;
using CmlLib.Core.Executors;

namespace CmlLib.Core.Benchmarks;

public class ConcurrentQueueBenchmark : ExecutorBenchmarkBase
{
    private struct TaskSizeChangedEventArgs
    {
        public string Name;
        public long TaskSize;
    }

    private Dictionary<string, long> taskSizeStorage = null!;
    private ConcurrentQueue<TaskSizeChangedEventArgs> messageQueue = null!;
    private long progressedBytes;

    protected override void Setup()
    {
        taskSizeStorage = new Dictionary<string, long>();
        messageQueue = new ConcurrentQueue<TaskSizeChangedEventArgs>();
    }

    protected override void OnReportEvent()
    {
        while (messageQueue.TryDequeue(out var message))
        {
            taskSizeStorage[message.Name] = message.TaskSize;
        }

        long totalSize = 0;
        foreach (var kv in taskSizeStorage)
        {
            totalSize += kv.Value;
        }
        
        FireProgress(new ByteProgress
        {
            TotalBytes = totalSize,
            ProgressedBytes = Interlocked.Read(ref progressedBytes)
        });
    }

    protected override void OnTaskAdded(LinkedTaskHead head)
    {
        messageQueue.Enqueue(new TaskSizeChangedEventArgs
        {
            Name = head.Name,
            TaskSize = head.File.Size
        });
    }

    protected override async ValueTask<LinkedTask?> Transform(LinkedTask task)
    {
        long prevBytes = 0;
        var progress = new SyncProgress<ByteProgress>(e =>
        {
            var d = e.ProgressedBytes - prevBytes;
            //if (d < 1024 * 8) // 8kb
            //    return;
            prevBytes = e.ProgressedBytes;
            Interlocked.Add(ref progressedBytes, d);
        });

        var nextTask = await task.Execute(progress, default);
        return nextTask;
    }
}