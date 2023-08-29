using CmlLib.Core.Installers;
using CmlLib.Core.Tasks;

namespace CmlLib.Core.Benchmarks;

public class NoLockBenchmark : ExecutorBenchmarkBase
{
    private ThreadLocal<ByteProgress> progressStorage = null!;

    protected override void Setup()
    {
        progressStorage = new ThreadLocal<ByteProgress>(
            () => new ByteProgress(),
            true);
    }

    protected override void OnTaskAdded(LinkedTaskHead head)
    {
        progressStorage.Value = new ByteProgress
        {
            TotalBytes = progressStorage.Value.TotalBytes + head.File.Size,
            ProgressedBytes = 0
        };
    }

    protected override void OnReportEvent()
    {
        long totalBytes = 0;
        long progressedBytes = 0;
        foreach (var p in progressStorage.Values)
        {
            totalBytes += p.TotalBytes;
            progressedBytes += p.ProgressedBytes;
        }

        FireProgress(new ByteProgress
        {
            TotalBytes = totalBytes,
            ProgressedBytes = progressedBytes
        });
    }

    protected override async ValueTask<LinkedTask?> Transform(LinkedTask task)
    {
        var lastProgress = new ByteProgress
        {
            TotalBytes = task.Size,
            ProgressedBytes = 0
        };
        var progress = new SyncProgress<ByteProgress>(e =>
        {
            progressStorage.Value = new ByteProgress
            {
                TotalBytes = progressStorage.Value.TotalBytes + e.TotalBytes - lastProgress.TotalBytes,
                ProgressedBytes = progressStorage.Value.ProgressedBytes + e.ProgressedBytes - lastProgress.ProgressedBytes
            };
            lastProgress = e;
        });

        var nextTask = await task.Execute(progress, default);

        if (nextTask == null)
        {
            progress.Report(new ByteProgress
            {
                TotalBytes = lastProgress.TotalBytes,
                ProgressedBytes = lastProgress.TotalBytes
            });
        }
        return nextTask;
    }
}