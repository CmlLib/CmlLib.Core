using System.Text.Json.Serialization;

namespace CmlLib.Core.Files;

public record MFileMetadata
{   
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    [JsonPropertyName("sha1")]
    public string? Sha1 { get; set; }

    [JsonPropertyName("checksums")]
    public IReadOnlyCollection<string>? Checksums { get; set; }

    [JsonPropertyName("size")]
    public long Size { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    public string? GetSha1()
    {
        var result = Sha1;
        if (string.IsNullOrEmpty(result))
            result = Checksums?.FirstOrDefault();
        return result;
    }
}
