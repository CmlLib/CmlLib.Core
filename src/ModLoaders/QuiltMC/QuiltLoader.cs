using System.Text.Json.Serialization;

namespace CmlLib.Core.ModLoaders.QuiltMC
{
    public class QuiltLoader
    {
        [JsonPropertyName("separator")]
        public string? Separator { get; set; }
        [JsonPropertyName("build")]
        public int? Build { get; set; }
        [JsonPropertyName("maven")]
        public string? Maven { get; set; }
        [JsonPropertyName("version")]
        public string? Version { get; set; }
        [JsonPropertyName("stable")]
        public bool Stable { get; set; } = true;
    }
}
