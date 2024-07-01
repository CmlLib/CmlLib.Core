using System.Text.Json.Serialization;
using CmlLib.Core.Files;
using CmlLib.Core.Internals;
using CmlLib.Core.Java;

namespace CmlLib.Core.Version;

public class JsonVersionDTO
{
    [JsonPropertyName("inheritsFrom")]
    public string? InheritsFrom { get; set; }

    [JsonPropertyName("assetIndex")]
    public AssetMetadata? AssetIndex { get; set; }

    [JsonPropertyName("assets")]
    public string? Assets { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("complianceLevel")]
    [JsonConverter(typeof(NumberToStringConverter))]
    public string? ComplianceLevel { get; set; }

    [JsonPropertyName("javaVersion")]
    public JavaVersion? JavaVersion { get; set; }

    [JsonPropertyName("jar")]
    public string? Jar { get; set; }

    [JsonPropertyName("mainClass")]
    public string? MainClass { get; set; }

    [JsonPropertyName("minecraftArguments")]
    public string? MinecraftArguments { get; set; }

    [JsonPropertyName("minimumLauncherVersion")]
    [JsonConverter(typeof(NumberToStringConverter))]
    public string? MinimumLauncherVersion { get; set; }

    [JsonPropertyName("releaseTime")]
    [JsonConverter(typeof(SafeDateTimeOffsetJsonConverter))]
    public DateTimeOffset ReleaseTime { get; set; }

    [JsonPropertyName("time")]
    [JsonConverter(typeof(SafeDateTimeOffsetJsonConverter))]
    public DateTimeOffset Time { get; set; }

    [JsonPropertyName("type")]
    [JsonConverter(typeof(NumberToStringConverter))]
    public string? Type { get; set; }
}