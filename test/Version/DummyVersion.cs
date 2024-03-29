using CmlLib.Core.Files;
using CmlLib.Core.Java;
using CmlLib.Core.ProcessBuilder;
using CmlLib.Core.Version;

namespace CmlLib.Core.Test.Version;

public class DummyVersion : IVersion
{
    public DummyVersion(string id) => Id = id;

    public string Id { get; set; }

    public string? InheritsFrom { get; set; }

    public IVersion? ParentVersion { get; set; }

    public AssetMetadata? AssetIndex { get; set; }

    public MFileMetadata? Client { get; set; }

    public JavaVersion? JavaVersion { get; set; }

    public IReadOnlyCollection<MLibrary> Libraries { get; set; } = Array.Empty<MLibrary>();

    public string? Jar { get; set; }

    public MLogFileMetadata? Logging { get; set; }

    public string? MainClass { get; set; }

    public IReadOnlyCollection<MArgument> GameArguments { get; set; } = Array.Empty<MArgument>();

    public IReadOnlyCollection<MArgument> JvmArguments { get; set; } = Array.Empty<MArgument>();

    public DateTime ReleaseTime { get; set; }

    public string? Type { get; set; }

    public string MainJarId { get; set; }

    public string? GetProperty(string key)
    {
        throw new NotImplementedException();
    }
}