using System.Diagnostics;
using System.Text.Json;
using CmlLib.Core.Files;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;
using CmlLib.Utils;

namespace CmlLib.Core.FileExtractors;

public class AssetFileExtractor : IFileExtractor
{
    private readonly HttpClient httpClient;

    public AssetFileExtractor(HttpClient client)
    {
        this.httpClient = client;
    }

    private string assetServer = MojangServer.ResourceDownload;
    public string AssetServer
    {
        get => assetServer;
        set
        {
            if (value.Last() == '/')
                assetServer = value;
            else
                assetServer = value + "/";
        }
    }

    public async ValueTask<IEnumerable<LinkedTask>> Extract(MinecraftPath path, IVersion version)
    {
        var assets = version.AssetIndex;
        if (assets == null ||
            string.IsNullOrEmpty(assets.Id) ||
            string.IsNullOrEmpty(assets.Url))
            return Enumerable.Empty<LinkedTask>();

        using var assetIndexStream = await createAssetIndexStream(path, assets);
        var assetIndexJson = JsonDocument.Parse(assetIndexStream);
        return extractFromAssetIndexJson(assetIndexJson, path, version);
    }

    // Check index file validation and download
    private async ValueTask<Stream> createAssetIndexStream(MinecraftPath path, MFileMetadata assets)
    {
        Debug.Assert(!string.IsNullOrEmpty(assets?.Id));
        Debug.Assert(!string.IsNullOrEmpty(assets?.Url));

        var indexFilePath = path.GetIndexFilePath(assets.Id);

        if (IOUtil.CheckFileValidation(indexFilePath, assets.Sha1, true))
        {
            return File.Open(indexFilePath, FileMode.Open);
        }
        else
        {
            IOUtil.CreateParentDirectory(indexFilePath);

            var ms = new MemoryStream();
            using (var res = await httpClient.GetStreamAsync(assets.Url))
            {
                await res.CopyToAsync(ms);
            } // dispose immediately

            using (var fs = File.Create(indexFilePath))
            {
                await ms.CopyToAsync(fs);
            } // dispose immediately

            ms.Position = 0;
            return ms;
        }
    }

    private IEnumerable<LinkedTask> extractFromAssetIndexJson(
        JsonDocument _json, MinecraftPath path, IVersion version)
    {
        Debug.Assert(!string.IsNullOrEmpty(version.AssetIndex?.Id));

        using var json = _json;
        var root = json.RootElement;

        var assetId = version.AssetIndex.Id;
        var isVirtual = root.GetPropertyOrNull("virtual")?.GetBoolean() ?? false;
        var mapResource = root.GetPropertyOrNull("map_to_resources")?.GetBoolean() ?? false;

        if (!root.TryGetProperty("objects", out var objectsProp))
            yield break;

        var objects = objectsProp.EnumerateObject();
        foreach (var prop in objects)
        {
            var hash = prop.Value.GetPropertyValue("hash");
            if (hash == null)
                continue;

            var hashName = hash.Substring(0, 2) + "/" + hash;
            var hashPath = Path.Combine(path.GetAssetObjectPath(assetId), hashName);
            long size = prop.Value.GetPropertyOrNull("size")?.GetInt64() ?? 0;

            var copyPath = new List<string>(2);
            if (isVirtual)
                copyPath.Add(Path.Combine(path.GetAssetLegacyPath(assetId), prop.Name));
            if (mapResource)
                copyPath.Add(Path.Combine(path.Resource, prop.Name));

            var file = new TaskFile(prop.Name)
            {
                Path = hashPath,
                Hash = hash,
                Size = size,
                Url = AssetServer + hashName
            };

            var checkTask = new FileCheckTask(file);
            checkTask.OnFalse = new DownloadTask(file);

            if (copyPath.Count > 0)
                checkTask.InsertNextTask(new FileCopyTask(prop.Name, hashPath, copyPath.ToArray()));
            
            yield return checkTask;
        }
    }
}
