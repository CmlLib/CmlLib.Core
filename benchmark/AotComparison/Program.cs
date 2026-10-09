using System.Diagnostics;
using System.Globalization;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CmlLib.Core.Benchmarks.AotComparison;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        if (args[0] == "empty")
        {
            Console.WriteLine("READY");
            return;
        }

        using var operation = await Operation.Create(args[1], args[2]);
        if (args[0] == "once")
        {
            var start = Stopwatch.GetTimestamp();
            var checksum = await operation.Execute();
            var elapsed = Stopwatch.GetElapsedTime(start).TotalMilliseconds;
            Console.WriteLine($"READY\t{elapsed:R}\t{checksum}");
            return;
        }

        Console.WriteLine($"META\t{RuntimeInformation.FrameworkDescription}\t{RuntimeFeature.IsDynamicCodeCompiled}\t{GCSettings.IsServerGC}");
        var warmupSeconds = double.Parse(args[3], CultureInfo.InvariantCulture);
        var samples = int.Parse(args[4], CultureInfo.InvariantCulture);
        var sampleMs = double.Parse(args[5], CultureInfo.InvariantCulture);
        var warmupStart = Stopwatch.GetTimestamp();
        var iterations = 1;
        double lastMs;
        do
        {
            var result = await Measure(operation, iterations);
            lastMs = result.ElapsedMs;
            if (lastMs < 100 && iterations < 1_000_000)
                iterations = Math.Min(1_000_000, iterations * 2);
        } while (Stopwatch.GetElapsedTime(warmupStart).TotalSeconds < warmupSeconds);
        // Calibrate again because the final warmup batch may have doubled its count.
        var calibration = await Measure(operation, iterations);
        iterations = Math.Clamp((int)(iterations * sampleMs / Math.Max(calibration.ElapsedMs, 0.001)), 1, 1_000_000);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        for (var i = 0; i < samples; i++)
        {
            var result = await Measure(operation, iterations);
            Console.WriteLine($"RESULT\t{iterations}\t{result.ElapsedMs:R}\t{result.AllocatedBytes}\t{result.Checksum}");
        }
    }

    private static async Task<(double ElapsedMs, long AllocatedBytes, long Checksum)> Measure(Operation operation, int iterations)
    {
        var allocated = GC.GetTotalAllocatedBytes(precise: true);
        var start = Stopwatch.GetTimestamp();
        long checksum = 0;
        for (var i = 0; i < iterations; i++)
            checksum += await operation.Execute();
        var elapsed = Stopwatch.GetElapsedTime(start).TotalMilliseconds;
        return (elapsed, GC.GetTotalAllocatedBytes(precise: true) - allocated, checksum);
    }
}
