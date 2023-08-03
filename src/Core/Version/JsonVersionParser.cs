using System.Text.Json;

namespace CmlLib.Core.Version;

public static class JsonVersionParser
{
    public static IVersion ParseFromJsonString(string json, JsonVersionParserOptions options)
    {
        var document = JsonDocument.Parse(json);
        return ParseFromJson(document.RootElement, options);
    }

    public static IVersion ParseFromJson(JsonElement element, JsonVersionParserOptions options)
    {
        try
        {
            return parseInternal(element, options);
        }
        catch (MVersionParseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new MVersionParseException(ex);
        }
    }

    private static IVersion parseInternal(JsonElement root, JsonVersionParserOptions options)
    {
        return new JsonVersion(root, options);
    }
}
