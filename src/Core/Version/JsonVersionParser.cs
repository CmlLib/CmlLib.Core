using CmlLib.Core.Files;
using CmlLib.Utils;
using System.Text.Json;

namespace CmlLib.Core.Version;

public class JsonVersionParser
{
    public MVersion ParseFromJsonString(string json)
    {
        using var document = JsonDocument.Parse(json);
        return ParseFromJson(document.RootElement);
    }

    public MVersion ParseFromJson(JsonElement element)
    {
        try
        {
            return parseInternal(element);
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

    private MVersion parseInternal(JsonElement root)
    {
        // id
        if (!root.TryGetProperty("id", out var idElement))
            throw new MVersionParseException("Empty version id");

        var id = idElement.GetString()
            ?? throw new MVersionParseException("Empty version id");

        var version = new MVersion(id);

        // javaVersion
        version.JavaVersion = root
            .GetPropertyOrNull("javaVersion")?
            .GetPropertyOrNull("component")?
            .GetString();

        // assets
        if (root.TryGetProperty("assetIndex", out var assetIndex))
            version.Assets = assetIndex.Deserialize<MFileMetadata>(JsonUtil.JsonOptions);
        else if (root.TryGetProperty("assets", out var assets))
            version.Assets = new MFileMetadata(assets.GetString());

        // client jar
        var client = root
            .GetPropertyOrNull("downloads")?
            .GetPropertyOrNull("client");

        if (client.HasValue)
            version.Client = client.Value.Deserialize<MFileMetadata>(JsonUtil.JsonOptions);

        // libraries
        if (root.TryGetProperty("libraries", out var libProp) && libProp.ValueKind == JsonValueKind.Array)
        {
            var libList = new List<MLibrary>();
            var libParser = new MLibraryParser();
            foreach (var item in libProp.EnumerateArray())
            {
                var libs = libParser.ParseJsonObject(item);
                if (libs != null)
                    libList.AddRange(libs);
            }

            version.Libraries = libList.ToArray();
        }

        // mainClass
        version.MainClass = root.GetPropertyValue("mainClass");

        // argument
        version.MinecraftArguments = root.GetPropertyValue("minecraftArguments");

        if (root.TryGetProperty("arguments", out var ag))
        {
            if (ag.TryGetProperty("game", out var gameArg) && gameArg.ValueKind == JsonValueKind.Array)
                version.GameArguments = argParse(gameArg);
            if (ag.TryGetProperty("jvm", out var jvmArg) && jvmArg.ValueKind == JsonValueKind.Array)
                version.JvmArguments = argParse(jvmArg);
        }

        // metadata
        version.ReleaseTime = root.GetPropertyValue("releaseTime");

        version.Type = root.GetPropertyValue("type");

        // inherits
        version.ParentVersionId = root.GetPropertyValue("inheritsFrom");
        version.IsInherited = string.IsNullOrEmpty(version.ParentVersionId);

        version.Jar = root.GetPropertyValue("jar");
        if (string.IsNullOrEmpty(version.Jar))
            version.Jar = version.Id;

        // logging
        var loggingClient = root
            .GetPropertyOrNull("logging")?
            .GetPropertyOrNull("client");

        if (loggingClient.HasValue)
        {
            var logFile = loggingClient.Value
                .GetPropertyOrNull("file")?
                .Deserialize<MFileMetadata>(JsonUtil.JsonOptions);

            version.LoggingClient = new MLogConfiguration
            {
                LogFile = logFile,
                Type = loggingClient.Value.GetPropertyValue("type"),
                Argument = loggingClient.Value.GetPropertyValue("argument")
            };
        }

        return version;
    }

    // TODO: create argument object
    private static string[] argParse(JsonElement arr)
    {
        var strList = new List<string>();

        foreach (var item in arr.EnumerateArray())
        {
            if (item.ValueKind == JsonValueKind.Object)
            {
                bool allow = true;

                var rules = item.GetPropertyOrNull("rules");
                if (rules == null || rules.Value.ValueKind != JsonValueKind.Array)
                    rules = item.GetPropertyOrNull("compatibilityRules");

                if (rules != null)
                    allow = MRule.CheckOSRequire(rules.Value);

                if (allow)
                {
                    var value = item.GetPropertyOrNull("value") ?? item.GetPropertyOrNull("values");
                    if (value != null)
                    {
                        if (value.Value.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var strProp in value.Value.EnumerateArray())
                            {
                                if (strProp.ValueKind != JsonValueKind.String)
                                    continue;

                                var str = strProp.GetString();
                                if (!string.IsNullOrEmpty(str))
                                    strList.Add(str);
                            }
                        }
                        else if (value.Value.ValueKind == JsonValueKind.String)
                        {
                            var valueString = value.Value.GetString();
                            if (!string.IsNullOrEmpty(valueString))
                                strList.Add(valueString);
                        }
                    }
                }
            }
            else
            {
                var value = item.GetString();
                if (!string.IsNullOrEmpty(value))
                    strList.Add(value);
            }
        }

        return strList.ToArray();
    }
}
