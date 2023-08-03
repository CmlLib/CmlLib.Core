using System.Text.Json.Serialization;
using CmlLib.Core.Files;
using CmlLib.Core.Java;

namespace CmlLib.Core.Version;

public class VersionJsonModel
{
    [JsonPropertyName("inheritsFrom")]
    public string? InheritsFrom { get; set; }

    [JsonPropertyName("assetIndex")]
    public AssetMetadata? AssetIndex { get; set; }

    [JsonPropertyName("assets")]
    public string? Assets { get; set; }

    [JsonPropertyName("complianceLevel")]
    public string? ComplianceLevel { get; set;}

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("javaVersion")]
    public MinecraftJavaVersion? JavaVersion { get; set; }

    [JsonPropertyName("jar")]
    public string? Jar { get; set; }

    [JsonPropertyName("mainClass")]
    public string? MainClass { get; set; }

    [JsonPropertyName("minecraftArguments")]
    public string? MinecraftArguments { get; set; }

    [JsonPropertyName("minimumLauncherVersion")]
    public string? MinimumLauncherVersion { get; set; }

    [JsonPropertyName("releaseTime")]
    public DateTime ReleaseTime { get; set; }

    [JsonPropertyName("time")]
    public DateTime Time { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}