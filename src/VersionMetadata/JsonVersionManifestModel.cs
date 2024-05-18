using System.Text.Json.Serialization;
using CmlLib.Core.Files;
using CmlLib.Core.Internals;

namespace CmlLib.Core.VersionMetadata;

public class JsonVersionManifestModel
{
    [JsonPropertyName("latest")]
    public LatestVersion? Latest { get; set; }

    [JsonPropertyName("versions")]
    public IEnumerable<JsonVersionMetadataModel> Versions { get; set; } = Enumerable.Empty<JsonVersionMetadataModel>();
}

public class LatestVersion
{
    [JsonPropertyName("release")]
    public string? Release { get; set; }

    [JsonPropertyName("snapshot")]
    public string? Snapshot { get; set; }
}

public record JsonVersionMetadataModel : MFileMetadata
{
    [JsonPropertyName("type")] 
    public string? Type { get; set; }

    [JsonPropertyName("time")]
    [JsonConverter(typeof(SafeDateTimeOffsetJsonConverter))]
    public DateTimeOffset Time { get; set; }

    [JsonPropertyName("releaseTime")]
    [JsonConverter(typeof(SafeDateTimeOffsetJsonConverter))]
    public DateTimeOffset ReleaseTime { get; set; }

    [JsonPropertyName("complianceLevel")]
    public int ComplianceLevel { get; set; }
}