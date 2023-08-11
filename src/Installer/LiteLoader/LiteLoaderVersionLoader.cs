using System.Text.Json;
using CmlLib.Core.VersionLoader;
using CmlLib.Core.VersionMetadata;
using CmlLib.Core.Internals;

namespace CmlLib.Core.Installer.LiteLoader;

public class LiteLoaderVersionLoader : IVersionLoader
{
    public const string LiteLoaderLibName = "com.mumfrey:liteloader";
    public const string ManifestServer = "http://dl.liteloader.com/versions/versions.json";
    
    private readonly HttpClient _httpClient;

    public LiteLoaderVersionLoader(HttpClient httpClient) =>
        _httpClient = httpClient;

    public async ValueTask<VersionCollection> GetVersionMetadatasAsync()
    {
        var res = await _httpClient.GetStringAsync(ManifestServer)
            .ConfigureAwait(false);

        return parseVersions(res);
    }

    private VersionCollection parseVersions(string json)
    {
        using var jsonDocument = JsonDocument.Parse(json);
        var root = jsonDocument.RootElement; 
        
        var metadataList = new List<IVersionMetadata>();
        if (root.TryGetProperty("versions", out var versions))
        {
            foreach (var item in versions.EnumerateObject())
            {
                var vanillaVersion = item.Name;
                var versionObj = item.Value;

                var libObj = 
                    versionObj.GetPropertyOrNull("artefacts") ?? 
                    versionObj.GetPropertyOrNull("snapshots");

                var latestLiteLoader = libObj?
                    .GetPropertyOrNull(LiteLoaderLibName)?
                    .GetPropertyOrNull("latest");

                if (latestLiteLoader == null)
                    continue;

                var model = new JsonVersionMetadataModel
                {
                    Name = $"LiteLoader-{vanillaVersion}",
                    Type = versionObj.GetPropertyOrNull("repo")?.GetPropertyValue("stream")
                };
                var metadata = new LiteLoaderVersionMetadata(model, vanillaVersion, latestLiteLoader.Value);

                metadataList.Add(metadata);
            }
        }

        return new VersionCollection(metadataList, null, null);
    }
}