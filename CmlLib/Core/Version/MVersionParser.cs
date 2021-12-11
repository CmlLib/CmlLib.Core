using CmlLib.Core.Files;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

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
            try
            {
                var job = JObject.Parse(json);

                // id
                var id = job["id"]?.ToString();
                if (string.IsNullOrEmpty(id))
                    throw new MVersionParseException("Empty version id");

                var version = new MVersion(id);

                // javaVersion
                version.JavaVersion = job["javaVersion"]?["component"]?.ToString();
                
                // assets
                var assetindex = job["assetIndex"];
                var assets = job["assets"];
                if (assetindex != null)
                {
                    version.AssetId = assetindex["id"]?.ToString();
                    version.AssetUrl = assetindex["url"]?.ToString();
                    version.AssetHash = assetindex["sha1"]?.ToString();
                }
                else if (assets != null)
                    version.AssetId = assets.ToString();

                // client jar
                var client = job["downloads"]?["client"];
                if (client != null)
                {
                    version.ClientDownloadUrl = client["url"]?.ToString();
                    version.ClientHash = client["sha1"]?.ToString();
                }

                // libraries
                if (job["libraries"] is JArray libJArr)
                {
                    var libList = new List<MLibrary>(libJArr.Count);
                    var libParser = new MLibraryParser();
                    foreach (var item in libJArr)
                    {
                        var libs = libParser.ParseJsonObject((JObject) item);
                        if (libs != null)
                            libList.AddRange(libs);
                    }

                    version.Libraries = libList.ToArray();
                }

                // mainClass
                version.MainClass = job["mainClass"]?.ToString();

                // argument
                version.MinecraftArguments = job["minecraftArguments"]?.ToString();

                var ag = job["arguments"];
                if (ag != null)
                {
                    if (ag["game"] is JArray gameArg)
                        version.GameArguments = argParse(gameArg);
                    if (ag["jvm"] is JArray jvmArg)
                        version.JvmArguments = argParse(jvmArg);
                }

                // metadata
                version.ReleaseTime = job["releaseTime"]?.ToString();

                var type = job["type"]?.ToString();
                version.TypeStr = type;
                version.Type = MVersionTypeConverter.FromString(type);

                // inherits
                if (job["inheritsFrom"] != null)
                {
                    version.IsInherited = true;
                    version.ParentVersionId = job["inheritsFrom"]?.ToString();
                }

                version.Jar = job["jar"]?.ToString();
                if (string.IsNullOrEmpty(version.Jar))
                    version.Jar = version.Id;

                // logging
                var loggingClient = job["logging"]?["client"];
                if (loggingClient != null)
                {
                    version.LoggingClient = new MLogConfiguration
                    {
                        Id = loggingClient["file"]?["id"]?.ToString(),
                        Sha1 = loggingClient["file"]?["sha1"]?.ToString(),
                        Size = loggingClient["file"]?["size"]?.ToString(),
                        Url = loggingClient["file"]?["url"]?.ToString(),
                        Type = loggingClient["type"]?.ToString(),
                        Argument = loggingClient["argument"]?.ToString()
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
        private static string[] argParse(JArray arr)
        {
            var strList = new List<string>(arr.Count);

            foreach (var item in arr)
            {
                if (item is JObject)
                {
                    bool allow = true;

                    var rules = item["rules"] as JArray ?? item["compatibilityRules"] as JArray;
                    if (rules != null)
                        allow = MRule.CheckOSRequire(rules);

                    var value = item["value"] ?? item["values"];

                    if (allow && value != null)
                    {
                        if (value is JArray)
                        {
                            foreach (var str in value)
                            {
                                strList.Add(str.ToString());
                            }
                        }
                        else
                            strList.Add(value.ToString());
                    }
                }
                else
                    strList.Add(item.ToString());
            }

            return strList.ToArray();
        }
    }
}
