using BenchmarkDotNet.Attributes;
using Standart.Hash.xxHash;

namespace CmlLib.Core.Benchmarks;

public class HashBenchmark
{
    private byte[] Data;

    [GlobalSetup]
    public void GlobalSetup()
    {
        Data = new byte[1024*1024];
        Random.Shared.NextBytes(Data);
    }

    [Benchmark]
    public ulong TestXX()
    {
        return xxHash64.ComputeHash(Data, Data.Length);
    }

    [Benchmark]
    public byte[] TestMD5()
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        return md5.ComputeHash(Data);
    }

    [Benchmark]
    public byte[] TestSHA1()
    {
        using var sha1 = System.Security.Cryptography.SHA1.Create();
        return sha1.ComputeHash(Data);
    }
}