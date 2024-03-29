using System.Text.Json;
using System.Text.Json.Serialization;

namespace CmlLib.Core.Internals;

internal static class JsonUtil
{
    public static JsonSerializerOptions JsonOptions { get; } = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static JsonElement? GetPropertyOrNull(this JsonElement jsonElement, string propertyName)
    {
        if (jsonElement.TryGetProperty(propertyName, out var val))
            return val;
        else
            return null;
    }

    public static string? GetPropertyValue(this JsonElement jsonElement, string propertyName)
    {
        return GetPropertyOrNull(jsonElement, propertyName)?.GetString(); 
    }
}
