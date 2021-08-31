using CmlLib.Core.Downloader;
using CmlLib.Core.Version;
using CmlLib.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CmlLib.Core.Files
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
            checkIndex(path, version);
            return CheckAssetFiles(path, version, progress);
        }

        private void checkIndex(MinecraftPath path, MVersion version)
        {
            if (string.IsNullOrEmpty(version.AssetId))
                return;

            string index = path.GetIndexFilePath(version.AssetId);

            if (!string.IsNullOrEmpty(version.AssetUrl))
                if (!IOUtil.CheckFileValidation(index, version.AssetHash, CheckHash))
                {
                    var directoryName = Path.GetDirectoryName(index);
                    if (!string.IsNullOrEmpty(directoryName))
                        Directory.CreateDirectory(directoryName);

                    using (var wc = new WebClient())
                    {
                        wc.DownloadFile(version.AssetUrl, index);
                    }
                }
        }

        [MethodTimer.Time]
        public JObject? ReadIndex(MinecraftPath path, MVersion version)
        {
            if (string.IsNullOrEmpty(version.AssetId))
                return null;
            
            string indexPath = path.GetIndexFilePath(version.AssetId);
            if (!File.Exists(indexPath)) return null;

            string json = File.ReadAllText(indexPath);
            var index = JObject.Parse(json); // 100ms

            return index;
        }

        [MethodTimer.Time]
        public DownloadFile[]? CheckAssetFiles(MinecraftPath path, MVersion version,
            IProgress<DownloadFileChangedEventArgs>? progress)
        {
            JObject? index = ReadIndex(path, version);
            if (index == null)
                return null;

            bool isVirtual = checkJsonTrue(index["virtual"]); // check virtual
            bool mapResource = checkJsonTrue(index["map_to_resources"]); // check map_to_resources

            var list = index["objects"] as JObject;
            if (list == null)
                return null;

            var downloadRequiredFiles = new List<DownloadFile>(list.Count);

            int total = list.Count;
            int progressed = 0;

            foreach (var item in list)
            {
                if (item.Value != null)
                {
                    var f = checkAssetFile(item.Key, item.Value, path, version, isVirtual, mapResource);

                    if (f != null)
                        downloadRequiredFiles.Add(f);
                }

                progressed++;
                
                if (progressed % 50 == 0) // prevent ui freezing
                    progress?.Report(
                        new DownloadFileChangedEventArgs(MFile.Resource, this, "", total, progressed));
            }

            return downloadRequiredFiles.Distinct().ToArray(); // 10ms
        }

        private DownloadFile? checkAssetFile(string key, JToken job, MinecraftPath path, MVersion version, 
            bool isVirtual, bool mapResource)
        {
            if (string.IsNullOrEmpty(version.AssetId))
                return null;
            
            // download hash resource
            string? hash = job["hash"]?.ToString();
            if (hash == null)
                return null;

            var hashName = hash.Substring(0, 2) + "/" + hash;
            var hashPath = Path.Combine(path.GetAssetObjectPath(version.AssetId), hashName);

            long size = 0;
            string? sizeStr = job["size"]?.ToString();
            if (!string.IsNullOrEmpty(sizeStr))
                long.TryParse(sizeStr, out size);

            var afterDownload = new List<Func<Task>>(1);

            if (isVirtual)
            {
                var resPath = Path.Combine(path.GetAssetLegacyPath(version.AssetId), key);
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

        private bool checkJsonTrue(JToken? j)
        {
            string? str = j?.ToString().ToLowerInvariant();
            return str is "true";
        }
    }
}
