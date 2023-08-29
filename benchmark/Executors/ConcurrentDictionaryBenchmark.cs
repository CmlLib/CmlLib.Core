using System.Collections.Concurrent;
using CmlLib.Core.Benchmarks;
using CmlLib.Core.Installers;
using CmlLib.Core.Tasks;

namespace CmlLib.Core.Executors;

public class ConcurrentDictionaryBenchmark : ExecutorBenchmarkBase
{
    private ConcurrentDictionary<string, ByteProgress> progressStorage = null!;

    protected override void Setup()
    {
        progressStorage = new ConcurrentDictionary<string, ByteProgress>();
    }

    protected override void OnTaskAdded(LinkedTaskHead head)
    {
        progressStorage[head.Name] = new ByteProgress
        {
            TotalBytes = head.File.Size,
            ProgressedBytes = 0
        };
    }

    protected override void OnReportEvent()
    {
        long totalBytes = 0;
        long progressedBytes = 0;

        foreach (var kv in progressStorage)
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
            progressStorage.AddOrUpdate(task.Name, e, (_, _) => e);
        });

        var nextTask = await task.Execute(progress, default);

        if (nextTask == null)
        {
            var p = new ByteProgress
            {
                TotalBytes = task.Size,
                ProgressedBytes = task.Size
            };
            progress.Report(p);
        }
        return nextTask;
    }
}

