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
        var res = await _httpClient.GetStreamAsync(_endpoint)
            .ConfigureAwait(false);

        using var jsonDocument = await JsonDocument.ParseAsync(res);
        var root = jsonDocument.RootElement;
        
        var latestReleaseId = root.GetPropertyOrNull("latest")?.GetPropertyValue("release");
        var latestSnapshotId = root.GetPropertyOrNull("latest")?.GetPropertyValue("snapshot");

        var metadatas = new List<IVersionMetadata>();
        if (root.TryGetProperty("versions", out var versions) && 
            versions.ValueKind == JsonValueKind.Array)
        {
            foreach (var t in versions.EnumerateArray())
            {
                var metadataModel = t.Deserialize<JsonVersionMetadataModel>();
                if (metadataModel == null)
                    continue;

                var metadata = new MojangVersionMetadata(metadataModel, _httpClient);
                metadatas.Add(metadata);
            }
        }

        return new VersionMetadataCollection(metadatas, latestReleaseId, latestSnapshotId);
    }
}
