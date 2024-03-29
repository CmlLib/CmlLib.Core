namespace CmlLib.Core.Files;

public class MinecraftJavaManifestMetadata
{
    public MinecraftJavaManifestMetadata(string os, string component)
    {
        OS = os;
        Component = component;
    }

    public string OS { get; set; }
    public string Component { get; set; }
    public MFileMetadata? Metadata { get; set; }
    public string? VersionName { get; set; }
    public string? VersionReleased { get; set; }

    public string? GetMajorVersion() => VersionName?.Split('.')?[0];
}