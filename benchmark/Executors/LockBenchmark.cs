using CmlLib.Core.Executors;
using CmlLib.Core.Tasks;

namespace CmlLib.Core.Benchmarks;

public class LockBenchmark : ExecutorBenchmarkBase
{
    private Dictionary<string, long> sizeStorage = null!;
    private ThreadLocal<Dictionary<string, ByteProgress>> progressStorage = null!;

    protected override void Setup()
    {
        progressStorage = new ThreadLocal<Dictionary<string, ByteProgress>>(
            () => new Dictionary<string, ByteProgress>(),
            true);
        sizeStorage = new Dictionary<string, long>();
    }

    protected override void OnTaskAdded(LinkedTaskHead head)
    {
        sizeStorage[head.Name] = head.File.Size;
    }

    protected override void OnReportEvent()
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
                    sizeStorage.Remove(kv.Key);
                }
            }
        }

        foreach (var kv in sizeStorage)
            totalBytes += kv.Value;

        FireProgress(new ByteProgress
        {
            TotalBytes = totalBytes,
            ProgressedBytes = progressedBytes
        });
    }

    protected override async ValueTask<LinkedTask?> Transform(LinkedTask t)
    {
        var task = t as DownloadTask;
        if (task == null)
            return t.NextTask;

        var progress = new SyncProgress<ByteProgress>(e =>
        {
            lock (progressStorage.Value!)
            {
                progressStorage.Value![task.Name] = e;
            }
        });

        var nextTask = await task.Execute(progress, default);

        if (nextTask == null)
        {
            progress.Report(new ByteProgress
            {
                TotalBytes = task.Size,
                ProgressedBytes = task.Size
            });
        }
        return nextTask;
    }
}