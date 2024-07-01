using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;

namespace CmlLib.Core.Internals;

public class NumberToStringConverter : JsonConverter<object?>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(string) == typeToConvert;
    }
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString();
        }
        return Encoding.UTF8.GetString(reader.ValueSpan.ToArray());
    }

    public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value?.ToString());
    }
}
