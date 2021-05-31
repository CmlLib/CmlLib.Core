using CmlLib.Core.Downloader;
using CmlLib.Core.Version;
using CmlLib.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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

        public DownloadFile[] CheckFiles(MinecraftPath path, MVersion version, 
            IProgress<DownloadFileChangedEventArgs> progress)
        {
            return checkIndexAndAsset(path, version, progress);
        }

        public Task<DownloadFile[]> CheckFilesTaskAsync(MinecraftPath path, MVersion version, 
            IProgress<DownloadFileChangedEventArgs> progress)
        {
            return Task.Run(() => checkIndexAndAsset(path, version, progress));
        }

        private DownloadFile[] checkIndexAndAsset(MinecraftPath path, MVersion version,
            IProgress<DownloadFileChangedEventArgs> progress)
        {
            checkIndex(path, version);
            return CheckAssetFiles(path, version, progress);
        }

        private void checkIndex(MinecraftPath path, MVersion version)
        {
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
        public JObject ReadIndex(MinecraftPath path, MVersion version)
        {
            string indexpath = path.GetIndexFilePath(version.AssetId);
            if (!File.Exists(indexpath)) return null;

            string json = File.ReadAllText(indexpath);
            var index = JObject.Parse(json); // 100ms

            return index;
        }

        [MethodTimer.Time]
        public DownloadFile[] CheckAssetFiles(MinecraftPath path, MVersion version,
            IProgress<DownloadFileChangedEventArgs> progress)
        {
            JObject index = ReadIndex(path, version);
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
                var f = checkAssetFile(item.Key, item.Value, path, version, isVirtual, mapResource);

                if (f != null)
                    downloadRequiredFiles.Add(f);

                progressed++;
                
                if (progressed % 50 == 0) // prevent ui freezing
                    progress?.Report(
                        new DownloadFileChangedEventArgs(MFile.Resource, "", total, progressed));
            }

            return downloadRequiredFiles.Distinct().ToArray(); // 10ms
        }

        private DownloadFile checkAssetFile(string key, JToken job, MinecraftPath path, MVersion version, bool isVirtual, bool mapResource)
        {
            // download hash resource
            string hash = job["hash"]?.ToString();
            if (hash == null)
                return null;

            string hashName = hash.Substring(0, 2) + "/" + hash;
            string hashPath = Path.Combine(path.GetAssetObjectPath(version.AssetId), hashName);

            string sizestr = job["size"]?.ToString();
            long.TryParse(sizestr, out long size);

            var afterDownload = new List<Func<Task>>(1);

            if (isVirtual)
            {
                afterDownload.Add(async () =>
                {
                    string resPath = Path.Combine(path.GetAssetLegacyPath(version.AssetId), key);
                    if (!await IOUtil.CheckFileValidationAsync(resPath, hash, CheckHash).ConfigureAwait(false))
                        await safeCopy(hashPath, resPath).ConfigureAwait(false);
                });
            }

            if (mapResource)
            {
                afterDownload.Add(async () =>
                {
                    string resPath = Path.Combine(path.Resource, key);
                    if (!await IOUtil.CheckFileValidationAsync(resPath, hash, CheckHash).ConfigureAwait(false))
                        await safeCopy(hashPath, resPath).ConfigureAwait(false);
                });
            }

            if (!IOUtil.CheckFileValidation(hashPath, hash, CheckHash))
            {
                string hashUrl = AssetServer + hashName;
                return new DownloadFile
                {
                    Type = MFile.Resource,
                    Name = key,
                    Path = hashPath,
                    Url = hashUrl,
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

        private bool checkJsonTrue(JToken j)
        {
            string str = j?.ToString().ToLowerInvariant();
            if (str != null && str == "true")
                return true;
            else
                return false;
        }

        private async Task safeCopy(string org, string des)
        {
            try
            {
                var directoryName = Path.GetDirectoryName(des);
                if (string.IsNullOrEmpty(directoryName))
                    return;

                Directory.CreateDirectory(directoryName);
                await IOUtil.CopyFileAsync(org, des)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString());
            }
        }
    }
}
