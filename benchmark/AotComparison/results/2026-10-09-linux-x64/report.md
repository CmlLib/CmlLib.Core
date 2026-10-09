# Native AOT versus JIT: local measurements

Measured 2026-10-09 (Asia/Seoul) on Linux x64, AMD Ryzen 7 PRO 7840U, .NET 8.0.31. Core revision: `1caf00d`. No tiering/PGO/ReadyToRun/hardware-intrinsic/GC environment overrides were set.

Both binaries use identical code, .NET 8, self-contained deployment and generated JSON metadata. JIT uses the default tiered compilation/PGO; ReadyToRun is disabled. No network requests occur.

Startup is parent-observed process spawn → READY output. `empty` isolates entry-point startup; other rows include fixture loading, setup and the first completed operation. Every run starts a new process, with a warm OS file cache (not a cold boot).

Steady-state: 3 independent processes per variant/operation, 2s warmup, 7 × approximately 250ms measured batches. Values are medians of the process medians. Checksum equality is enforced.

| Operation | JIT startup ms | AOT startup ms | JIT µs/op | AOT µs/op | AOT throughput speedup |
|---|---:|---:|---:|---:|---:|
| empty | 56.20 | 8.03 | — | — | — |
| version | 97.25 | 17.63 | 1907.85 | 541.29 | 3.52× |
| manifest | 96.06 | 18.33 | 1137.00 | 668.86 | 1.70× |
| sort | 101.90 | 18.99 | 353.36 | 438.13 | 0.81× |
| java | 90.16 | 11.59 | 60.57 | 15.31 | 3.96× |
| fabric | 92.95 | 12.35 | 90.02 | 43.88 | 2.05× |
| quilt | 88.25 | 12.70 | 91.13 | 44.17 | 2.06× |
| liteloader_write | 100.14 | 17.57 | 21.61 | 13.73 | 1.57× |

Inputs: 150-library version metadata, 500-version manifest/sort, 12 Java components, 100 Fabric/Quilt loaders, LiteLoader profile with 11 libraries. Mock HTTP overhead is included in manifest/catalog operations. LiteLoader writes overwrite one file per process, using the OS page cache without fsync; this is not durable disk throughput.

Affinity: [2]. Runtime metadata: `{'aot': {'runtime': '.NET 8.0.31', 'dynamic_code_compiled': 'False', 'server_gc': 'False'}, 'jit': {'runtime': '.NET 8.0.31', 'dynamic_code_compiled': 'True', 'server_gc': 'False'}}`. Deployment bytes (excluding symbols): `{'aot': 4723272, 'jit': 74550931}`.

Raw observations, per-process ranges, startup p95 and allocations are in raw.json/summary.json. Results are specific to this host, synthetic inputs and build settings. They do not measure download speeds, GUI startup or Java/Minecraft execution.
