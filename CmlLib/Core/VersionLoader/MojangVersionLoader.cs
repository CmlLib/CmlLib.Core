using CmlLib.Core.Version;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace CmlLib.Core.VersionLoader
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

        public Task<MVersionCollection> GetVersionMetadatasAsync()
        {
            return Task.Run(GetVersionMetadatas);
        }

        [MethodTimer.Time]
        private MVersionCollection parseList(string res)
        {
            string latestReleaseId = null;
            string latestSnapshotId = null;

            MVersionMetadata latestRelease = null;
            MVersionMetadata latestSnapshot = null;
            
            var jobj = JObject.Parse(res);
            var jarr = jobj["versions"] as JArray;

            var latest = jobj["latest"];
            if (latest != null)
            {
                latestReleaseId = latest["release"]?.ToString();
                latestSnapshotId = latest["snapshot"]?.ToString();
            }

            bool checkLatestRelease = !string.IsNullOrEmpty(latestReleaseId);
            bool checkLatestSnapshot = !string.IsNullOrEmpty(latestSnapshotId);

            var arr = new List<MVersionMetadata>(jarr?.Count ?? 0);
            if (jarr != null)
            {
                foreach (var t in jarr)
                {
                    var obj = t.ToObject<MVersionMetadata>();
                    if (obj == null)
                        continue;
                    
                    obj.IsLocalVersion = false;
                    obj.MType = MVersionTypeConverter.FromString(obj.Type);
                    arr.Add(obj);

                    if (checkLatestRelease && obj.Name == latestReleaseId)
                        latestRelease = obj;
                    if (checkLatestSnapshot && obj.Name == latestSnapshotId)
                        latestSnapshot = obj;
                }
            }

            return new MVersionCollection(arr.ToArray(), null, latestRelease, latestSnapshot);
        }
    }
}
