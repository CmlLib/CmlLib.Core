using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace CmlLib.Core.Version
{
    public class MVersion
    {
        public static MVersion Parse(MVersionMetadata info)
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

        public static MVersion ParseFromFile(string path)
        {
            var json = File.ReadAllText(path);
            return ParseFromJson(json);
        }

        private static MVersion ParseFromJson(string json)
        {
            var version = new MVersion();
            var job = JObject.Parse(json);
            version.Id = job["id"]?.ToString();

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

            var client = job["downloads"]?["client"];
            if (client != null)
            {
                version.ClientDownloadUrl = client["url"]?.ToString();
                version.ClientHash = client["sha1"]?.ToString();
            }

            version.Libraries = MLibrary.Parser.ParseJson((JArray)job["libraries"]);
            version.MainClass = n(job["mainClass"]?.ToString());

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

            version.ReleaseTime = job["releaseTime"]?.ToString();

            var ype = job["type"]?.ToString();
            version.TypeStr = ype;
            version.Type = MVersionTypeConverter.FromString(ype);

            if (job["inheritsFrom"] != null)
            {
                version.IsInherited = true;
                version.ParentVersionId = job["inheritsFrom"].ToString();
            }
            else
                version.Jar = version.Id;

            //if (writeVersion)
            //{
            //    var path = Path.Combine(mc.Versions, version.Id);
            //    Directory.CreateDirectory(path);
            //    File.WriteAllText(Path.Combine(path, version.Id + ".json"), json);
            //}
            return version;
        }

        static string[] argParse(JArray arr)
        {
            var strList = new List<string>(arr.Count);
            var ruleChecker = new MRule();

            foreach (var item in arr)
            {
                if (item is JObject)
                {
                    bool allow = true;

                    if (item["rules"] != null)
                        allow = ruleChecker.CheckOSRequire((JArray)item["rules"]);

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

        public void InheritFrom(MVersion parentVersion)
        {
            // Inherit list
            // Overload : AssetId, AssetUrl, AssetHash, ClientDownloadUrl, ClientHash, MainClass, MinecraftArguments
            // Combine : Libraries, GameArguments, JvmArguments

            // Overloads

            if (nc(AssetId))
                AssetId = parentVersion.AssetId;

            if (nc(AssetUrl))
                AssetUrl = parentVersion.AssetUrl;

            if (nc(AssetHash))
                AssetHash = parentVersion.AssetHash;

            if (nc(ClientDownloadUrl))
                ClientDownloadUrl = parentVersion.ClientDownloadUrl;

            if (nc(ClientHash))
                ClientHash = parentVersion.ClientHash;

            if (nc(MainClass))
                MainClass = parentVersion.MainClass;

            if (nc(MinecraftArguments))
                MinecraftArguments = parentVersion.MinecraftArguments;

            Jar = parentVersion.Jar;

            // Combine

            if (parentVersion.Libraries != null)
            {
                if (Libraries != null)
                    Libraries = Libraries.Concat(parentVersion.Libraries).ToArray();
                else
                    Libraries = parentVersion.Libraries;
            }

            if (parentVersion.GameArguments != null)
            {
                if (GameArguments != null)
                    GameArguments = GameArguments.Concat(parentVersion.GameArguments).ToArray();
                else
                    GameArguments = parentVersion.GameArguments;
            }


            if (parentVersion.JvmArguments != null)
            {
                if (JvmArguments != null)
                    JvmArguments = JvmArguments.Concat(parentVersion.JvmArguments).ToArray();
                else
                    JvmArguments = parentVersion.JvmArguments;
            }
        }

        static string n(string t) // handle null string
        {
            return t == null ? "" : t;
        }

        static bool nc(string t) // check null string
        {
            return t == null || t == "";
        }

        public bool IsWeb { get; private set; }

        public bool IsInherited { get; private set; } = false;
        public string ParentVersionId { get; private set; } = "";

        public string Id { get; private set; } = "";

        public string AssetId { get; private set; } = "";
        public string AssetUrl { get; private set; } = "";
        public string AssetHash { get; private set; } = "";

        public string Jar { get; private set; } = "";
        public string ClientDownloadUrl { get; private set; } = "";
        public string ClientHash { get; private set; } = "";
        public MLibrary[] Libraries { get; private set; }
        public string MainClass { get; private set; } = "";
        public string MinecraftArguments { get; private set; } = "";
        public string[] GameArguments { get; private set; }
        public string[] JvmArguments { get; private set; }
        public string ReleaseTime { get; private set; } = "";
        public MVersionType Type { get; private set; } = MVersionType.Custom;

        public string TypeStr { get; private set; } = "";

        public string NativePath { get; set; } = "";
    }
}
