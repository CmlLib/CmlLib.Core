using System.Text.Json.Serialization;

namespace CmlLib.Core.ModLoaders.FabricMC
{
    public class FabricLoader
    {
        [JsonPropertyName("separator")]
        public string? Separator { get; set; }

        [JsonPropertyName("build")]
        public int Build { get; set; }

        [JsonPropertyName("maven")]
        public string? Maven { get; set; }

        [JsonPropertyName("version")]
        public string? Version { get; set; }

        [JsonPropertyName("stable")]
        public bool Stable { get; set; } = true;
    }
}
