using System.Text.Json;
using CmlLib.Core.Internals;

namespace CmlLib.Core.ModLoaders.QuiltMC;

public class QuiltInstaller
{
    public static readonly string DefaultApiServerHost = "https://meta.quiltmc.org";
    private readonly HttpClient _httpClient;
    private readonly string _host;

    public QuiltInstaller(HttpClient httpClient) =>
        (_httpClient, _host) = (httpClient, DefaultApiServerHost);

    public QuiltInstaller(HttpClient httpClient, string host) =>
        (_httpClient, _host) = (httpClient, host);

    public static string GetVersionName(string gameVersion, string loaderVersion)
    {
        return $"quilt-loader-{loaderVersion}-{gameVersion}";
    }

    public async Task<IReadOnlyCollection<string>> GetSupportedVersionNames()
    {
        using var res = await _httpClient.GetStreamAsync($"{_host}/v3/versions/game");
        var list = await JsonSerializer.DeserializeAsync<IEnumerable<QuiltLoader>>(res);
        if (list == null)
            return Array.Empty<string>();

        return list
            .Where(item => item.Stable && !string.IsNullOrEmpty(item.Version))
            .Select(item => item.Version!)
            .ToList();
    }

    public async Task<IReadOnlyCollection<QuiltLoader>> GetLoaders()
    {
        using var res = await _httpClient.GetStreamAsync($"{_host}/v3/versions/loader");
        var list = await JsonSerializer.DeserializeAsync<IReadOnlyCollection<QuiltLoader>>(res);
        if (list == null)
            return Array.Empty<QuiltLoader>();
        else
            return list;
    }

    public async Task<IReadOnlyCollection<QuiltLoader>> GetLoaders(string gameVersion)
    {
        using var res = await _httpClient.GetStreamAsync($"{_host}/v3/versions/loader/{gameVersion}");
        using var json = await JsonDocument.ParseAsync(res);
        return parseLoaders(json.RootElement).ToList();
    }

    public async Task<QuiltLoader?> GetFirstLoader(string gameVersion)
    {
        using var res = await _httpClient.GetStreamAsync($"{_host}/v3/versions/loader/{gameVersion}");
        using var json = await JsonDocument.ParseAsync(res);
        return parseLoaders(json.RootElement).FirstOrDefault();
    }

    public async Task<Stream> GetProfileJson(string gameVersion, string loaderVersion)
    {
        return await _httpClient.GetStreamAsync($"{DefaultApiServerHost}/v3/versions/loader/{gameVersion}/{loaderVersion}/profile/json");
    }

    private IEnumerable<QuiltLoader> parseLoaders(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Array)
            Enumerable.Empty<QuiltLoader>();

        foreach (var item in root.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object)
                continue;
            var loader = item.GetPropertyOrNull("loader")?.Deserialize<QuiltLoader>();
            if (loader != null)
                yield return loader;
        }
    }

    public async Task<string> Install(string gameVersion, MinecraftPath installTo)
    {
        var loader = await GetFirstLoader(gameVersion);
        if (string.IsNullOrEmpty(loader?.Version))
            throw new KeyNotFoundException("Cannot find any loader for " + gameVersion);

        var versionName = GetVersionName(gameVersion, loader.Version);
        return await Install(gameVersion, loader.Version, installTo, versionName);
    }

    public async Task<string> Install(string gameVersion, MinecraftPath installTo, string versionName)
    {
        var loader = await GetFirstLoader(gameVersion);
        if (string.IsNullOrEmpty(loader?.Version))
            throw new KeyNotFoundException("Cannot find any loader for " + gameVersion);

        return await Install(gameVersion, loader.Version, installTo, versionName);
    }

    public async Task<string> Install(string gameVersion, string loaderVersion, MinecraftPath installTo)
    {
        var versionName = GetVersionName(gameVersion, loaderVersion);
        return await Install(gameVersion, loaderVersion, installTo, versionName);
    }

    public async Task<string> Install(string gameVersion, string loaderVersion, MinecraftPath installTo, string versionName)
    {
        using var profileStream = await GetProfileJson(gameVersion, loaderVersion);
        return await installProfile(profileStream, installTo, versionName);
    }

    private async Task<string> installProfile(Stream profileStream, MinecraftPath installTo, string versionName)
    {
        var versionJsonPath = installTo.GetVersionJsonPath(versionName);
        IOUtil.CreateParentDirectory(versionJsonPath);
        using var versionJsonFileStream = File.Create(versionJsonPath);
        await profileStream.CopyToAsync(versionJsonFileStream);
        return versionName;
    }
}
