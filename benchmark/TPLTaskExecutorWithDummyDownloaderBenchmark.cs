using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CmlLib.Core.Executors;
using CmlLib.Core.FileExtractors;
using CmlLib.Core.Installers;
using CmlLib.Core.Version;

namespace CmlLib.Core.Benchmarks;

public class TPLTaskExecutorWithDummyDownloaderBenchmark
{
    private int parallelism = 6;
    private int extractorCount = 8;

    public static InstallerProgressChangedEventArgs? FileProgressArgs;
    public static ByteProgress BytesProgressArgs;

    private MinecraftPath MinecraftPath = new MinecraftPath();
    private IVersion DummyVersion = new DummyVersion();
    private DummyDownloaderExtractor[] Extractors;
    private TPLGameInstaller Executor;

    [IterationSetup]
    public void IterationSetup()
    {
        Extractors = new DummyDownloaderExtractor[extractorCount];
        for (int i = 0; i < extractorCount; i++)
            Extractors[i] = new DummyDownloaderExtractor(i.ToString(), 1024, 1024*256);
        Executor = new TPLGameInstaller(parallelism);
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