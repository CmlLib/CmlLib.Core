using BenchmarkDotNet.Attributes;
using CmlLib.Core.Executors;
using CmlLib.Core.Installers;

namespace CmlLib.Core.Benchmarks;

[MemoryDiagnoser(false)]
public class ExecutorsBenchmark
{
    #nullable disable
    ConcurrentDictionaryBenchmark _concurrent;
    ConcurrentQueueBenchmark _queue;
    LockBenchmark _lock;
    SemaphoreSlimBenchmark _semaphore;
    ThreadLocalBenchmark _thread;
    NoLockBenchmark _nolock;

    [IterationSetup]
    public void IterationSetup()
    {
        _concurrent = new ConcurrentDictionaryBenchmark();
        _concurrent.IterationSetup().Wait();

        _queue = new ConcurrentQueueBenchmark();
        _queue.IterationSetup().Wait();

        _lock = new LockBenchmark();
        _lock.IterationSetup().Wait();

        _semaphore = new SemaphoreSlimBenchmark();
        _semaphore.IterationSetup().Wait();

        _thread = new ThreadLocalBenchmark();
        _thread.IterationSetup().Wait();

        _nolock = new NoLockBenchmark();
        _nolock.IterationSetup().Wait();
    }

    //[Benchmark(Baseline = true)]
    //public async Task StartConcurrent()
    //{
    //    await _concurrent.Benchmark();
    //}

    [Benchmark]
    public async Task StartLock()
    {
        await _lock.Benchmark();
    }

    [Benchmark]
    public async Task StartSemaphore()
    {
        await _semaphore.Benchmark();
    }

    [Benchmark]
    public async Task StartNoLock()
    {
        await _nolock.Benchmark();
    }

    //[Benchmark(Baseline = true)]
    //public async Task StartQueue()
    //{
    //    await _queue.Benchmark();
    //}

    //[Benchmark]
    //public async Task StartThread()
    //{
    //    await _thread.Benchmark();
    //}

    public static void PrintProgress(InstallerProgressChangedEventArgs e)
    {
        //if (status != TaskStatus.Done) return;
        //if (proceed % 100 != 0) return;
        var now = DateTime.Now.ToString("hh:mm:ss.fff");
        Console.WriteLine($"[{now}][{e.ProgressedTasks}/{e.TotalTasks}][{e.EventType}] {e.Name}");
    }
}