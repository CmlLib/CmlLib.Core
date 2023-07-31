using System.Text.Json.Serialization;

namespace CmlLib.Core.VersionMetadata;

public class JsonVersionMetadataModel
{
    [JsonPropertyName("id")] 
    public string? Name { get; set; }

    [JsonPropertyName("type")] 
    public string? Type { get; set; }

    [JsonPropertyName("releaseTime")] 
    public DateTime? ReleaseTime { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}