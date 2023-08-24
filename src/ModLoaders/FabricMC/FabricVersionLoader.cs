using System.Text.Json;
using CmlLib.Core.VersionLoader;
using CmlLib.Core.VersionMetadata;
using CmlLib.Core.Internals;

namespace CmlLib.Core.ModLoaders.FabricMC;

public class FabricVersionLoader : IVersionLoader
{
    public static readonly string ApiServer = "https://meta.fabricmc.net";
    private static readonly string LoaderUrl = ApiServer + "/v2/versions/loader";
    
    private readonly HttpClient _httpClient;

    public FabricVersionLoader(HttpClient httpClient) => 
        _httpClient = httpClient;

    public string? LoaderVersion { get; set; }

    protected string GetVersionName(string version, string loaderVersion)
    {
        return $"fabric-loader-{loaderVersion}-{version}";
    }

    public async ValueTask<VersionCollection> GetVersionMetadatasAsync()
    {
        if (string.IsNullOrEmpty(LoaderVersion))
        {
            var loaders = await GetFabricLoadersAsync().ConfigureAwait(false);
            
            if (loaders.Length == 0 || string.IsNullOrEmpty(loaders[0].Version))
                throw new KeyNotFoundException("can't find loaders");
            
            LoaderVersion = loaders[0].Version;
        }

        var url = "https://meta.fabricmc.net/v2/versions/game/intermediary";
        var res = await _httpClient.GetStringAsync(url)
            .ConfigureAwait(false);

        var versions = parseVersions(res, LoaderVersion!);
        return new VersionCollection(versions, null, null);
    }

    private IEnumerable<IVersionMetadata> parseVersions(string res, string loader)
    {
        using var jsonDocument = JsonDocument.Parse(res);
        var root = jsonDocument.RootElement;

        foreach (var item in root.EnumerateArray())
        {
            var versionName = item.GetPropertyValue("version");
            if (string.IsNullOrEmpty(versionName))
                continue;

            var jsonUrl = $"{ApiServer}/v2/versions/loader/{versionName}/{loader}/profile/json";

            var id = GetVersionName(versionName, loader);
            var model = new JsonVersionMetadataModel
            {
                Name = id,
                Url = jsonUrl,
                Type = "fabric"
            };
            yield return new MojangVersionMetadata(model, _httpClient);
        }
    }

    public async Task<FabricLoader[]> GetFabricLoadersAsync()
    {
        var res = await _httpClient.GetStringAsync(LoaderUrl);
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
