using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CmlLib.Core.Executors;
using CmlLib.Core.FileExtractors;
using CmlLib.Core.Installers;
using CmlLib.Core.Version;

namespace CmlLib.Core.Benchmarks;

[SimpleJob(RunStrategy.Monitoring, iterationCount: 10)]
public class TPLTaskExecutorWithRandomFileBenchmark
{
    public static bool Verbose { get; set; } = false;

    private int parallelism = 6;
    private int extractorCount = 4;

    public static InstallerProgressChangedEventArgs? FileProgressArgs;
    public static ByteProgress BytesProgressArgs;

    private MinecraftPath MinecraftPath = new MinecraftPath();
    private IVersion DummyVersion = new DummyVersion();
    private RandomFileExtractor[] Extractors;
    private TPLGameInstaller Executor;

    [GlobalSetup]
    public void GlobalSetup()
    {
    }

    [IterationSetup]
    public void IterationSetup()
    {
        Extractors = new RandomFileExtractor[extractorCount];
        for (int i = 0; i < extractorCount; i++)
        {
            var path = Path.GetFullPath("./benchmark" + i);
            Extractors[i] = new RandomFileExtractor(path, 1024, 1024*1024/2);
            Extractors[i].Setup();
        }
        Executor = new TPLGameInstaller(parallelism);
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
        foreach (var item in Extractors)
        {
            item.Cleanup();
        }
    }

    [Benchmark]
    public async Task Benchmark()
    {
        await Executor.Install(
            Extractors, 
            MinecraftPath, 
            DummyVersion,
            TPLTaskExecutorWithDummyTaskBenchmark.RulesContext,
            TPLTaskExecutorWithDummyTaskBenchmark.FileProgress,
            TPLTaskExecutorWithDummyTaskBenchmark.ByteProgress,
            default);
    }
}