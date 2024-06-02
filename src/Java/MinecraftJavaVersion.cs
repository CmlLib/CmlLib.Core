using System.Text.Json.Serialization;

namespace CmlLib.Core.Java;

public record JavaVersion
{
    public JavaVersion(string component) : this(component, 0)
    {

    }

    [JsonConstructor]
    public JavaVersion(string component, int majorVersion)
    {
        this.Component = component;
        this.MajorVersion = majorVersion;
    }

    [JsonPropertyName("component")]
    public string Component { get; }

    [JsonPropertyName("majorVersion")]
    public int MajorVersion { get; }
}