using System.Text.Json.Serialization;

namespace CmlLib.Core.Files;

public class MLogFileMetadata
{
    [JsonPropertyName("file")]
    public MFileMetadata? LogFile { get; set; }

    [JsonPropertyName("argument")]
    public string? Argument { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}
