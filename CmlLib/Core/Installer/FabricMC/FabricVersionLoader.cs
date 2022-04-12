using CmlLib.Core.Version;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using CmlLib.Core.VersionLoader;
using CmlLib.Core.VersionMetadata;
using CmlLib.Utils;

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

        public async Task<MVersionCollection> GetVersionMetadatasAsync()
        {
            if (string.IsNullOrEmpty(LoaderVersion))
            {
                var loaders = await GetFabricLoadersAsync().ConfigureAwait(false);
                
                LoaderVersion = loaders[0].Version;
                if (loaders.Length == 0 || string.IsNullOrEmpty(LoaderVersion))
                    throw new KeyNotFoundException("can't find loaders");
            }

            var url = "https://meta.fabricmc.net/v2/versions/game/intermediary";
            var res = await HttpUtil.HttpClient.GetStringAsync(url)
                .ConfigureAwait(false);

            var versions = parseVersions(res, LoaderVersion!);
            return new MVersionCollection(versions.ToArray());
        }

        private List<MVersionMetadata> parseVersions(string res, string loader)
        {
            using var jsonDocument = JsonDocument.Parse(res);
            var root = jsonDocument.RootElement;
            var versionList = new List<MVersionMetadata>();

            foreach (var item in root.EnumerateArray())
            {
                var versionName = item.GetPropertyValue("version");
                if (string.IsNullOrEmpty(versionName))
                    continue;

                var jsonUrl = $"{ApiServer}/v2/versions/loader/{versionName}/{loader}/profile/json";

                var id = GetVersionName(versionName, loader);
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

        public async Task<FabricLoader[]> GetFabricLoadersAsync()
        {
            var res = await HttpUtil.HttpClient.GetStringAsync(LoaderUrl);
            return parseLoaders(res);
        }

        private FabricLoader[] parseLoaders(string res)
        {
            using var jsonDocument = JsonDocument.Parse(res);
            var loaderList = new List<FabricLoader>();
            foreach (var item in jsonDocument.RootElement.EnumerateArray())
            {
                var obj = item.Deserialize<FabricLoader>();
                if (obj != null)
                    loaderList.Add(obj);
            }

            return loaderList.ToArray();
        }
    }
}
