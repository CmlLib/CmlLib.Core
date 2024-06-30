using CmlLib.Core.Files;
using CmlLib.Core.Internals;
using CmlLib.Core.Rules;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;
using System.Diagnostics;
using System.Text.Json;

namespace CmlLib.Core.FileExtractors;

public class AssetFileExtractor : IFileExtractor
{
    private readonly HttpClient httpClient;

    public AssetFileExtractor(HttpClient client)
    {
        this.httpClient = client;
    }

    public string AssetServer { get; set; } = MojangServer.ResourceDownload;

    public async ValueTask<IEnumerable<GameFile>> Extract(
        MinecraftPath path,
        IVersion version,
        RulesEvaluatorContext rulesContext,
        CancellationToken cancellationToken)
    {
        var assetIndex = await loadAssetIndex(path, version.AssetIndex);
        if (assetIndex == null)
            return Enumerable.Empty<GameFile>();

        return Extractor.ExtractTasksFromAssetIndex(
            assetIndex, 
            path, 
            AssetServer,
            dispose: true);
    }

    private async ValueTask<IAssetIndex?> loadAssetIndex(MinecraftPath path, MFileMetadata? assets)
    {
        if (assets == null ||
            string.IsNullOrEmpty(assets?.Id))
            return null;

        using var assetIndexStream = await createAssetIndexStream(path, assets);
        if (assetIndexStream == null)
            return null;

        var assetIndexJson = await JsonDocument.ParseAsync(assetIndexStream);
        return new JsonAssetIndex(assets.Id, assetIndexJson);
    }

    private async ValueTask<Stream?> createAssetIndexStream(MinecraftPath path, MFileMetadata assets)
    {
        Debug.Assert(!string.IsNullOrEmpty(assets.Id));

        var indexFilePath = path.GetIndexFilePath(assets.Id);
        if (IOUtil.CheckFileValidation(indexFilePath, assets.Sha1))
        {
            return File.Open(indexFilePath, FileMode.Open);
        }
        else
        {
            if (string.IsNullOrEmpty(assets.Url))
                return null;

            IOUtil.CreateParentDirectory(indexFilePath);
            var httpStream = await httpClient.GetStreamAsync(assets.Url);
            var fileStream = File.Create(indexFilePath);
            return new PipedStream(httpStream, fileStream, writeToEndOnClose: true);
        }
    }

    public static class Extractor
    {
        public static IEnumerable<GameFile> ExtractTasksFromAssetIndex(
            IAssetIndex assetIndex, MinecraftPath path, string assetServer, bool dispose)
        {
            if (assetServer.Last() != '/')
                assetServer += '/';

            foreach (var assetObject in assetIndex.EnumerateAssetObjects())
            {
                var hashName = assetObject.Hash.Substring(0, 2) + "/" + assetObject.Hash;
                var hashPath = IOUtil.NormalizePath(Path.Combine(path.GetAssetObjectPath(assetIndex.Id), hashName));

                var copyPath = new List<string>(2);
                if (assetIndex.IsVirtual)
                    copyPath.Add(IOUtil.NormalizePath(Path.Combine(path.GetAssetLegacyPath(assetIndex.Id), assetObject.Name)));
                if (assetIndex.MapToResources)
                    copyPath.Add(IOUtil.NormalizePath(Path.Combine(path.Resource, assetObject.Name)));

                yield return new GameFile(assetObject.Name)
                {
                    Path = hashPath,
                    Hash = assetObject.Hash,
                    Size = assetObject.Size,
                    Url = assetServer + hashName,
                    UpdateTask = copyPath.Select(path => new FileCopyTask(path))
                };
            }

            if (dispose && assetIndex is IDisposable disposableAssetIndex)
                disposableAssetIndex.Dispose();
        }
    }
}
