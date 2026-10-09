# Native AOT performance measurements

Local measurements show faster process startup and JSON processing with Native
AOT for these inputs. Version sorting is slower with AOT. These are measurements
of CmlLib.Core operations, not of a complete launcher or Minecraft startup.

## Environment and method

- Date: 2026-10-09 (Asia/Seoul).
- Linux x64, AMD Ryzen 7 PRO 7840U, .NET 8.0.31, SDK 8.0.131.
- Core revision: `1caf00d`; both variants use the same benchmark code and inputs.
- Release, self-contained deployment, JSON reflection disabled. Both variants
  use source-generated JSON metadata; this is not a comparison with the former
  reflection-based implementation.
- Ordinary .NET uses the JIT with default tiered compilation/PGO settings and
  ReadyToRun disabled. Native AOT uses its default compiler optimization settings.
- Both use workstation GC and CPU affinity 2. No tiering, PGO, ReadyToRun,
  hardware-intrinsic, or GC environment overrides were set.
- No live HTTP requests or Minecraft downloads occur. Catalog operations use an
  in-process HTTP handler, whose overhead is included in those timings.

## Process startup and first operation

The parent process measures process creation until the child prints `READY`.
Each value is the median of 20 fresh processes per variant. Two discarded
launches warm the OS file cache before measurement. This measures a new process
with a warm file cache, not cold-boot or cold-disk startup.

`Empty entry point` measures entry-point startup. Other rows include fixture
loading, setup and the first completed operation. Sorting also includes loading
the manifest before its first sort. Process teardown is excluded.

| Operation | JIT (ms) | Native AOT (ms) |
|---|---:|---:|
| Empty entry point | 56.20 | 8.03 |
| Version metadata parsing | 97.25 | 17.63 |
| Version manifest loading | 96.06 | 18.33 |
| Version sorting | 101.90 | 18.99 |
| Java manifest loading | 90.16 | 11.59 |
| Fabric loader catalog | 92.95 | 12.35 |
| Quilt loader catalog | 88.25 | 12.70 |
| LiteLoader profile generation | 100.14 | 17.57 |

Entry-point startup falls by about 48 ms (7.0× faster). Completing the first
version parse falls by about 80 ms (5.5× faster) in this harness.

## Warmed operation throughput

Each variant/operation runs in three independent processes. Each process warms
up for at least two seconds, then runs seven calibrated batches of approximately
250 ms each. The reported value is the median of the three process medians.
Variants and operations run sequentially in a seeded randomized order. Return
checksums must match across variants and batches.

| Operation and input | JIT (µs/op) | Native AOT (µs/op) | JIT time / AOT time |
|---|---:|---:|---:|
| Version metadata, 150 libraries | 1907.85 | 541.29 | 3.52× |
| Manifest, 500 versions | 1137.00 | 668.86 | 1.70× |
| Sort 500 preloaded versions | 353.36 | 438.13 | 0.81× |
| Java manifest, 12 components | 60.57 | 15.31 | 3.96× |
| Fabric catalog, 100 loaders | 90.02 | 43.88 | 2.05× |
| Quilt catalog, 100 loaders | 91.13 | 44.17 | 2.06× |
| LiteLoader profile, 11 libraries | 21.61 | 13.73 | 1.57× |

Lower time is better; a ratio above 1 favors AOT. Sorting takes about 24% longer
with AOT. Version parsing accesses asset/client/log metadata, all libraries and
their rules/classifiers, and game/JVM arguments. Sorting uses preloaded metadata
in this throughput measurement.

LiteLoader repeatedly overwrites one file per process. Writes use the OS page
cache without `fsync`; this is serialization plus buffered file I/O, not durable
disk throughput. Normal GC and async/delegate harness overhead are included.
Managed allocations are broadly similar between variants; this was not a
resident-memory measurement.

## Interpretation and reproduction

The results support an AOT benefit for process startup and the measured JSON
paths on this host. The slower sorting result also shows that AOT is not a
uniform throughput improvement. Network downloads, GUI initialization, actual
Java/Minecraft execution and other operating systems are outside this test.
These synthetic fixtures and a single host cannot establish universal speedups.

The [benchmark harness and reproduction commands](../benchmark/AotComparison/README.md)
describe the build settings, fixture generation and measurement protocol.
The [original report](../benchmark/AotComparison/results/2026-10-09-linux-x64/report.md),
[raw observations](../benchmark/AotComparison/results/2026-10-09-linux-x64/raw.json),
[JSON summary](../benchmark/AotComparison/results/2026-10-09-linux-x64/summary.json),
and [CSV summary](../benchmark/AotComparison/results/2026-10-09-linux-x64/summary.csv)
include startup p95, per-process ranges and allocation counters. There are 320
startup observations and 294 throughput batches in the saved run.
