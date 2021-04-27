using CmlLib.Core.Files;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace CmlLib.Core.Version
{
    public static class MVersionParser
    {
        public static MVersion Parse(MVersionMetadata info)
        {
            try
            {
                string json;
                if (!info.IsLocalVersion)
                {
                    using (var wc = new WebClient())
                    {
                        json = wc.DownloadString(info.Path);
                        return ParseFromJson(json);
                    }
                }
                else
                    return ParseFromFile(info.Path);
            }
            catch (MVersionParseException ex)
            {
                ex.VersionName = info.Name;
                throw;
            }
        }

        public static MVersion ParseAndSave(MVersionMetadata info, MinecraftPath savePath)
        {
            try
            {
                string json;
                if (!info.IsLocalVersion)
                {
                    using (var wc = new WebClient())
                    {
                        json = wc.DownloadString(info.Path);
                        string path = savePath.GetVersionJsonPath(info.Name);
                        var directoryName = Path.GetDirectoryName(path);
                        if (!string.IsNullOrEmpty((directoryName)))
                            Directory.CreateDirectory(directoryName);
                        File.WriteAllText(path, json);

                        return ParseFromJson(json);
                    }
                }
                else
                    return ParseFromFile(info.Path);
            }
            catch (MVersionParseException ex)
            {
                ex.VersionName = info.Name;
                throw;
            }
        }

        public static MVersion ParseFromFile(string path)
        {
            string json = File.ReadAllText(path);
            return ParseFromJson(json);
        }

        public static MVersion ParseFromJson(string json)
        {
            try
            {
                var version = new MVersion();
                var job = JObject.Parse(json);

                // id
                version.Id = job["id"]?.ToString();
                if (string.IsNullOrEmpty(version.Id))
                    throw new MVersionParseException("Empty version id");

                // assets
                var assetindex = (JObject)job["assetIndex"];
                var assets = job["assets"];
                if (assetindex != null)
                {
                    version.AssetId = n(assetindex["id"]?.ToString());
                    version.AssetUrl = n(assetindex["url"]?.ToString());
                    version.AssetHash = n(assetindex["sha1"]?.ToString());
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
                var libJArr = job["libraries"] as JArray;
                if (libJArr != null)
                {
                var libList = new List<MLibrary>(libJArr.Count);
                var libParser = new MLibraryParser();
                foreach (var item in libJArr)
                {
                    var libs = libParser.ParseJsonObject((JObject)item);
                    if (libs != null)
                        libList.AddRange(libs);
                }
                version.Libraries = libList.ToArray();
                }

                // mainClass
                version.MainClass = n(job["mainClass"]?.ToString());

                // argument
                var ma = job["minecraftArguments"]?.ToString();
                if (ma != null)
                    version.MinecraftArguments = ma;

                var ag = job["arguments"];
                if (ag != null)
                {
                    if (ag["game"] != null)
                        version.GameArguments = argParse((JArray)ag["game"]);
                    if (ag["jvm"] != null)
                        version.JvmArguments = argParse((JArray)ag["jvm"]);
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
                    version.ParentVersionId = job["inheritsFrom"].ToString();
                }
                else
                    version.Jar = version.Id;

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

        private static string[] argParse(JArray arr)
        {
            var strList = new List<string>(arr.Count);

            foreach (var item in arr)
            {
                if (item is JObject)
                {
                    bool allow = true;

                    if (item["rules"] != null)
                        allow = MRule.CheckOSRequire((JArray)item["rules"]);

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

        private static string n(string t) // handle null string
        {
            return t ?? "";
        }
    }
}
