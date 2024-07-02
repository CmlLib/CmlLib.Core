using System.Text.Json;

namespace CmlLib.Core.Version;

public static class JsonVersionParser
{
    public static IVersion ParseFromJsonString(string json, JsonVersionParserOptions options)
    {
        var document = JsonDocument.Parse(json);
        return ParseFromJson(document, options);
    }

    public static IVersion ParseFromJsonStream(Stream stream, JsonVersionParserOptions options)
    {
        var document = JsonDocument.Parse(stream);
        return ParseFromJson(document, options);
    }

    public static IVersion ParseFromJson(JsonDocument json, JsonVersionParserOptions options)
    {
        try
        {
            return new JsonVersion(json, options);
        }
        catch (VersionParseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new VersionParseException(ex);
        }
    }
}
