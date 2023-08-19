using System.Diagnostics;
using BenchmarkDotNet.Running;
using CmlLib.Core.Benchmarks;
using CmlLib.Core.Executors;

var summary = BenchmarkRunner.Run<ExecutorsBenchmark>();
return;

//await once();
//async Task once()
{
    var benchmark = new SemaphoreSlimBenchmark();
    benchmark.Verbose = true;
    benchmark.Size = 1024 * 1;
    benchmark.Count = 1024 * 256;
    //benchmark.MaxParallelism = 1;
    //benchmark.MaxParallelism = 12;
    //benchmark.Count = 1024 * 16;

    await benchmark.IterationSetup();
    var sw = new Stopwatch();
    sw.Start();
    await benchmark.Benchmark();
    sw.Stop();
    Console.WriteLine("Done");
    Console.WriteLine(sw.Elapsed.ToString());
}