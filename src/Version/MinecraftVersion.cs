using CmlLib.Core.Files;
using CmlLib.Core.Java;
using CmlLib.Core.ProcessBuilder;

namespace CmlLib.Core.Version;

public class MinecraftVersion : IVersion
{
    public MinecraftVersion(string id)
    {
        Id = id;
        MainJarId = id;
    }

    public string Id { get; }
    public string MainJarId { get; set; }
    public string? InheritsFrom { get; set; }
    public IVersion? ParentVersion { get; set; }
    public AssetMetadata? AssetIndex { get; set; }
    public MFileMetadata? Client { get; set; }
    public JavaVersion? JavaVersion { get; set; }
    public IReadOnlyCollection<MLibrary> Libraries => LibraryList;
    public List<MLibrary> LibraryList { get; set; } = [];
    public string? Jar { get; set; }
    public MLogFileMetadata? Logging { get; set; }
    public string? MainClass { get; set; }
    public DateTimeOffset ReleaseTime { get; set; }
    public string? Type { get; set; }

    public List<MArgument> GameArguments { get; set; } = [];
    public List<MArgument> GameArgumentsForBaseVersion { get; set; } = [];
    public List<MArgument> JvmArguments { get; set; } = [];
    public List<MArgument> JvmArgumentsForBaseVersion { get; set; } = [];

    public IReadOnlyCollection<MArgument> GetGameArguments(bool isBaseVersion) =>
        isBaseVersion ? GameArgumentsForBaseVersion : GameArguments;

    public IReadOnlyCollection<MArgument> GetJvmArguments(bool isBaseVersion) =>
        isBaseVersion ? JvmArgumentsForBaseVersion : JvmArguments;

    public string? GetProperty(string key) => null;
}