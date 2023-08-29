using System.Collections.Concurrent;
using CmlLib.Core.Executors;
using CmlLib.Core.Installers;
using CmlLib.Core.Tasks;

namespace CmlLib.Core.Benchmarks;

public class ThreadLocalBenchmark : ExecutorBenchmarkBase
{
    private ConcurrentDictionary<string, ByteProgress> totalProgress = null!;
    private ThreadLocal<int> lastUpdate = null!;

    protected override void Setup()
    {
        totalProgress = new ConcurrentDictionary<string, ByteProgress>();
        lastUpdate = new ThreadLocal<int>(() => 0);
    }

    protected override void OnTaskAdded(LinkedTaskHead head)
    {
        var p = new ByteProgress
        {
            TotalBytes = head.File.Size,
            ProgressedBytes = 0
        };
        totalProgress.AddOrUpdate(head.Name, p, (_, _) => p);
    }

    protected override void OnReportEvent()
    {
        long totalBytes = 0;
        long progressedBytes = 0;

        foreach (var kv in totalProgress)
        {
            totalBytes += kv.Value.TotalBytes;
            progressedBytes += kv.Value.ProgressedBytes;
        }

        FireProgress(new ByteProgress
        {
            TotalBytes = totalBytes,
            ProgressedBytes = progressedBytes
        });
    }

    protected async override ValueTask<LinkedTask?> Transform(LinkedTask t)
    {   
        var task = t as DownloadTask;
        if (task == null)
            return t.NextTask;

        var progress = new SyncProgress<ByteProgress>(e =>
        {
            if (Math.Abs(lastUpdate.Value - Environment.TickCount) > 256)
            {
                totalProgress.AddOrUpdate(task.Name, e, (_, _) => e);
                lastUpdate.Value = Environment.TickCount;
            }
        });

        var nextTask = await task.Execute(progress, default);

        if (nextTask == null)
        {
            var p = new ByteProgress
            {
                TotalBytes = task.Size,
                ProgressedBytes = task.Size
            };
            totalProgress.AddOrUpdate(task.Name, p, (_, _) => p);
        }
        return nextTask;
    }
}