using CmlLib.Core.Downloader;
using CmlLib.Core.Version;
using CmlLib.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CmlLib.Core.Files
{
    public sealed class AssetChecker : IFileChecker
    {
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
            CheckIndex(path, version);
            return CheckAsset(path, version);
        }

        private void CheckIndex(MinecraftPath path, MVersion version)
        {
            string index = path.GetIndexFilePath(version.AssetId);

            if (!string.IsNullOrEmpty(version.AssetUrl) && !CheckFileValidation(index, version.AssetHash))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(index));

                using (var wc = new WebClient())
                {
                    wc.DownloadFile(version.AssetUrl, index);
                }
            }
        }

        public JObject ReadIndex(MinecraftPath path, MVersion version)
        {
            var indexpath = path.GetIndexFilePath(version.AssetId);
            if (!File.Exists(indexpath)) return null;

            var json = File.ReadAllText(indexpath);
            var index = JObject.Parse(json);

            return index;
        }

        public DownloadFile[] CheckAsset(MinecraftPath path, MVersion version)
        {
            var index = ReadIndex(path, version);
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
                JToken job = item.Value;

                // download hash resource
                var hash = job["hash"]?.ToString();
                var hashName = hash.Substring(0, 2) + "/" + hash;
                var hashPath = Path.Combine(path.GetAssetObjectPath(version.AssetId), hashName);

                var afterDownload = new List<Action>(1);

                if (isVirtual)
                {
                    afterDownload.Add(new Action(() =>
                    {
                        var resPath = Path.Combine(path.GetAssetLegacyPath(version.AssetId), item.Key);
                        safeCopy(hashPath, resPath);
                    }));
                }

                if (mapResource)
                {
                    afterDownload.Add(new Action(() =>
                    {
                        var resPath = Path.Combine(path.Resource, item.Key);
                        safeCopy(hashPath, resPath);
                    }));
                }

                if (!CheckFileValidation(hashPath, hash))
                {
                    var hashUrl = AssetServer + hashName;
                    downloadRequiredFiles.Add(new DownloadFile
                    {
                        Type = MFile.Resource,
                        Name = item.Key,
                        Path = hashPath,
                        Url = hashUrl,
                        AfterDownload = afterDownload.ToArray()
                    });
                }
                else
                {
                    afterDownload.ForEach(x => x.Invoke());
                }

                progressed++;
                fireDownloadFileChangedEvent(MFile.Resource, "", total, progressed);
            }

            return downloadRequiredFiles.Distinct().ToArray();
        }

        private void fireDownloadFileChangedEvent(MFile file, string name, int totalFiles, int progressedFiles)
        {
            var e = new DownloadFileChangedEventArgs(file, name, totalFiles, progressedFiles);
            ChangeFile?.Invoke(e);
        }

        private bool checkJsonTrue(JToken j)
        {
            var str = j?.ToString()?.ToLower();
            if (str != null && str == "true")
                return true;
            else
                return false;
        }

        private bool CheckFileValidation(string path, string hash)
        {
            if (!File.Exists(path))
                return false;

            if (!CheckHash)
                return true;
            else
                return IOUtil.CheckFileValidation(path, hash);
        }

        private void safeCopy(string org, string des)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(des));
                File.Copy(org, des, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString());
            }
        }
    }
}
