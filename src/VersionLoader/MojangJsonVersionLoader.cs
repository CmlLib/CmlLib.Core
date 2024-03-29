using CmlLib.Core.VersionMetadata;
using CmlLib.Core.Internals;
using System.Text.Json;

namespace CmlLib.Core.VersionLoader;

public class MojangJsonVersionLoader : IVersionLoader
{
    private readonly HttpClient _httpClient;
    private readonly string _endpoint;

    public MojangJsonVersionLoader(HttpClient httpClient) =>
        (_httpClient, _endpoint) = (httpClient, MojangServer.Version);

    public MojangJsonVersionLoader(HttpClient httpClient, string endpoint) =>
        (_httpClient, _endpoint) = (httpClient, endpoint);

    public async ValueTask<VersionMetadataCollection> GetVersionMetadatasAsync()
    {
        var manifest = await GetManifestAsync();
        return new VersionMetadataCollection(
            manifest?.Versions?.Select(v => new MojangVersionMetadata(v, _httpClient)) ?? [], 
            manifest?.Latest?.Release, 
            manifest?.Latest?.Snapshot);
    }

    public async Task<JsonVersionManifestModel?> GetManifestAsync()
    {
        using var res = await _httpClient.GetStreamAsync(_endpoint);
        return await JsonSerializer.DeserializeAsync<JsonVersionManifestModel>(res);
    }
}
