using System.Text.Json.Serialization;

namespace CmlLib.Core.Files;

public class AssetMetadata : MFileMetadata
{
    [JsonPropertyName("totalSize")]
    public long TotalSize { get; set; }   
}