using System.Diagnostics;
using CmlLib.Core.Executors;
using CmlLib.Core.Installers;
using CmlLib.Core.Tasks;

namespace CmlLib.Core.Benchmarks;

public class SemaphoreSlimBenchmark : ExecutorBenchmarkBase
{
    private Dictionary<string, long> sizeStorage = null!;
    private ThreadLocal<Dictionary<string, ByteProgress>> progressStorage = null!;
    private SemaphoreSlim semaphore = null!;

    protected override void Setup()
    {
        sizeStorage = new Dictionary<string, long>();
        progressStorage = new ThreadLocal<Dictionary<string, ByteProgress>>(
            () => new Dictionary<string, ByteProgress>(),
            true);
        semaphore = new SemaphoreSlim(1);
    }

    protected override void OnTaskAdded(LinkedTaskHead head)
    {
        sizeStorage[head.Name] = head.File.Size;
    }

    protected override void OnReportEvent()
    {
        try
        {
            semaphore.Wait(100);

            long totalBytes = 0;
            long progressedBytes = 0;

            foreach (var dict in progressStorage.Values)
            {
                foreach (var kv in dict)
                {
                    totalBytes += kv.Value.TotalBytes;
                    progressedBytes += kv.Value.ProgressedBytes;
                    sizeStorage.Remove(kv.Key);
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
        finally
        {
            semaphore.Release();
        }
    }

    protected override async ValueTask<LinkedTask?> Transform(LinkedTask task)
    {
        var progress = new SyncProgress<ByteProgress>(e =>
        {
            var isLocked = false;
            try
            {
                if (semaphore.Wait(0))
                {
                    isLocked = true;
                    progressStorage.Value![task.Name] = e;
                    //Console.WriteLine("progress");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                if (isLocked)
                    semaphore.Release();
            }
        });

        var nextTask = await task.Execute(progress, default);
        if (nextTask == null)
        {
            try
            {
                semaphore.Wait(1000);
                var previousProgress = progressStorage.Value![task.Name];
                progressStorage.Value![task.Name] = new ByteProgress
                {
                    TotalBytes = previousProgress.TotalBytes,
                    ProgressedBytes = previousProgress.TotalBytes
                };
                //Console.WriteLine("completed");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                semaphore.Release();
            }
        }
        return nextTask;
    }
}