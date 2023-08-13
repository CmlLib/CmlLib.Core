using System.Diagnostics;
using BenchmarkDotNet.Running;
using CmlLib.Core.Benchmarks;

//var summary = BenchmarkRunner.Run<TPLTaskExecutorWithDummyDownloaderBenchmark>();
//return;

await once();
async Task once()
{
    var benchmark = new TPLTaskExecutorWithDummyDownloaderBenchmark();
    TPLTaskExecutorWithDummyDownloaderBenchmark.Verbose = true;
    benchmark.IterationSetup();
    var sw = new Stopwatch();
    sw.Start();
    await benchmark.Benchmark();
    sw.Stop();
    Console.WriteLine("Done");
    Console.WriteLine(sw.Elapsed.ToString());
}