using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CmlLib.Core.Executors;
using CmlLib.Core.FileExtractors;
using CmlLib.Core.Installers;
using CmlLib.Core.Rules;
using CmlLib.Core.Version;

namespace CmlLib.Core.Benchmarks;

public class TPLTaskExecutorWithDummyTaskBenchmark
{
    public static bool Verbose = false;
    public static InstallerProgressChangedEventArgs? FileProgressArgs;
    public static ByteProgress BytesProgressArgs;

    private static object consoleLock = new object();
    private static string? bottomMsg;
    private static ByteProgress previousEvent;
    public static RulesEvaluatorContext RulesContext = new RulesEvaluatorContext(LauncherOSRule.Current);
    public static IProgress<InstallerProgressChangedEventArgs> FileProgress = new SyncProgress<InstallerProgressChangedEventArgs>(e =>
    {
        FileProgressArgs = e;
        if (Verbose)
        {
            lock (consoleLock)
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                ExecutorsBenchmark.PrintProgress(e);
                Console.WriteLine(bottomMsg);
            }
        }
    });
    public static IProgress<ByteProgress> ByteProgress = new SyncProgress<ByteProgress>(e =>
    {
        BytesProgressArgs = e;
        if (Verbose)
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
        }
    });

    private int parallelism = 6;
    private int extractorCount = 8;

    private MinecraftPath MinecraftPath = new MinecraftPath();
    private IVersion DummyVersion = new DummyVersion();
    private DummyTaskExtractor[] Extractors;
    private TPLGameInstaller Executor;

    [IterationSetup]
    public void IterationSetup()
    {
        Extractors = new DummyTaskExtractor[extractorCount];
        for (int i = 0; i < extractorCount; i++)
            Extractors[i] = new DummyTaskExtractor(i.ToString(), 1024);
        Executor = new TPLGameInstaller(parallelism);
    }

    [Benchmark]
    public async Task Benchmark()
    {
        await Executor.Install(
            Extractors,
            MinecraftPath,
            DummyVersion,
            RulesContext,
            FileProgress,
            ByteProgress,
            default);
    }
}