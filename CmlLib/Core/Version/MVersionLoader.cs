using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace CmlLib.Core.Version
{
    public class MVersionLoader
    {
        public MVersionLoader(MinecraftPath path)
        {
            this.MinecraftPath = path;
        }

        public MinecraftPath MinecraftPath { get; private set; }

        /// <summary>
        /// Get All MVersionInfo from mojang server and local
        /// </summary>
        public MVersionCollection GetVersionMetadatas()
        {
            var list = getFromLocal();
            var web = GetVersionMetadatasFromWeb();
            foreach (var item in web)
            {
                if (!list.Contains(item))
                    list.Add(item);
            }
            return new MVersionCollection(list.ToArray(), MinecraftPath, web.LatestReleaseVersion, web.LatestSnapshotVersion);
        }

        /// <summary>
        /// Get All MVersionInfo from local
        /// </summary>
        public MVersionCollection GetVersionMetadatasFromLocal()
        {
            var list = getFromLocal().ToArray();
            return new MVersionCollection(list, MinecraftPath);
        }

        private List<MVersionMetadata> getFromLocal()
        {
            var dirs = new DirectoryInfo(MinecraftPath.Versions).GetDirectories();
            var arr = new List<MVersionMetadata>(dirs.Length);

            for (int i = 0; i < dirs.Length; i++)
            {
                var dir = dirs[i];
                var filepath = Path.Combine(dir.FullName, dir.Name + ".json");
                if (File.Exists(filepath))
                {
                    var info = new MVersionMetadata();
                    info.IsLocalVersion = true;
                    info.Name = dir.Name;
                    info.Path = filepath;
                    info.Type = "local";
                    info.MType = MVersionType.Custom;
                    arr.Add(info);
                }
            }

            return arr;
        }

        /// <summary>
        /// Get All MVersionInfo from mojang server
        /// </summary>
        public static MVersionCollection GetVersionMetadatasFromWeb()
        {
            string latestReleaseId = null;
            string latestSnapshotId = null;

            MVersionMetadata latestRelease = null;
            MVersionMetadata latestSnapshot = null;

            JArray jarr;
            using (WebClient wc = new WebClient())
            {
                var jobj = JObject.Parse(wc.DownloadString(MojangServer.Version));
                jarr = JArray.Parse(jobj["versions"].ToString());

                var latest = jobj["latest"];
                if (latest != null)
                {
                    latestReleaseId = latest["release"]?.ToString();
                    latestSnapshotId = latest["snapshot"]?.ToString();
                }
            }

            var checkLatestRelease = !string.IsNullOrEmpty(latestReleaseId);
            var checkLatestSnapshot = !string.IsNullOrEmpty(latestSnapshotId);

            var arr = new List<MVersionMetadata>(jarr.Count);
            for (int i = 0; i < jarr.Count; i++)
            {
                var obj = jarr[i].ToObject<MVersionMetadata>();
                obj.IsLocalVersion = false;
                obj.MType = MVersionTypeConverter.FromString(obj.Type);
                arr.Add(obj);

                if (checkLatestRelease && obj.Name == latestReleaseId)
                    latestRelease = obj;
                if (checkLatestSnapshot && obj.Name == latestSnapshotId)
                    latestSnapshot = obj;
            }

            return new MVersionCollection(arr.ToArray(), null, latestRelease, latestSnapshot);
        }
    }
}
