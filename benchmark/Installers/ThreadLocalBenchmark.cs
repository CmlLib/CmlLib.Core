using BenchmarkDotNet.Attributes;

namespace CmlLib.Core.Benchmarks;

public class ThreadLocalBenchmark
{
    public int ThreadCount = 6;
    public int IterationCount = 1000;
    public int AggregateCount = 10;

    private ThreadLocal<long> smallThreadLocal1 = new(true);
    private ThreadLocal<long> smallThreadLocal2 = new(true);

    [Benchmark]
    public long OneThreadLocal()
    {
        var threads = new List<Thread>();
        for (int i = 0; i < ThreadCount; i++)
        {
            var thread = new Thread(() =>
            {
                for (int i = 0; i < IterationCount; i++)
                {
                    smallThreadLocal1.Value = smallThreadLocal1.Value + i;
                    smallThreadLocal2.Value = smallThreadLocal2.Value + IterationCount - i;
                }
            });
            threads.Add(thread);
        }

        foreach (var t in threads) t.Start();

        long result = 0;
        for (int i = 0; i < AggregateCount; i++)
        {
            result += smallThreadLocal1.Values.Sum() + smallThreadLocal2.Values.Sum();
        }

        foreach (var t in threads) t.Join();
        return result;
    }

    private ThreadLocal<ByteProgress> bigThreadLocal = new(true);

    [Benchmark]
    public long TwoThreadLocal()
    {
        var threads = new List<Thread>();
        for (int i = 0; i < ThreadCount; i++)
        {
            var thread = new Thread(() =>
            {
                for (int i = 0; i < IterationCount; i++)
                {
                    var stored = bigThreadLocal.Value;
                    bigThreadLocal.Value = new ByteProgress
                    (
                        totalBytes: stored.TotalBytes + i,
                        progressedBytes: stored.ProgressedBytes + IterationCount - i
                    );
                }
            });
            threads.Add(thread);
        }

        foreach (var t in threads) t.Start();

        long result = 0;
        for (int i = 0; i < AggregateCount; i++)
        {
            result += bigThreadLocal.Values.Select(v => v.TotalBytes + v.ProgressedBytes).Sum();
        }

        foreach (var t in threads) t.Join();
        return result;
    }
}