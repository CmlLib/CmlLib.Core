using System.Text.Json.Serialization;

namespace CmlLib.Core.ModLoaders.LiteLoader;

internal sealed class LiteLoaderProfile
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "release";

    [JsonPropertyName("mainClass")]
    public string MainClass { get; set; } = "net.minecraft.launchwrapper.Launch";

    [JsonPropertyName("inheritsFrom")]
    public string? InheritsFrom { get; set; }

    [JsonPropertyName("jar")]
    public string? Jar { get; set; }

    [JsonPropertyName("libraries")]
    public IEnumerable<LiteLoaderLibrary>? Libraries { get; set; }

    [JsonPropertyName("minecraftArguments")]
    public string? MinecraftArguments { get; set; }
}
