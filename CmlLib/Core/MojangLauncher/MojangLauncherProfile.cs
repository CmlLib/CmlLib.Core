using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json.Linq;
using CmlLib.Core.Auth;

namespace CmlLib.Core.MojangLauncher
{
    public class MojangLauncherProfile
    {
        private MojangLauncherProfile() { }

        public static MojangLauncherProfile LoadFromDefaultPath()
        {
            return LoadFromFile(Path.Combine(MinecraftPath.GetOSDefaultPath(), "launcher_profiles.json"));
        }

        public static MojangLauncherProfile LoadFromFile(string profilePath)
        {
            var json = File.ReadAllText(profilePath);
            return Load(json);
        }

        public static MojangLauncherProfile Load(string json)
        {
            var job = JObject.Parse(json);

            var profile = new MojangLauncherProfile();

            profile.LauncherVersion = job["launcherVersion"]?["profilesFormat"]?.ToString();

            var clientToken = job["clientToken"]?.ToString();
            profile.ClientToken = clientToken;

            var auths = job["authenticationDatabase"];
            var sessionList = new List<MSession>();
            foreach (var item in auths)
            {
                var innerObj = item.Children().First();

                var session = new MSession();
                session.AccessToken = innerObj["accessToken"]?.ToString();
                session.ClientToken = clientToken;

                var profiles = innerObj["profiles"] as JObject;
                var firstProfileProperty = profiles?.Properties()?.First();

                if (firstProfileProperty != null)
                {
                    session.UUID = firstProfileProperty.Name;
                    session.Username = firstProfileProperty.Value["displayName"]?.ToString();
                }

                sessionList.Add(session);
            }

            profile.Sessions = sessionList.ToArray();
            return profile;
        }

        public string LauncherVersion { get; private set; }
        public string ClientToken { get; private set; }
        public MSession[] Sessions { get; private set; }
    }
}
