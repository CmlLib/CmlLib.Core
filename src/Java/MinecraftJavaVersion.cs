using System.Text.Json.Serialization;

namespace CmlLib.Core.Java;

public record struct JavaVersion
{
    public JavaVersion(string component) : this(component, 0)
    {

    }

    public JavaVersion(string component, int majorVersion)
    {
        this.Component = component;
        this.MajorVersion = majorVersion;
    }

    [JsonPropertyName("component")]
    public string Component { get; set; }

    [JsonPropertyName("majorVersion")]
    public int MajorVersion { get; set; }
}