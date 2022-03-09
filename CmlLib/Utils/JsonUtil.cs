using System.Text.Json;
using System.Text.Json.Serialization;

namespace CmlLib.Utils
{
    public class JsonUtil
    {
        public static JsonSerializerOptions JsonOptions { get; } = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }
}
