using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using CmlLib.Core.Downloader;
using CmlLib.Core.Files;
using CmlLib.Core.Version;
using CmlLib.Utils;

namespace CmlLib.Core.FileChecker
{
    public sealed class AssetChecker : IFileChecker
    {
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
        public bool CheckHash { get; set; } = true;

        public DownloadFile[]? CheckFiles(MinecraftPath path, MVersion version, 
            IProgress<DownloadFileChangedEventArgs>? progress)
        {
            return checkIndexAndAsset(path, version, progress);
        }

        public Task<DownloadFile[]?> CheckFilesTaskAsync(MinecraftPath path, MVersion version, 
            IProgress<DownloadFileChangedEventArgs>? progress)
        {
            return Task.Run(() => checkIndexAndAsset(path, version, progress));
        }

        private DownloadFile[]? checkIndexAndAsset(MinecraftPath path, MVersion version,
            IProgress<DownloadFileChangedEventArgs>? progress)
        {
            if (version.Assets == null)
                return null;
            checkIndex(path, version.Assets);
            return CheckAssetFiles(path, version.Assets, progress);
        }

        // Check index file validation and download
        private void checkIndex(MinecraftPath path, MFileMetadata assets)
        {
            if (string.IsNullOrEmpty(assets.Id) || string.IsNullOrEmpty(assets.Url))
                return;

            var indexFilePath = path.GetIndexFilePath(assets.Id);

            if (IOUtil.CheckFileValidation(indexFilePath, assets.Sha1, CheckHash))
                return;
            
            var directoryName = Path.GetDirectoryName(indexFilePath);
            if (!string.IsNullOrEmpty(directoryName))
                Directory.CreateDirectory(directoryName);

            using var wc = new WebClient();
            wc.DownloadFile(assets.Url, indexFilePath);
        }

        [MethodTimer.Time]
        public DownloadFile[]? CheckAssetFiles(MinecraftPath path, MFileMetadata assets,
            IProgress<DownloadFileChangedEventArgs>? progress)
        {
            if (string.IsNullOrEmpty(assets.Id))
                return null;

            var indexFilePath = path.GetIndexFilePath(assets.Id);
            if (!File.Exists(indexFilePath))
                return null;

            var indexFileContent = File.ReadAllText(indexFilePath);
            using var index = JsonDocument.Parse(indexFileContent);
            var root = index.RootElement;

            var listProperty = root.SafeGetProperty("objects");
            if (!listProperty.HasValue)
                return null;
            var list = listProperty.Value;

            var isVirtual = root.SafeGetProperty("virtual")?.GetBoolean() ?? false;
            var mapResource = root.SafeGetProperty("map_to_resources")?.GetBoolean() ?? false;
            
            var objects = list.EnumerateObject();
            var totalObject = objects.Count();
            objects.Reset();
            var downloadRequiredFiles = new List<DownloadFile>(totalObject);
            
            int progressed = 0;
            foreach (var prop in objects)
            {
                var f = checkAssetFile(prop.Name, prop.Value, path, assets, isVirtual, mapResource);

                if (f != null)
                    downloadRequiredFiles.Add(f);

                progressed++;

                if (progressed % 50 == 0) // prevent ui freezing
                    progress?.Report(
                        new DownloadFileChangedEventArgs(MFile.Resource, this, "", totalObject, progressed));
            }

            return downloadRequiredFiles.Distinct().ToArray(); // 10ms
        }

        private DownloadFile? checkAssetFile(string key, JsonElement element, MinecraftPath path, MFileMetadata assets, 
            bool isVirtual, bool mapResource)
        {
            if (string.IsNullOrEmpty(assets.Id))
                return null;
            
            var hash = element.GetPropertyValue("hash");
            if (hash == null)
                return null;

            var hashName = hash.Substring(0, 2) + "/" + hash;
            var hashPath = Path.Combine(path.GetAssetObjectPath(assets.Id), hashName);

            long size = element.SafeGetProperty("size")?.GetInt64() ?? 0;

            var afterDownload = new List<Func<Task>>(1);

            if (isVirtual)
            {
                var resPath = Path.Combine(path.GetAssetLegacyPath(assets.Id), key);
                afterDownload.Add(() => assetCopy(hashPath, resPath));
            }

            if (mapResource)
            {
                var desPath = Path.Combine(path.Resource, key);
                afterDownload.Add(() => assetCopy(hashPath, desPath));
            }

            if (!IOUtil.CheckFileValidation(hashPath, hash, CheckHash))
            {
                string hashUrl = AssetServer + hashName;
                return new DownloadFile(hashPath, hashUrl)
                {
                    Type = MFile.Resource,
                    Name = key,
                    Size = size,
                    AfterDownload = afterDownload.ToArray()
                };
            }
            else
            {
                foreach (var item in afterDownload)
                {
                    item().GetAwaiter().GetResult();
                }

                return null;
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
                    var directoryName = Path.GetDirectoryName(des);
                    if (!string.IsNullOrEmpty(directoryName))
                        Directory.CreateDirectory(directoryName);

                    await IOUtil.CopyFileAsync(org, des);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
