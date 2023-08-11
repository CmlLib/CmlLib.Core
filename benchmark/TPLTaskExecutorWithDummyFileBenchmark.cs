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

    private MinecraftPath MinecraftPath = new MinecraftPath();
    private IVersion DummyVersion = new DummyVersion();
    private DummyTaskExtractor[] Extractors;
    private TPLTaskExecutor Executor;

    [IterationSetup]
    public void IterationSetup()
    {
        Extractors = new DummyTaskExtractor[extractorCount];
        for (int i = 0; i < extractorCount; i++)
            Extractors[i] = new DummyTaskExtractor(1024);
        Executor = new TPLTaskExecutor(parallelism);
    }

    [Benchmark]
    public async Task Benchmark()
    {
        await Executor.Install(Extractors, MinecraftPath, DummyVersion);
    }
}