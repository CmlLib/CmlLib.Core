using CmlLib.Core.Version;
using System.Collections.Generic;
using System.Threading.Tasks;
using CmlLib.Core.VersionMetadata;
using CmlLib.Utils;
using System.Text.Json;

namespace CmlLib.Core.VersionLoader
{
    public class MojangVersionLoader : IVersionLoader
    {
        public async Task<MVersionCollection> GetVersionMetadatasAsync()
        {
            var res = await HttpUtil.HttpClient.GetStringAsync(MojangServer.Version);
            return parseList(res);
        }

        private MVersionCollection parseList(string res)
        {
            string? latestReleaseId = null;
            string? latestSnapshotId = null;

            MVersionMetadata? latestRelease = null;
            MVersionMetadata? latestSnapshot = null;

            using var jsonDocument = JsonDocument.Parse(res);
            var root = jsonDocument.RootElement;

            if (root.TryGetProperty("latest", out var latest))
            {
                latestReleaseId = latest.GetPropertyValue("release");
                latestSnapshotId = latest.GetPropertyValue("snapshot");
            }

            bool checkLatestRelease = !string.IsNullOrEmpty(latestReleaseId);
            bool checkLatestSnapshot = !string.IsNullOrEmpty(latestSnapshotId);

            var arr = new List<WebVersionMetadata>();
            if (root.TryGetProperty("versions", out var versions) && versions.ValueKind == JsonValueKind.Array)
            {
                foreach (var t in versions.EnumerateArray())
                {
                    var obj = t.Deserialize<WebVersionMetadata>();
                    if (obj == null)
                        continue;
                    
                    obj.MType = MVersionTypeConverter.FromString(obj.Type);
                    arr.Add(obj);

                    if (checkLatestRelease && obj.Name == latestReleaseId)
                        latestRelease = obj;
                    if (checkLatestSnapshot && obj.Name == latestSnapshotId)
                        latestSnapshot = obj;
                }
            }

            return new MVersionCollection(arr, null, latestRelease, latestSnapshot);
        }
    }
}
