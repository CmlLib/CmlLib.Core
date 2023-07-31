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
    DateTime? ReleaseTime { get; }

    Task<MVersion> GetVersionAsync();
    Task<MVersion> GetAndSaveVersionAsync(MinecraftPath minecraftPath);
    Task SaveVersionAsync(MinecraftPath minecraftPath);
}
