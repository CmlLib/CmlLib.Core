using CmlLib.Core.Version;
using System.Net;
using System.Text.Json;
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
            using var json = JsonDocument.Parse(res);
            var jarr = json.RootElement.EnumerateArray();
            var versionList = new List<MVersionMetadata>();

            foreach (var item in jarr)
            {
                if (!item.TryGetProperty("version", out var versionProp))
                    continue;
                var versionName = versionProp.GetString();
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
            using var json = JsonDocument.Parse(res);
            var jarr = json.RootElement.EnumerateArray();
            var loaderList = new List<QuiltLoader>();
            foreach (var item in jarr)
            {
                var obj = item.Deserialize<QuiltLoader>();
                if (obj != null)
                    loaderList.Add(obj);
            }

            return loaderList.ToArray();
        }
    }
}
