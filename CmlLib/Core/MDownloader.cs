using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;

namespace CmlLib.Core
{
    public class MDownloader
    {
        public class DownloadFile : IEquatable<DownloadFile>
        {
            public DownloadFile(MFile type, string name, string path, string url)
            {
                this.Type = type;
                this.Name = name;
                this.Path = path;
                this.Url = url;
            }

            public MFile Type { get; private set; }
            public string Name { get; private set; }
            public string Path { get; private set; }
            public string Url { get; private set; }

            bool IEquatable<DownloadFile>.Equals(DownloadFile other)
            {
                if (other == null)
                    return false;

                return this.Path == other.Path;
            }

            public override int GetHashCode()
            {
                return this.Path.GetHashCode();
            }
        }

        public class MDownloadFileException : Exception
        {
            public MDownloadFileException(DownloadFile exFile)
                : this(null, null, exFile) { }

            public MDownloadFileException(string message, Exception innerException, DownloadFile exFile)
                : base(message, innerException)
            {
                this.ExceptionFile = exFile;
            }

            public DownloadFile ExceptionFile { get; private set; }
        }

        public event DownloadFileChangedHandler ChangeFile;
        public event ProgressChangedEventHandler ChangeProgress;

        public bool CheckHash { get; set; } = true;

        protected MProfile profile;
        protected Minecraft Minecraft;

        public MDownloader(MProfile _profile)
        {
            this.profile = _profile;
            this.Minecraft = _profile.Minecraft;
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

            var files = from lib in profile.Libraries
                        where CheckDownloadRequireLibrary(lib)
                        select new DownloadFile(MFile.Library, lib.Name, lib.Path, lib.Url);

            DownloadFiles(files.Distinct().ToArray());
        }

        private bool CheckDownloadRequireLibrary(MLibrary lib)
        {
            return lib.IsRequire
                && lib.Path != ""
                && lib.Url != ""
                && !CheckFileValidation(lib.Path, lib.Hash);
        }

        /// <summary>
        /// Download index file
        /// </summary>
        public void DownloadIndex()
        {
            string path = Path.Combine(Minecraft.Index, profile.AssetId + ".json");

            if (profile.AssetUrl != "" && !CheckFileValidation(path, profile.AssetHash))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                using (var wc = new WebClient())
                {
                    wc.DownloadFile(profile.AssetUrl, path);
                }
            }
        }

        /// <summary>
        /// Download assets and copy to legacy or resources
        /// </summary>
        public void DownloadResource()
        {
            var indexpath = Path.Combine(Minecraft.Index, profile.AssetId + ".json");
            if (!File.Exists(indexpath)) return;

            fireDownloadFileChangedEvent(MFile.Resource, profile.AssetId, 0, 0);

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
            if (string.IsNullOrEmpty(profile.ClientDownloadUrl)) return;

            string id = profile.Jar;
            var path = Path.Combine(Minecraft.Versions, id, id + ".jar");

            fireDownloadFileChangedEvent(MFile.Minecraft, id, 1, 0);

            if (!CheckFileValidation(path, profile.ClientHash))
            {
                var file = new DownloadFile(MFile.Minecraft, id, path, profile.ClientDownloadUrl);
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
            var e = new DownloadFileChangedEventArgs()
            {
                FileKind = file,
                FileName = name,
                TotalFileCount = totalFiles,
                ProgressedFileCount = progressedFiles
            };
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
                    throw new MDownloadFileException(ex.Message, ex, files[i]);
                }
            }
        }
    }
}
