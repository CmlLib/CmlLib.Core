using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CmlLib.Core.Version
{
    public class MojangVersionLoader : IVersionLoader
    {
        public MVersionCollection GetVersionMetadatas()
        {
            using (var wc = new WebClient())
            {
                var res = wc.DownloadString(MojangServer.Version);
                return parseList(res);
            }
        }

        public async Task<MVersionCollection> GetVersionMetadatasAsync()
        {
            using (var wc = new WebClient())
            {
                var res = await wc.DownloadStringTaskAsync(MojangServer.Version);
                return parseList(res);
            }
        }

        private MVersionCollection parseList(string res)
        {
            string latestReleaseId = null;
            string latestSnapshotId = null;

            MVersionMetadata latestRelease = null;
            MVersionMetadata latestSnapshot = null;

            JArray jarr;
            var jobj = JObject.Parse(res);
            jarr = JArray.Parse(jobj["versions"].ToString());

            var latest = jobj["latest"];
            if (latest != null)
            {
                latestReleaseId = latest["release"]?.ToString();
                latestSnapshotId = latest["snapshot"]?.ToString();
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
