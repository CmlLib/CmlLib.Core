using System.Text.Json;
using System.Text.Json.Serialization;

namespace CmlLib.Utils
{
    public static class JsonUtil
    {
        public static JsonSerializerOptions JsonOptions { get; } = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static JsonElement? SafeGetProperty(this JsonElement jsonElement, string propertyName)
        {
            if (jsonElement.TryGetProperty(propertyName, out var val))
                return val;
            else
                return null;
        }

        public static string? GetPropertyValue(this JsonElement jsonElement, string propertyName)
        {
            return SafeGetProperty(jsonElement, propertyName)?.GetString(); 
        }
    }
}
