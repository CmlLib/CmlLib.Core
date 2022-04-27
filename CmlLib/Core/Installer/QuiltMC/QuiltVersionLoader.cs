using CmlLib.Core.Version;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CmlLib.Core.VersionLoader;
using CmlLib.Core.VersionMetadata;

namespace CmlLib.Core.Installer.QuiltMC
{
    public class QuiltVersionLoader : IVersionLoader
    {
        public static readonly string ApiServer = "https://meta.quiltmc.org";
        private static readonly string LoaderUrl = ApiServer + "/v3/versions/loader";

        public string? LoaderVersion { get; set; }

        protected string GetVersionName(string version, string loaderVersion)
        {
            return $"quilt-loader-{loaderVersion}-{version}";
        }

        public MVersionCollection GetVersionMetadatas()
            => internalGetVersionMetadatasAsync(sync: true).GetAwaiter().GetResult();

        public Task<MVersionCollection> GetVersionMetadatasAsync()
            => internalGetVersionMetadatasAsync(sync: false);

        private async Task<MVersionCollection> internalGetVersionMetadatasAsync(bool sync)
        {
            if (string.IsNullOrEmpty(LoaderVersion))
            {
                QuiltLoader[] loaders;
                if (sync)
                    loaders = GetQuiltLoaders();
                else
                    loaders = await GetQuiltLoadersAsync().ConfigureAwait(false);

                if (loaders.Length == 0 || string.IsNullOrEmpty(loaders[0].Version))
                    throw new KeyNotFoundException("can't find loaders");
                
                LoaderVersion = loaders[0].Version;
            }

            string url = "https://meta.quiltmc.org/v3/versions/game";
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

                string jsonUrl = $"{ApiServer}/v3/versions/loader/{versionName}/{loader}/profile/json";

                string id = GetVersionName(versionName, loader);
                var versionMetadata = new WebVersionMetadata(id)
                {
                    MType = MVersionType.Custom,
                    Path = jsonUrl,
                    Type = "quilt"
                };
                versionList.Add(versionMetadata);
            }

            return versionList;
        }

        public QuiltLoader[] GetQuiltLoaders()
        {
            using var wc = new WebClient();
            var res = wc.DownloadString(LoaderUrl);

            return parseLoaders(res);
        }

        public async Task<QuiltLoader[]> GetQuiltLoadersAsync()
        {
            using var wc = new WebClient();
            var res = await wc.DownloadStringTaskAsync(LoaderUrl)
                .ConfigureAwait(false);

            return parseLoaders(res);
        }

        private QuiltLoader[] parseLoaders(string res)
        {
            var jarr = JArray.Parse(res);
            var loaderList = new List<QuiltLoader>(jarr.Count);
            foreach (var item in jarr)
            {
                var obj = item.ToObject<QuiltLoader>();
                if (obj != null)
                    loaderList.Add(obj);
            }

            return loaderList.ToArray();
        }
    }
}
