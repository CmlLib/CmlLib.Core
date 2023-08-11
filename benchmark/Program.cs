using BenchmarkDotNet.Running;
using CmlLib.Core.Benchmarks;

var summary = BenchmarkRunner.Run<TPLTaskExecutorWithDummyTaskBenchmark>();
return;

await once();
async Task once()
{
    var benchmark = new TPLTaskExecutorWithDummyTaskBenchmark();
    benchmark.IterationSetup();
    await benchmark.Benchmark();
    Console.WriteLine("Done");
}