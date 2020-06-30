using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace CmlLib.Core
{
    public class MProfile
    {
        public static MProfile FindProfile(Minecraft mc, MProfileMetadata[] infos, string name)
        {
            MProfile startProfile = null;
            MProfile baseProfile = null;

            foreach (var item in infos)
            {
                if (item.Name == name)
                {
                    startProfile = Parse(mc, item);
                    break;
                }
            }

            if (startProfile == null)
                throw new Exception(name + " not found");

            if (startProfile.IsInherited)
            {
                if (startProfile.ParentProfileId == startProfile.Id) // prevent StackOverFlowException
                    throw new IOException("Invalid Profile : inheritFrom property is equal to id property.");

                baseProfile = FindProfile(mc, infos, startProfile.ParentProfileId);
                startProfile.InheritFrom(baseProfile);
            }

            return startProfile;
        }

        public static MProfile Parse(Minecraft mc, MProfileMetadata info)
        {
            string json;
            if (info.IsWeb)
            {
                using (var wc = new WebClient())
                {
                    json = wc.DownloadString(info.Path);
                    return ParseFromJson(mc, json, true);
                }
            }
            else
                return ParseFromFile(mc, info.Path);
        }

        public static MProfile ParseFromFile(Minecraft mc, string path)
        {
            var json = File.ReadAllText(path);
            return ParseFromJson(mc, json, false);
        }

        private static MProfile ParseFromJson(Minecraft mc, string json, bool writeProfile = true)
        {
            var profile = new MProfile();
            var job = JObject.Parse(json);
            profile.Id = job["id"]?.ToString();

            var assetindex = (JObject)job["assetIndex"];
            var assets = job["assets"];
            if (assetindex != null)
            {
                profile.AssetId = n(assetindex["id"]?.ToString());
                profile.AssetUrl = n(assetindex["url"]?.ToString());
                profile.AssetHash = n(assetindex["sha1"]?.ToString());
            }
            else if (assets != null)
                profile.AssetId = assets.ToString();

            var client = job["downloads"]?["client"];
            if (client != null)
            {
                profile.ClientDownloadUrl = client["url"]?.ToString();
                profile.ClientHash = client["sha1"]?.ToString();
            }

            profile.Libraries = MLibrary.Parser.ParseJson(mc.Library, (JArray)job["libraries"]);
            profile.MainClass = n(job["mainClass"]?.ToString());

            var ma = job["minecraftArguments"]?.ToString();
            if (ma != null)
                profile.MinecraftArguments = ma;

            var ag = job["arguments"];
            if (ag != null)
            {
                if (ag["game"] != null)
                    profile.GameArguments = argParse((JArray)ag["game"]);
                if (ag["jvm"] != null)
                    profile.JvmArguments = argParse((JArray)ag["jvm"]);
            }

            profile.ReleaseTime = job["releaseTime"]?.ToString();

            var ype = job["type"]?.ToString();
            profile.TypeStr = ype;
            profile.Type = MProfileTypeConverter.FromString(ype);

            if (job["inheritsFrom"] != null)
            {
                profile.IsInherited = true;
                profile.ParentProfileId = job["inheritsFrom"].ToString();
            }
            else
                profile.Jar = profile.Id;

            if (writeProfile)
            {
                var path = Path.Combine(mc.Versions, profile.Id);
                Directory.CreateDirectory(path);
                File.WriteAllText(Path.Combine(path, profile.Id + ".json"), json);
            }

            profile.Minecraft = mc;
            return profile;
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

        public void InheritFrom(MProfile parentProfile)
        {
            // Inherit list
            // Overload : AssetId, AssetUrl, AssetHash, ClientDownloadUrl, ClientHash, MainClass, MinecraftArguments
            // Combine : Libraries, GameArguments, JvmArguments

            // Overloads

            if (nc(this.AssetId))
                this.AssetId = parentProfile.AssetId;

            if (nc(this.AssetUrl))
                this.AssetUrl = parentProfile.AssetUrl;

            if (nc(this.AssetHash))
                this.AssetHash = parentProfile.AssetHash;

            if (nc(this.ClientDownloadUrl))
                this.ClientDownloadUrl = parentProfile.ClientDownloadUrl;

            if (nc(this.ClientHash))
                this.ClientHash = parentProfile.ClientHash;

            if (nc(this.MainClass))
                this.MainClass = parentProfile.MainClass;

            if (nc(this.MinecraftArguments))
                this.MinecraftArguments = parentProfile.MinecraftArguments;

            this.Jar = parentProfile.Jar;

            // Combine

            if (parentProfile.Libraries != null)
            {
                if (this.Libraries != null)
                    this.Libraries = this.Libraries.Concat(parentProfile.Libraries).ToArray();
                else
                    this.Libraries = parentProfile.Libraries;
            }

            if (parentProfile.GameArguments != null)
            {
                if (this.GameArguments != null)
                    this.GameArguments = this.GameArguments.Concat(parentProfile.GameArguments).ToArray();
                else
                    this.GameArguments = parentProfile.GameArguments;
            }


            if (parentProfile.JvmArguments != null)
            {
                if (this.JvmArguments != null)
                    this.JvmArguments = this.JvmArguments.Concat(parentProfile.JvmArguments).ToArray();
                else
                    this.JvmArguments = parentProfile.JvmArguments;
            }
        }

        static string n(string t) // handle null string
        {
            return (t == null) ? "" : t;
        }

        static bool nc(string t) // check null string
        {
            return (t == null) || (t == "");
        }

        public Minecraft Minecraft { get; private set; }
        public bool IsWeb { get; private set; }

        public bool IsInherited { get; private set; } = false;
        public string ParentProfileId { get; private set; } = "";

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
        public MProfileType Type { get; private set; } = MProfileType.Custom;

        public string TypeStr { get; private set; } = "";

        public string NativePath { get; set; } = "";
    }
}
