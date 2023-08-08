using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CmlLib.Core.Executors;
using CmlLib.Core.FileExtractors;
using CmlLib.Core.Version;

namespace CmlLib.Core.Benchmarks;

[SimpleJob(RunStrategy.Monitoring, iterationCount: 10)]
public class TPLTaskExecutorBenchmark
{
    public static bool Verbose { get; set; } = false;

    private int parallelism = 6;
    private int extractorCount = 4;

    private MinecraftPath MinecraftPath = new MinecraftPath();
    private IVersion DummyVersion = new DummyVersion();
    private TestFileExtractor[] Extractors;
    private TPLTaskExecutor Executor;

    [GlobalSetup]
    public void GlobalSetup()
    {
    }

    [IterationSetup]
    public void IterationSetup()
    {
        Extractors = new TestFileExtractor[extractorCount];
        for (int i = 0; i < extractorCount; i++)
        {
            var path = Path.GetFullPath("./benchmark" + i);
            Extractors[i] = new TestFileExtractor(path, 1024, 1024*1024/2);
            Extractors[i].Setup();
        }
        Executor = new TPLTaskExecutor(parallelism);

        if (Verbose)
            Executor.Progress += (s, e) => e.Print();
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
        await Executor.Install(Extractors, MinecraftPath, DummyVersion);
    }
}