using CmlLib.Core.Version;
using CmlLib.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;

namespace CmlLib.Core.Downloader
{
    public class MDownloader
    {
        public event DownloadFileChangedHandler ChangeFile;
        public event ProgressChangedEventHandler ChangeProgress;

        public bool IgnoreInvalidFiles { get; set; } = true;
        public bool CheckHash { get; set; } = true;

        protected MVersion version;
        protected MinecraftPath Minecraft;

        public MDownloader(MinecraftPath downloadPath, MVersion _version)
        {
            version = _version;
            Minecraft = downloadPath;
        }

        /// <summary>
        /// Download All files that require to launch
        /// </summary>
        /// <param name="resource"></param>
        public void DownloadAll(bool resource = true)
        {
            DownloadLibraries();

            if (resource)
            {
                DownloadIndex();
                DownloadResource();
            }

            DownloadMinecraft();
        }

        /// <summary>
        /// Download all required library files
        /// </summary>
        public void DownloadLibraries()
        {
            fireDownloadFileChangedEvent(MFile.Library, "", 0, 0);

            var files = from lib in version.Libraries
                        where CheckDownloadRequireLibrary(lib)
                        select new DownloadFile(MFile.Library, lib.Name, Path.Combine(Minecraft.Library, lib.Path), lib.Url);

            DownloadFiles(files.Distinct().ToArray());
        }

        private bool CheckDownloadRequireLibrary(MLibrary lib)
        {
            return lib.IsRequire
                && lib.Path != ""
                && lib.Url != ""
                && !CheckFileValidation(Path.Combine(Minecraft.Library, lib.Path), lib.Hash);
        }

        /// <summary>
        /// Download index file
        /// </summary>
        public void DownloadIndex()
        {
            string path = Path.Combine(Minecraft.Index, version.AssetId + ".json");

            if (version.AssetUrl != "" && !CheckFileValidation(path, version.AssetHash))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                using (var wc = new WebClient())
                {
                    wc.DownloadFile(version.AssetUrl, path);
                }
            }
        }

        /// <summary>
        /// Download assets and copy to legacy or resources
        /// </summary>
        public void DownloadResource()
        {
            var indexpath = Path.Combine(Minecraft.Index, version.AssetId + ".json");
            if (!File.Exists(indexpath)) return;

            fireDownloadFileChangedEvent(MFile.Resource, version.AssetId, 0, 0);

            var json = File.ReadAllText(indexpath);
            var index = JObject.Parse(json);

            var isVirtual = checkJsonTrue(index["virtual"]); // check virtual
            var mapResource = checkJsonTrue(index["map_to_resources"]); // check map_to_resources

            var list = (JObject)index["objects"];
            var downloadRequiredFiles = new List<DownloadFile>();
            var copyRequiredFiles = new List<Tuple<string, string>>();

            foreach (var item in list)
            {
                JToken job = item.Value;

                // download hash resource
                var hash = job["hash"]?.ToString();
                var hashName = hash.Substring(0, 2) + "/" + hash;
                var hashPath = Path.Combine(Minecraft.AssetObject, hashName);
                var hashUrl = MojangServer.ResourceDownload + hashName;

                if (!CheckFileValidation(hashPath, hash))
                    downloadRequiredFiles.Add(new DownloadFile(MFile.Resource, item.Key, hashPath, hashUrl));

                if (isVirtual)
                {
                    var resPath = Path.Combine(Minecraft.AssetLegacy, item.Key);
                    copyRequiredFiles.Add(new Tuple<string, string>(hashPath, resPath));
                }

                if (mapResource)
                {
                    var resPath = Path.Combine(Minecraft.Resource, item.Key);
                    copyRequiredFiles.Add(new Tuple<string, string>(hashPath, resPath));
                }
            }

            DownloadFiles(downloadRequiredFiles.Distinct().ToArray());

            foreach (var item in copyRequiredFiles)
            {
                if (!File.Exists(item.Item2))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(item.Item2));
                    File.Copy(item.Item1, item.Item2, true);
                }
            }
        }

        bool checkJsonTrue(JToken j)
        {
            var str = j?.ToString()?.ToLower();
            if (str != null && str == "true")
                return true;
            else
                return false;
        }

        /// <summary>
        /// Download client jar
        /// </summary>
        public void DownloadMinecraft()
        {
            if (string.IsNullOrEmpty(version.ClientDownloadUrl)) return;

            string id = version.Jar;
            var path = Path.Combine(Minecraft.Versions, id, id + ".jar");

            fireDownloadFileChangedEvent(MFile.Minecraft, id, 1, 0);

            if (!CheckFileValidation(path, version.ClientHash))
            {
                var file = new DownloadFile(MFile.Minecraft, id, path, version.ClientDownloadUrl);
                DownloadFiles(new DownloadFile[] { file });
            }
        }

        private bool CheckFileValidation(string path, string hash)
        {
            return File.Exists(path) && CheckSHA1(path, hash);
        }

        private bool CheckFileValidation(string path, string hash, long size)
        {
            var file = new FileInfo(path);
            return file.Exists && file.Length == size && CheckSHA1(path, hash);
        }

        private bool CheckSHA1(string path, string compareHash)
        {
            try
            {
                if (!CheckHash)
                    return true;

                if (compareHash == null || compareHash == "")
                    return true;

                var fileHash = "";

                using (var file = File.OpenRead(path))
                using (var hasher = new System.Security.Cryptography.SHA1CryptoServiceProvider())
                {
                    var binaryHash = hasher.ComputeHash(file);
                    fileHash = BitConverter.ToString(binaryHash).Replace("-", "").ToLower();
                }

                return fileHash == compareHash;
            }
            catch
            {
                return false;
            }
        }

        protected void fireDownloadFileChangedEvent(MFile file, string name, int totalFiles, int progressedFiles)
        {
            var e = new DownloadFileChangedEventArgs(file, name, totalFiles, progressedFiles);
            fireDownloadFileChangedEvent(e);
        }

        protected void fireDownloadFileChangedEvent(DownloadFileChangedEventArgs e)
        {
            ChangeFile?.Invoke(e);
        }

        private void fireDownloadProgressChangedEvent(object sender, ProgressChangedEventArgs e)
        {
            ChangeProgress?.Invoke(this, e);
        }

        public virtual void DownloadFiles(DownloadFile[] files)
        {
            var webdownload = new WebDownload();
            webdownload.DownloadProgressChangedEvent += fireDownloadProgressChangedEvent;

            var length = files.Length;
            if (length == 0)
                return;

            fireDownloadFileChangedEvent(files[0].Type, files[0].Name, length, 0);

            for (int i = 0; i < length; i++)
            {
                try
                {
                    var downloadFile = files[i];
                    Directory.CreateDirectory(Path.GetDirectoryName(downloadFile.Path));
                    webdownload.DownloadFile(downloadFile.Url, downloadFile.Path);
                    fireDownloadFileChangedEvent(downloadFile.Type, downloadFile.Name, length, i + 1);
                }
                catch (WebException ex)
                {
                    if (!IgnoreInvalidFiles)
                        throw new MDownloadFileException(ex.Message, ex, files[i]);
                }
            }
        }
    }
}
