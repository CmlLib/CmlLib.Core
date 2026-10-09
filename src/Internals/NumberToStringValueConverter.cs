using System.Text.Json;
using System.Text.Json.Serialization;

namespace CmlLib.Core.Internals;

// Source-generated string properties require a converter with the matching value type.
internal sealed class NumberToStringValueConverter : JsonConverter<string?>
{
    private static readonly NumberToStringConverter Converter = new();

    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => (string?)Converter.Read(ref reader, typeToConvert, options);

    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
        => Converter.Write(writer, value, options);
}
