using System.Text.Json;
using System.Text.Json.Serialization;

namespace CmlLib.Core.Internals;

// 공개 DTO를 소비하는 외부 어셈블리의 source generator에서도 접근할 수 있어야 한다.
public sealed class NumberToStringValueConverter : JsonConverter<string?>
{
    private static readonly NumberToStringConverter Converter = new();

    public override string? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        return (string?)Converter.Read(ref reader, typeToConvert, options);
    }

    public override void Write(
        Utf8JsonWriter writer,
        string? value,
        JsonSerializerOptions options)
    {
        Converter.Write(writer, value, options);
    }
}
