using CmlLib.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CmlLib.Core.Version;
using System.Threading;
using CmlLib.Core.Downloader;

namespace CmlLib.Core.Files
{
    public sealed class AssetChecker : IFileChecker
    {
        IProgress<DownloadFileChangedEventArgs> pChangeFile;
        public event DownloadFileChangedHandler ChangeFile;

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

        public DownloadFile[] CheckFiles(MinecraftPath path, MVersion version)
        {
            return CheckFilesTaskAsync(path, version).GetAwaiter().GetResult();
        }

        public async Task<DownloadFile[]> CheckFilesTaskAsync(MinecraftPath path, MVersion version)
        {
            pChangeFile = new Progress<DownloadFileChangedEventArgs>(
                (e) => ChangeFile?.Invoke(e));

            await CheckIndex(path, version);
            return await CheckAssetFiles(path, version);
        }

        private async Task CheckIndex(MinecraftPath path, MVersion version)
        {
            string index = path.GetIndexFilePath(version.AssetId);

            if (!string.IsNullOrEmpty(version.AssetUrl))
            if (!await IOUtil.CheckFileValidationAsync(index, version.AssetHash, CheckHash))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(index));

                using (var wc = new WebClient())
                {
                    await wc.DownloadFileTaskAsync(version.AssetUrl, index);
                }
            }
        }

        public async Task<JObject> ReadIndexAsync(MinecraftPath path, MVersion version)
        {
            var indexpath = path.GetIndexFilePath(version.AssetId);
            if (!File.Exists(indexpath)) return null;

            var json = await IOUtil.ReadFileAsync(indexpath);
            var index = JObject.Parse(json);

            return index;
        }

        public async Task<DownloadFile[]> CheckAssetFiles(MinecraftPath path, MVersion version)
        {
            var index = await ReadIndexAsync(path, version);
            if (index == null)
                return null;

            var isVirtual = checkJsonTrue(index["virtual"]); // check virtual
            var mapResource = checkJsonTrue(index["map_to_resources"]); // check map_to_resources

            var list = (JObject)index["objects"];
            var downloadRequiredFiles = new List<DownloadFile>(list.Count);

            int total = list.Count;
            int progressed = 0;

            foreach (var item in list)
            {
                var task = Task.Run(() => CheckAssetFile(item.Key, item.Value, path, version, isVirtual, mapResource));
                fireDownloadFileChangedEvent(MFile.Resource, "", total, progressed);

                var f = await task;
                if (f != null)
                    downloadRequiredFiles.Add(f);

                Interlocked.Increment(ref progressed);
            }

            fireDownloadFileChangedEvent(MFile.Resource, "", total, total);

            return downloadRequiredFiles.Distinct().ToArray();
        }

        private DownloadFile CheckAssetFile(string key, JToken job, MinecraftPath path, MVersion version, bool isVirtual, bool mapResource)
        {
            // download hash resource
            var hash = job["hash"]?.ToString();
            var hashName = hash.Substring(0, 2) + "/" + hash;
            var hashPath = Path.Combine(path.GetAssetObjectPath(version.AssetId), hashName);

            var sizestr = job["size"]?.ToString();
            long size = 0;
            long.TryParse(sizestr, out size);

            var afterDownload = new List<Func<Task>>(1);

            if (isVirtual)
            {
                afterDownload.Add(new Func<Task>(async () =>
                {
                    var resPath = Path.Combine(path.GetAssetLegacyPath(version.AssetId), key);
                    if (!await IOUtil.CheckFileValidationAsync(resPath, hash, CheckHash))
                        await safeCopy(hashPath, resPath);
                }));
            }

            if (mapResource)
            {
                afterDownload.Add(new Func<Task>(async () =>
                {
                    var resPath = Path.Combine(path.Resource, key);
                    if (!await IOUtil.CheckFileValidationAsync(resPath, hash, CheckHash))
                        await safeCopy(hashPath, resPath);
                }));
            }

            if (!IOUtil.CheckFileValidation(hashPath, hash, CheckHash))
            {
                var hashUrl = AssetServer + hashName;
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

        private void fireDownloadFileChangedEvent(MFile file, string name, int totalFiles, int progressedFiles)
        {
            var e = new DownloadFileChangedEventArgs(file, name, totalFiles, progressedFiles);
            //ChangeFile?.Invoke(e);
            pChangeFile.Report(e);
        }

        private bool checkJsonTrue(JToken j)
        {
            var str = j?.ToString()?.ToLower();
            if (str != null && str == "true")
                return true;
            else
                return false;
        }

        private async Task safeCopy(string org, string des)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(des));
                await IOUtil.CopyFileAsync(org, des);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString());
            }
        }
    }
}
