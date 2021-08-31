using CmlLib.Core.Version;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CmlLib.Core.VersionLoader;

namespace CmlLib.Core.Installer.FabricMC
{
    public class FabricVersionLoader : IVersionLoader
    {
        public static readonly string ApiServer = "https://meta.fabricmc.net";
        private static readonly string LoaderUrl = ApiServer + "/v2/versions/loader";
        
        public string? LoaderVersion { get; set; }

        protected string GetVersionName(string version, string loaderVersion)
        {
            return $"fabric-loader-{loaderVersion}-{version}";
        }

        public MVersionCollection GetVersionMetadatas()
            => internalGetVersionMetadatasAsync(sync: true).GetAwaiter().GetResult();

        public Task<MVersionCollection> GetVersionMetadatasAsync()
            => internalGetVersionMetadatasAsync(sync: false);

        private async Task<MVersionCollection> internalGetVersionMetadatasAsync(bool sync)
        {
            if (string.IsNullOrEmpty(LoaderVersion))
            {
                FabricLoader[] loaders;
                if (sync)
                    loaders = GetFabricLoaders();
                else
                    loaders = await GetFabricLoadersAsync().ConfigureAwait(false);
                
                LoaderVersion = loaders[0].Version;
                if (loaders.Length == 0 || string.IsNullOrEmpty(LoaderVersion))
                    throw new KeyNotFoundException("can't find loaders");
            }

            string url = "https://meta.fabricmc.net/v2/versions/game/intermediary";
            string res;
            using (var wc = new WebClient())
            {
                if (sync)
                    res = wc.DownloadString(url);
                else
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
                var versionMetadata = new WebVersionMetadata(id)
                {
                    MType = MVersionType.Custom,
                    Path = jsonUrl,
                    Type = "fabric"
                };
                versionList.Add(versionMetadata);
            }

            return versionList;
        }

        public FabricLoader[] GetFabricLoaders()
        {
            using var wc = new WebClient();
            var res = wc.DownloadString(LoaderUrl);

            return parseLoaders(res);
        }

        public async Task<FabricLoader[]> GetFabricLoadersAsync()
        {
            using var wc = new WebClient();
            var res = await wc.DownloadStringTaskAsync(LoaderUrl)
                .ConfigureAwait(false);

            return parseLoaders(res);
        }

        private FabricLoader[] parseLoaders(string res)
        {
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
