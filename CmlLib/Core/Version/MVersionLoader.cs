using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System;

namespace CmlLib.Core.Files
{
    public class MVersionLoader
    {
        /// <summary>
        /// Get All MVersionInfo from mojang server and local
        /// </summary>
        public MVersionCollection GetVersionMetadatas(MinecraftPath path)
        {
            var list = getFromLocal(path);
            var web = GetVersionMetadatasFromWeb();
            foreach (var item in web)
            {
                if (!list.Contains(item))
                    list.Add(item);
            }
            return new MVersionCollection(list.ToArray(), path, web.LatestReleaseVersion, web.LatestSnapshotVersion);
        }

        /// <summary>
        /// Get All MVersionInfo from local
        /// </summary>
        public MVersionCollection GetVersionMetadatasFromLocal(MinecraftPath path)
        {
            var list = getFromLocal(path).ToArray();
            return new MVersionCollection(list, path);
        }

        private List<MVersionMetadata> getFromLocal(MinecraftPath path)
        {
            var dirs = new DirectoryInfo(path.Versions).GetDirectories();
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
        public MVersionCollection GetVersionMetadatasFromWeb()
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
