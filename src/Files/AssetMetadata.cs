using System.Text.Json.Serialization;

namespace CmlLib.Core.Files;

public record AssetMetadata : MFileMetadata
{
    [JsonPropertyName("totalSize")]
    public long TotalSize { get; set; }   
}