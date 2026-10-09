#!/usr/bin/env python3
"""Paired process-start and warmed throughput measurements; standard library only."""
import argparse
import csv
import json
import os
import platform
import random
import statistics
import subprocess
import time
from pathlib import Path

OPERATIONS = ["version", "manifest", "sort", "java", "fabric", "quilt", "liteloader_write"]


def fixtures(directory):
    directory.mkdir(parents=True, exist_ok=True)
    library = lambda i: {
        "name": f"example:library-{i}:1.0",
        "downloads": {"artifact": {"path": f"example/library-{i}/1.0/library.jar", "url": "https://fixture/library.jar", "sha1": "a" * 40, "size": 100000 + i},
                      "classifiers": {"natives-linux": {"size": 20000, "url": "https://fixture/native.jar"}}},
        "natives": {"linux": "natives-linux"}, "rules": [{"action": "allow", "os": {"name": "linux"}}]}
    version = {"id": "1.20.1", "type": "release", "time": "2023-06-12T12:00:00Z", "releaseTime": "2023-06-12T12:00:00Z",
               "complianceLevel": 1, "minimumLauncherVersion": 21, "javaVersion": {"component": "java-runtime-gamma", "majorVersion": 17},
               "mainClass": "net.minecraft.client.main.Main", "assetIndex": {"id": "5", "size": 400000, "totalSize": 10000000},
               "downloads": {"client": {"url": "https://fixture/client.jar", "size": 23000000, "sha1": "b" * 40}},
               "logging": {"client": {"argument": "-Dlog4j.configurationFile=${path}", "type": "log4j2-xml", "file": {"id": "client.xml", "size": 1000}}},
               "libraries": [library(i) for i in range(150)],
               "arguments": {"game": ["--username", "${auth_player_name}", "--version", "${version_name}", {"rules": [{"action": "allow", "features": {"is_demo_user": True}}], "value": "--demo"}],
                             "jvm": ["-Xmx2G", "-cp", "${classpath}", {"rules": [{"action": "allow", "os": {"name": "linux"}}], "value": "-Djava.library.path=${natives_directory}"}]}}
    versions = [{"id": f"1.{i // 100}.{i % 100}", "type": "release", "url": "https://fixture/version.json", "sha1": "c" * 40,
                 "time": "2023-06-12T12:00:00Z", "releaseTime": "2023-06-12T12:00:00Z", "complianceLevel": 1} for i in range(500)]
    random.Random(42).shuffle(versions)
    manifest = {"latest": {"release": "1.4.99", "snapshot": "23w24a"}, "versions": versions}
    java = {os_name: {f"java-runtime-{i}": [{"manifest": {"url": "https://fixture/java.json", "sha1": "d" * 40, "size": 300000},
                                          "version": {"name": "17.0.1", "released": "2023-06-12T12:00:00Z"}}] for i in range(4)}
            for os_name in ["linux", "windows-x64", "mac-os"]}
    loaders = [{"version": f"0.15.{i}", "maven": f"example:loader:0.15.{i}", "build": i, "separator": ".", "stable": i % 3 != 0} for i in range(100)]
    for name, value in {"version": version, "manifest": manifest, "java": java, "loaders": loaders}.items():
        (directory / f"{name}.json").write_text(json.dumps(value, separators=(",", ":")))


def once(executable, operation, directory):
    args = [str(executable), "empty"] if operation == "empty" else [str(executable), "once", operation, str(directory)]
    start = time.perf_counter_ns()
    process = subprocess.Popen(args, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)
    ready = process.stdout.readline().strip()
    ready_ns = time.perf_counter_ns() - start
    output, error = process.communicate(timeout=30)
    total_ns = time.perf_counter_ns() - start
    if process.returncode or not ready.startswith("READY"):
        raise RuntimeError(f"{args}: {ready} {output} {error}")
    columns = ready.split("\t")
    return {"ready_ms": ready_ns / 1e6, "exit_ms": total_ns / 1e6,
            "operation_ms": float(columns[1]) if len(columns) > 1 else None,
            "checksum": int(columns[2]) if len(columns) > 2 else None}


def throughput(executable, operation, directory, args, expected):
    result = subprocess.run([str(executable), "throughput", operation, str(directory), str(args.warmup), str(args.samples), str(args.sample_ms)],
                            text=True, capture_output=True, check=True, timeout=120)
    rows, metadata = [], None
    for line in result.stdout.splitlines():
        parts = line.split("\t")
        if parts[0] == "META":
            metadata = {"runtime": parts[1], "dynamic_code_compiled": parts[2], "server_gc": parts[3]}
        elif parts[0] == "RESULT":
            iterations, ms, allocated, checksum = int(parts[1]), float(parts[2]), int(parts[3]), int(parts[4])
            if checksum != expected * iterations:
                raise RuntimeError(f"Output checksum mismatch: {operation}")
            rows.append({"iterations": iterations, "elapsed_ms": ms, "ns_per_op": ms * 1e6 / iterations,
                         "allocated_bytes_per_op": allocated / iterations, "checksum": checksum})
    if len(rows) != args.samples or metadata is None:
        raise RuntimeError(f"Incomplete benchmark output: {result.stdout}")
    return rows, metadata


def percentile(values, fraction):
    values = sorted(values)
    position = (len(values) - 1) * fraction
    low = int(position)
    high = min(low + 1, len(values) - 1)
    return values[low] + (values[high] - values[low]) * (position - low)


def summarize(raw):
    rows = []
    for operation in ["empty"] + OPERATIONS:
        row = {"operation": operation}
        for variant in ["jit", "aot"]:
            cold = [x["ready_ms"] for x in raw["startup"] if x["operation"] == operation and x["variant"] == variant]
            row[f"{variant}_startup_ms"] = statistics.median(cold)
            row[f"{variant}_startup_p95_ms"] = percentile(cold, .95)
            warm = [x for x in raw["throughput"] if x["operation"] == operation and x["variant"] == variant]
            if warm:
                # Each independent process contributes one median; do not treat its batches as independent trials.
                trial_values = [statistics.median(x["ns_per_op"] for x in warm if x["trial"] == trial) for trial in range(raw["settings"]["trials"])]
                row[f"{variant}_ns_per_op"] = statistics.median(trial_values)
                row[f"{variant}_trial_ns_min"] = min(trial_values)
                row[f"{variant}_trial_ns_max"] = max(trial_values)
                row[f"{variant}_allocated_bytes_per_op"] = statistics.median(x["allocated_bytes_per_op"] for x in warm)
        row["startup_speedup"] = row["jit_startup_ms"] / row["aot_startup_ms"]
        if operation != "empty":
            row["throughput_speedup"] = row["jit_ns_per_op"] / row["aot_ns_per_op"]
        rows.append(row)
    return rows


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--aot", type=Path, required=True)
    parser.add_argument("--jit", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--startup-runs", type=int, default=20)
    parser.add_argument("--trials", type=int, default=3)
    parser.add_argument("--warmup", type=float, default=2)
    parser.add_argument("--samples", type=int, default=7)
    parser.add_argument("--sample-ms", type=float, default=250)
    parser.add_argument("--cpu", type=int)
    args = parser.parse_args()
    if args.cpu is not None:
        os.sched_setaffinity(0, {args.cpu})
    args.output.mkdir(parents=True, exist_ok=True)
    directory = args.output / "fixtures"
    fixtures(directory)
    executables = {"aot": args.aot.resolve(), "jit": args.jit.resolve()}
    raw = {"environment": {"platform": platform.platform(), "affinity": sorted(os.sched_getaffinity(0)),
                            "git_commit": subprocess.check_output(["git", "rev-parse", "HEAD"], text=True).strip(),
                            "runtime_overrides": {key: os.environ[key] for key in [
                                "DOTNET_TieredCompilation", "DOTNET_TieredPGO", "DOTNET_ReadyToRun", "DOTNET_EnableHWIntrinsic", "DOTNET_gcServer",
                                "COMPlus_TieredCompilation", "COMPlus_TieredPGO", "COMPlus_ReadyToRun", "COMPlus_EnableHWIntrinsic", "COMPlus_gcServer"
                            ] if key in os.environ},
                            "cpu_info": subprocess.check_output(["lscpu"], text=True),
                            "sdk_info": subprocess.check_output(["dotnet", "--info"], text=True)},
           "settings": {k: str(v) if isinstance(v, Path) else v for k, v in vars(args).items()},
           "fixture_bytes": {p.name: p.stat().st_size for p in directory.glob("*.json")},
           "startup": [], "throughput": [], "runtime": {},
           "deploy_bytes": {variant: sum(p.stat().st_size for p in exe.parent.rglob("*") if p.is_file() and p.suffix not in {".pdb", ".dbg"}) for variant, exe in executables.items()}}
    rng = random.Random(42)
    expected = {}
    for operation in ["empty"] + OPERATIONS:
        for variant, exe in executables.items():
            for _ in range(2):
                once(exe, operation, directory)  # Warm the OS file cache, not the process/JIT.
        for trial in range(args.startup_runs):
            order = list(executables)
            rng.shuffle(order)
            for variant in order:
                result = once(executables[variant], operation, directory)
                if operation != "empty":
                    if operation in expected and expected[operation] != result["checksum"]:
                        raise RuntimeError(f"AOT/JIT output mismatch: {operation}")
                    expected[operation] = result["checksum"]
                raw["startup"].append(dict(operation=operation, variant=variant, trial=trial, **result))
        print(f"Startup measured: {operation}", flush=True)
    for trial in range(args.trials):
        operations = OPERATIONS.copy()
        rng.shuffle(operations)
        for operation in operations:
            order = list(executables)
            rng.shuffle(order)
            for variant in order:
                rows, metadata = throughput(executables[variant], operation, directory, args, expected[operation])
                raw["throughput"].extend(dict(operation=operation, variant=variant, trial=trial, **row) for row in rows)
                raw["runtime"][variant] = metadata
            print(f"Throughput measured: trial {trial + 1}/{args.trials}, {operation}", flush=True)
            (args.output / "raw.json").write_text(json.dumps(raw, indent=2))
    summary = summarize(raw)
    (args.output / "raw.json").write_text(json.dumps(raw, indent=2))
    (args.output / "summary.json").write_text(json.dumps(summary, indent=2))
    fields = list(dict.fromkeys(key for row in summary for key in row))
    with (args.output / "summary.csv").open("w", newline="") as f:
        writer = csv.DictWriter(f, fieldnames=fields)
        writer.writeheader()
        writer.writerows(summary)
    report = ["# Native AOT versus JIT: local measurements", "",
              "Both binaries use identical code, .NET 8, self-contained deployment and generated JSON metadata. JIT uses the default tiered compilation/PGO; ReadyToRun is disabled. No network requests occur.", "",
              "Startup is parent-observed process spawn → READY output. `empty` isolates entry-point startup; other rows include fixture loading, setup and the first completed operation. Every run starts a new process, with a warm OS file cache (not a cold boot).", "",
              f"Steady-state: {args.trials} independent processes per variant/operation, {args.warmup}s warmup, {args.samples} × approximately {args.sample_ms}ms measured batches. Values are medians of the process medians. Checksum equality is enforced.", "",
              "| Operation | JIT startup ms | AOT startup ms | JIT µs/op | AOT µs/op | AOT throughput speedup |", "|---|---:|---:|---:|---:|---:|"]
    for row in summary:
        warm = f"{row['jit_ns_per_op']/1000:.2f} | {row['aot_ns_per_op']/1000:.2f} | {row['throughput_speedup']:.2f}×" if row["operation"] != "empty" else "— | — | —"
        report.append(f"| {row['operation']} | {row['jit_startup_ms']:.2f} | {row['aot_startup_ms']:.2f} | {warm} |")
    report.extend(["", "Inputs: 150-library version metadata, 500-version manifest/sort, 12 Java components, 100 Fabric/Quilt loaders, LiteLoader profile with 11 libraries. Mock HTTP overhead is included in manifest/catalog operations. LiteLoader writes overwrite one file per process, using the OS page cache without fsync; this is not durable disk throughput.", "",
                   f"Affinity: {raw['environment']['affinity']}. Runtime metadata: `{raw['runtime']}`. Deployment bytes (excluding symbols): `{raw['deploy_bytes']}`.", "",
                   "Raw observations, per-process ranges, startup p95 and allocations are in raw.json/summary.json. Results are specific to this host, synthetic inputs and build settings. They do not measure download speeds, GUI startup or Java/Minecraft execution."])
    (args.output / "report.md").write_text("\n".join(report) + "\n")
    print(json.dumps(summary, indent=2), flush=True)


if __name__ == "__main__":
    main()
