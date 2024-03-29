using BenchmarkDotNet.Running;
using CmlLib.Core.Benchmarks;

//Console.WriteLine("Hello, World!");
var summary = BenchmarkRunner.Run<ThreadLocalBenchmark>();