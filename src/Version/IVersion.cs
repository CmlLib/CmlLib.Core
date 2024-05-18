using CmlLib.Core.Files;
using CmlLib.Core.Java;
using CmlLib.Core.ProcessBuilder;

namespace CmlLib.Core.Version;

public interface IVersion
{
    string Id { get; }
    string MainJarId { get; }
    string? InheritsFrom { get; }
    IVersion? ParentVersion { get; set; }
    AssetMetadata? AssetIndex { get; }
    MFileMetadata? Client { get; }
    JavaVersion? JavaVersion { get; }
    IReadOnlyCollection<MLibrary> Libraries { get; }
    string? Jar { get; }
    MLogFileMetadata? Logging { get; }
    string? MainClass { get; }
    IReadOnlyCollection<MArgument> GetGameArguments(bool isBaseVersion);
    IReadOnlyCollection<MArgument> GetJvmArguments(bool isBaseVersion);
    DateTimeOffset ReleaseTime { get; }
    string? Type { get; }

    string? GetProperty(string key);
}