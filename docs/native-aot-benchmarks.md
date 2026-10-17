# Native AOT benchmark summary

Measured on 2026-10-09 (Asia/Seoul), Linux x64, Ryzen 7 PRO 7840U,
.NET 8.0.31 / SDK 8.0.131, Core revision `1caf00d`. Both builds use the
same code, Release/self-contained deployment, and source-generated JSON.
The JIT build has ReadyToRun disabled and default tiered compilation/PGO.

## Startup

Median of 20 fresh processes per variant, from process creation to entry-point
output or the first completed operation. The OS file cache is warm; the version
row includes fixture loading and setup.

| Operation | JIT (ms) | Native AOT (ms) |
|---|---:|---:|
| Empty entry point | 56.20 | 8.03 |
| First version parse | 97.25 | 17.63 |

## Warmed processing

Median of three independent process medians: at least 2 s warmup, then seven
approximately 250 ms batches per process. Lower time is better.

| Operation and input | JIT (µs/op) | Native AOT (µs/op) |
|---|---:|---:|
| Version metadata, 150 libraries | 1907.85 | 541.29 |
| Manifest, 500 versions | 1137.00 | 668.86 |
| Sort 500 preloaded versions | 353.36 | 438.13 |
| Java manifest, 12 components | 60.57 | 15.31 |
| Fabric catalog, 100 loaders | 90.02 | 43.88 |
| Quilt catalog, 100 loaders | 91.13 | 44.17 |
| LiteLoader profile, 11 libraries | 21.61 | 13.73 |

AOT improves startup and the measured JSON/profile operations on this host;
sorting takes about 24% longer. Managed allocations are effectively unchanged
(less than 0.01% difference); process resident memory was not measured.

These are synthetic, single-host measurements with workstation GC and CPU
affinity 2. HTTP is mocked; LiteLoader writes are buffered overwrites without
fsync. They do not measure download speed, GUI startup, Java/Minecraft execution,
or durable disk throughput, and do not establish universal AOT speedups.
