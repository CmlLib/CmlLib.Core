using System.Text.Json.Serialization;

namespace CmlLib.Core.Java;

public class MinecraftJavaVersion
{
    [JsonPropertyName("component")]
    public string? Component { get; set; }

    [JsonPropertyName("majorVersion")]
    public string? MajorVersion { get; set; }
}