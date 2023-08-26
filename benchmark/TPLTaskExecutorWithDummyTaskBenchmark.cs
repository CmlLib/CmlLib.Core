using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CmlLib.Core.Executors;
using CmlLib.Core.FileExtractors;
using CmlLib.Core.Version;

namespace CmlLib.Core.Benchmarks;

public class TPLTaskExecutorWithDummyTaskBenchmark
{
    private int parallelism = 6;
    private int extractorCount = 8;

    public static bool Verbose = false;
    public static TaskExecutorProgressChangedEventArgs? FileProgressArgs;
    public static ByteProgress BytesProgressArgs;

    private MinecraftPath MinecraftPath = new MinecraftPath();
    private IVersion DummyVersion = new DummyVersion();
    private DummyTaskExtractor[] Extractors;
    private TPLTaskExecutor Executor;

    private ByteProgress previousEvent;
    private object consoleLock = new object();
    private string? bottomMsg;

    [IterationSetup]
    public void IterationSetup()
    {
        Extractors = new DummyTaskExtractor[extractorCount];
        for (int i = 0; i < extractorCount; i++)
            Extractors[i] = new DummyTaskExtractor(i.ToString(), 1024);
        Executor = new TPLTaskExecutor(parallelism);
        Executor.FileProgress += (s, e) => FileProgressArgs = e;
        Executor.ByteProgress += (s, e) => BytesProgressArgs = e;
        if (Verbose)
        {
            Executor.FileProgress += (s, e) => 
            {
                lock (consoleLock)
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    ExecutorsBenchmark.PrintProgress(e);
                    Console.WriteLine(bottomMsg);
                }
            };
            Executor.ByteProgress += (s, e) => 
            {
                lock (consoleLock)
                {
                    if (previousEvent.ProgressedBytes >= e.ProgressedBytes)
                        return;
                    previousEvent = e;
                    bottomMsg = $"{e.ProgressedBytes} / {e.TotalBytes}          ";
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.WriteLine(bottomMsg);
                }
            };
        }
    }

    [Benchmark]
    public async Task Benchmark()
    {
        await Executor.Install(
            Extractors, 
            MinecraftPath, 
            DummyVersion, 
            default);
    }
}