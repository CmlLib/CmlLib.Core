using CmlLib.Core.Files;
using CmlLib.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CmlLib.Core.Version
{
    public static class MVersionParser
    {
        public static MVersion ParseFromFile(string path)
        {
            string json = File.ReadAllText(path);
            return ParseFromJson(json);
        }

        public static MVersion ParseFromJson(string json)
        {
            using var jsonDocument = JsonDocument.Parse(json);
            return ParseFromJson(jsonDocument);
        }
        
        [MethodTimer.Time]
        public static MVersion ParseFromJson(JsonDocument document)
        {
            try
            {
                var root = document.RootElement;
                
                // id
                if (!root.TryGetProperty("id", out var idElement))
                    throw new MVersionParseException("Empty version id");

                var id = idElement.GetString()
                    ?? throw new MVersionParseException("Empty version id");

                var version = new MVersion(id);

                // javaVersion
                version.JavaVersion = root.SafeGetProperty("javaVersion")?.SafeGetProperty("component")?.GetString();

                // assets
                if (root.TryGetProperty("assetIndex", out var assetIndex))
                    version.Assets = assetIndex.Deserialize<MFileMetadata>(JsonUtil.JsonOptions);
                else if (root.TryGetProperty("assets", out var assets))
                    version.Assets = new MFileMetadata(assets.GetString());

                // client jar
                var client = root.SafeGetProperty("downloads")?.SafeGetProperty("client");
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
                version.ReleaseTime = root.GetPropertyValue("releaseTime");;

                var type = root.GetPropertyValue("type");
                version.TypeStr = type;
                version.Type = MVersionTypeConverter.FromString(type);

                // inherits
                version.ParentVersionId = root.GetPropertyValue("inheritsFrom");
                version.IsInherited = string.IsNullOrEmpty(version.ParentVersionId);

                version.Jar = root.GetPropertyValue("jar");
                if (string.IsNullOrEmpty(version.Jar))
                    version.Jar = version.Id;

                // logging
                var loggingClient = root.SafeGetProperty("logging")?.SafeGetProperty("client");
                if (loggingClient != null)
                {
                    var logFile = loggingClient.Value.SafeGetProperty("file")?.Deserialize<MFileMetadata>(JsonUtil.JsonOptions);
                    version.LoggingClient = new MLogConfiguration
                    {
                        LogFile = logFile,   
                        Type = loggingClient.Value.GetPropertyValue("type"),
                        Argument = loggingClient.Value.GetPropertyValue("argument")
                    };
                }

                return version;
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

        // TODO: create argument object
        private static string[] argParse(JsonElement arr)
        {
            var strList = new List<string>();

            foreach (var item in arr.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.Object)
                {
                    bool allow = true;

                    var rules = item.SafeGetProperty("rules");
                    if (rules == null || rules.Value.ValueKind != JsonValueKind.Array)
                        rules = item.SafeGetProperty("compatibilityRules");

                    if (rules != null)
                        allow = MRule.CheckOSRequire(rules.Value);

                    if (allow)
                    {
                        var value = item.SafeGetProperty("value") ?? item.SafeGetProperty("values");
                        if (value != null)
                        {
                            if (value.Value.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var strProp in value.Value.EnumerateArray())
                                {
                                    var str = strProp.GetString();
                                    if (!string.IsNullOrEmpty(str))
                                        strList.Add(str);
                                }
                            }
                            else
                                strList.Add(value.ToString());
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
}
