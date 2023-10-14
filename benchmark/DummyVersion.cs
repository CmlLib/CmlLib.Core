using CmlLib.Core.Files;
using CmlLib.Core.Java;
using CmlLib.Core.ProcessBuilder;
using CmlLib.Core.Version;

namespace CmlLib.Core.Benchmarks;

public class DummyVersion : IVersion
{
    public string Id => throw new NotImplementedException();

    public string? InheritsFrom => throw new NotImplementedException();

    public IVersion? ParentVersion { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public AssetMetadata? AssetIndex => throw new NotImplementedException();

    public MFileMetadata? Client => throw new NotImplementedException();

    public JavaVersion? JavaVersion => throw new NotImplementedException();

    public MLibrary[] Libraries => throw new NotImplementedException();

    public string? Jar => throw new NotImplementedException();

    public MLogFileMetadata? Logging => throw new NotImplementedException();

    public string? MainClass => throw new NotImplementedException();

    public MArgument[] GameArguments => throw new NotImplementedException();

    public MArgument[] JvmArguments => throw new NotImplementedException();

    public DateTime ReleaseTime => throw new NotImplementedException();

    public string? Type => throw new NotImplementedException();

    public string? JarId => throw new NotImplementedException();

    public string? GetProperty(string key)
    {
        throw new NotImplementedException();
    }
}