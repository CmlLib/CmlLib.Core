using CmlLib.Core.Version;

namespace CmlLib.Core.VersionMetadata;

/// <summary>
/// Represent version metadata
/// It does not contains actual version data, but contains some metadata and the way to get version data (MVersion)
/// </summary>
public interface IVersionMetadata
{
    string Name { get; }
    string? Type { get; }
    DateTimeOffset ReleaseTime { get; }

    Task<IVersion> GetVersionAsync();
    Task<IVersion> GetAndSaveVersionAsync(MinecraftPath minecraftPath);
    Task SaveVersionAsync(MinecraftPath minecraftPath);
}
