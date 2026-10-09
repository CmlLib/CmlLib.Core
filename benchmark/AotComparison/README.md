# Native AOT versus JIT

This dependency-free harness compares the same CmlLib.Core code under Native AOT
and the ordinary .NET 8 JIT. Both builds are self-contained and use generated
JSON metadata with reflection disabled. The JIT build is untrimmed, has
ReadyToRun disabled, and retains default tiered compilation/PGO settings.

## Reproduce

From the repository root, with .NET 8, Python 3, and Native AOT build prerequisites:

```sh
dotnet publish benchmark/AotComparison/AotComparison.csproj -c Release -r linux-x64 -p:BenchmarkAot=true -p:NuGetAudit=false -m:1 -nr:false -o /tmp/cmllib-aot-benchmark/aot
dotnet publish benchmark/AotComparison/AotComparison.csproj -c Release -r linux-x64 -p:BenchmarkAot=false -p:PublishTrimmed=false -p:NuGetAudit=false -m:1 -nr:false -o /tmp/cmllib-aot-benchmark/jit
python3 benchmark/AotComparison/run.py --aot /tmp/cmllib-aot-benchmark/aot/AotComparison --jit /tmp/cmllib-aot-benchmark/jit/AotComparison --output /tmp/cmllib-aot-benchmark/results --cpu 2
```

Choose an available CPU for `--cpu`, or omit it to allow normal scheduling.
Do not run the two publishes concurrently: they share intermediate build files.
The benchmark itself runs variants sequentially, in a seeded randomized order.

`BenchmarkAot` controls only the executable project, avoiding a global
`PublishAot=true` override on the library's `netstandard2.0` target.

## Measurements

- **Startup:** 20 fresh processes per variant and operation. The parent measures
  process spawn until the app prints `READY`. `empty` measures entry-point
  startup; operation rows also include fixture loading/setup and the first
  completed operation. Two discarded launches warm the OS file cache. These
  measurements are process-cold, not cold-boot/disk-cold.
- **Steady-state:** three independent processes per variant and operation,
  each with at least two seconds of warmup, followed by seven calibrated
  approximately 250 ms batches. The result is the median of the three process
  medians, with per-process ranges also retained. Timings include normal GC and
  async/delegate overhead from the harness.
- **Correctness:** return-value checksums must match across variants and batches.
  `raw.json` includes runtime metadata and the dynamically compiled-code flag.
- **Allocation:** process-wide managed allocation counters; excludes native
  memory/JIT code and is not a resident-memory measurement.

Synthetic fixtures exercise full version parsing with 150 libraries and their
rules/classifiers/arguments, a 500-entry version manifest, sorting those 500
entries, 12 Java components, and 100 Fabric/Quilt loaders. Catalog requests use
an in-process HTTP handler; no network/download time is measured. Sorting uses
preloaded metadata in steady-state; its startup row includes that preload.

LiteLoader profile generation writes 11 libraries and game arguments to a
temporary directory. Steady-state iterations overwrite the same file and do
not fsync: this measures serialization and buffered file I/O, not durable disk
throughput. Fixture generation and output formatting are outside batch timing.

The runner writes generated fixtures, raw observations, a CSV/JSON summary, and
a Markdown report. These local microbenchmarks cannot predict GUI startup,
download speed, Java startup, or end-to-end Minecraft installation performance.

The [benchmark results documentation](../../docs/native-aot-benchmarks.md)
summarizes the 2026-10-09 Linux x64 run. The [original measurements](results/2026-10-09-linux-x64/report.md)
include the raw observations and summary files alongside the report.
