using System.Text.Json.Serialization;

namespace CmlLib.Core.ModLoaders.LiteLoader;

public class LiteLoaderVersion
{
    public string? BaseVersion { get; set; }
    [JsonPropertyName("tweakClass")]
    public string? TweakClass { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("libraries")]
    public IEnumerable<LiteLoaderLibrary>? Libraries { get; set; }
}