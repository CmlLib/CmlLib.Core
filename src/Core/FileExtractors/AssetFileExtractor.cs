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
        if (version.AssetIndex == null || string.IsNullOrEmpty(version.AssetIndex.Id))
            return Enumerable.Empty<LinkedTask>();

        var assetIndex = await getAssetIndex(path, version.AssetIndex);
        if (assetIndex == null)
            return Enumerable.Empty<LinkedTask>();

        return extractFromIndex(path, version, assetIndex);
    }

    private IEnumerable<LinkedTask> extractFromIndex(
        MinecraftPath path, IVersion version, Stream stream)
    {
        using var s = stream;
        using var doc = JsonDocument.Parse(s);
        var root = doc.RootElement;

        var assetId = version.AssetIndex?.Id ?? "legacy";
        var isVirtual = root.GetPropertyOrNull("virtual")?.GetBoolean() ?? false;
        var mapResource = root.GetPropertyOrNull("map_to_resources")?.GetBoolean() ?? false;

        var objectsProp = root.GetPropertyOrNull("objects");
        if (!objectsProp.HasValue)
            yield break;

        var objects = objectsProp.Value.EnumerateObject();
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

    // Check index file validation and download
    private async ValueTask<Stream?> getAssetIndex(MinecraftPath path, MFileMetadata? assets)
    {
        if (assets == null ||
            string.IsNullOrEmpty(assets.Id) ||
            string.IsNullOrEmpty(assets.Url))
            return null;

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

            return ms;
        }
    }

    private async Task assetCopy(string org, string des)
    {
        try
        {
            var orgFile = new FileInfo(org);
            var desFile = new FileInfo(des);

            if (!desFile.Exists || orgFile.Length != desFile.Length)
            {
                IOUtil.CreateParentDirectory(des);
                await IOUtil.CopyFileAsync(org, des);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }
}
