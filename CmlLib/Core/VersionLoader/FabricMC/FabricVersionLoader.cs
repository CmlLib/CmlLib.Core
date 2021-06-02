using CmlLib.Core.Version;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace CmlLib.Core.VersionLoader.FabricMC
{
    public class FabricVersionLoader : IVersionLoader
    {
        public string ApiServer { get; } = "https://meta.fabricmc.net";
        public string? LoaderVersion { get; set; }

        protected string GetVersionName(string version, string loaderVersion)
        {
            return $"fabric-loader-{loaderVersion}-{version}";
        }

        public MVersionCollection GetVersionMetadatas()
        {
            if (string.IsNullOrEmpty(LoaderVersion))
            {
                var loaders = GetFabricLoaders().GetAwaiter().GetResult();
                LoaderVersion = loaders[0].Version;
                if (loaders.Length == 0 || string.IsNullOrEmpty(LoaderVersion))
                    throw new KeyNotFoundException("can't find loaders");
            }

            string url = "https://meta.fabricmc.net/v2/versions/game/intermediary";
            string res;
            using (var wc = new WebClient())
            {
                res = wc.DownloadString(url);
            }

            var versions = parseVersions(res, LoaderVersion);
            return new MVersionCollection(versions.ToArray());
        }

        public async Task<MVersionCollection> GetVersionMetadatasAsync()
        {
            if (string.IsNullOrEmpty(LoaderVersion))
            {
                var loaders = GetFabricLoaders().GetAwaiter().GetResult();
                LoaderVersion = loaders[0].Version;
                if (loaders.Length == 0 || string.IsNullOrEmpty(LoaderVersion))
                    throw new KeyNotFoundException("can't find loaders");
            }

            string url = "https://meta.fabricmc.net/v2/versions/game/intermediary";
            string res;
            using (var wc = new WebClient())
            {
                res = await wc.DownloadStringTaskAsync(url).ConfigureAwait(false);
            }

            var versions = parseVersions(res, LoaderVersion!);
            return new MVersionCollection(versions.ToArray());
        }

        private List<MVersionMetadata> parseVersions(string res, string loader)
        {
            var jarr = JArray.Parse(res);
            var versionList = new List<MVersionMetadata>(jarr.Count);

            foreach (var item in jarr)
            {
                string? versionName = item["version"]?.ToString();
                if (string.IsNullOrEmpty(versionName))
                    continue;

                string jsonUrl = $"{ApiServer}/v2/versions/loader/{versionName}/{loader}/profile/json";

                string id = GetVersionName(versionName, loader);
                var versionMetadata = new MVersionMetadata(id)
                {
                    IsLocalVersion = false,
                    MType = MVersionType.Custom,
                    Path = jsonUrl,
                    Type = "fabric"
                };
                versionList.Add(versionMetadata);
            }

            return versionList;
        }

        public async Task<FabricLoader[]> GetFabricLoaders()
        {
            string res;
            using (var wc = new WebClient())
            {
                res = await wc.DownloadStringTaskAsync(ApiServer + "/v2/versions/loader")
                    .ConfigureAwait(false);
            }

            var jarr = JArray.Parse(res);
            var loaderList = new List<FabricLoader>(jarr.Count);
            foreach (var item in jarr)
            {
                var obj = item.ToObject<FabricLoader>();
                if (obj != null)
                    loaderList.Add(obj);
            }

            return loaderList.ToArray();
        }
    }
}
