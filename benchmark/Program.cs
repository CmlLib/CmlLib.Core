using BenchmarkDotNet.Running;
using CmlLib.Core.Benchmarks;

var summary = BenchmarkRunner.Run<TPLTaskExecutorBenchmark>();

async Task once()
{
    var benchmark = new TPLTaskExecutorBenchmark();
    benchmark.GlobalSetup();
    benchmark.IterationSetup();
    await benchmark.Benchmark();
    benchmark.IterationCleanup();
    Console.WriteLine("Done");
}