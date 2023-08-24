using System.Net;
using System.Text.Json;
using CmlLib.Core.VersionLoader;
using CmlLib.Core.VersionMetadata;

namespace CmlLib.Core.ModLoaders.QuiltMC;

public class QuiltVersionLoader : IVersionLoader
{
    public static readonly string ApiServer = "https://meta.quiltmc.org";
    private static readonly string LoaderUrl = ApiServer + "/v3/versions/loader";

    private readonly HttpClient _httpClient;

    public QuiltVersionLoader(HttpClient httpClient) => _httpClient = httpClient;

    public string? LoaderVersion { get; set; }

    protected string GetVersionName(string version, string loaderVersion)
    {
        return $"quilt-loader-{loaderVersion}-{version}";
    }

    public async ValueTask<VersionCollection> GetVersionMetadatasAsync()
    {
        if (string.IsNullOrEmpty(LoaderVersion))
        {
            var loaders = await GetQuiltLoadersAsync().ConfigureAwait(false);

            if (loaders.Length == 0 || string.IsNullOrEmpty(loaders[0].Version))
                throw new KeyNotFoundException("can't find loaders");
            
            LoaderVersion = loaders[0].Version;
        }

        var url = "https://meta.quiltmc.org/v3/versions/game";
        var res = await _httpClient.GetStringAsync(url);

        var versions = parseVersions(res, LoaderVersion!);
        return new VersionCollection(versions, null, null);
    }

    private IEnumerable<IVersionMetadata> parseVersions(string res, string loader)
    {
        using var json = JsonDocument.Parse(res);
        var jarr = json.RootElement.EnumerateArray();

        foreach (var item in jarr)
        {
            if (!item.TryGetProperty("version", out var versionProp))
                continue;
            var versionName = versionProp.GetString();
            if (string.IsNullOrEmpty(versionName))
                continue;

            string jsonUrl = $"{ApiServer}/v3/versions/loader/{versionName}/{loader}/profile/json";

            string id = GetVersionName(versionName, loader);
            var model = new JsonVersionMetadataModel
            {
                Type = "quilt",
                Url = jsonUrl,
            };
            yield return new MojangVersionMetadata(model, _httpClient);
        }
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
