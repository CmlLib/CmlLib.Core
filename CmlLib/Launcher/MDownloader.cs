using System;
using System.IO.Compression;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.ComponentModel;

namespace CmlLib.Launcher
{
    public delegate void DownloadFileChangedHandler(DownloadFileChangedEventArgs e);

    public class MDownloader
    {
        public event DownloadFileChangedHandler ChangeFile;
        public event ProgressChangedEventHandler ChangeProgress;

        public bool CheckHash { get; set; } = true;

        MProfile profile;
        WebDownload web;

        public MDownloader(MProfile _profile)
        {
            this.profile = _profile;

            web = new WebDownload();
            web.DownloadProgressChangedEvent += Web_DownloadProgressChangedEvent;
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
            int index = 0;
            int maxCount = profile.Libraries.Length;
            foreach (var item in profile.Libraries)
            {
                try
                {
                    if (CheckDownloadRequireLibrary(item))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(item.Path));
                        web.DownloadFile(item.Url, item.Path);
                    } 
                }
                catch
                {
                }

                l(MFile.Library, item.Name, maxCount, ++index); // event
            }
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
            string path = Minecraft.Index + profile.AssetId + ".json";

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
            var indexpath = Minecraft.Index + profile.AssetId + ".json";
            if (!File.Exists(indexpath)) return;

            using (var wc = new WebClient())
            {
                bool isVirtual = false;
                bool mapResource = false;

                var json = File.ReadAllText(indexpath);
                var index = JObject.Parse(json);

                var virtualValue = index["virtual"]?.ToString()?.ToLower(); // check virtual
                if (virtualValue != null && virtualValue == "true")
                    isVirtual = true;

                var mapResourceValue = index["map_to_resources"]?.ToString()?.ToLower(); // check map_to_resources
                if (mapResourceValue != null && mapResourceValue == "true")
                    mapResource = true;

                var list = (JObject)index["objects"];
                var count = list.Count;
                var i = 0;

                foreach (var item in list)
                {
                    JToken job = item.Value;

                    // download hash resource
                    var hash = job["hash"]?.ToString();
                    var hashName = hash.Substring(0, 2) + "\\" + hash;
                    var hashPath = Minecraft.AssetObject + hashName;
                    var hashUrl = "http://resources.download.minecraft.net/" + hashName;
                    Directory.CreateDirectory(Path.GetDirectoryName(hashPath));

                    if (!File.Exists(hashPath))
                        wc.DownloadFile(hashUrl, hashPath);

                    if (isVirtual)
                    {
                        var resPath = Minecraft.AssetLegacy + item.Key;

                        if (!File.Exists(resPath))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(resPath));
                            File.Copy(hashPath, resPath, true);
                        }
                    }

                    if (mapResource)
                    {
                        var resPath = Minecraft.Resource + item.Key;

                        if (!File.Exists(resPath))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(resPath));
                            File.Copy(hashPath, resPath, true);
                        }
                    }

                    l(MFile.Resource, profile.AssetId, count, ++i);
                }
            }
        }

        /// <summary>
        /// Download client jar
        /// </summary>
        public void DownloadMinecraft()
        {
            if (profile.ClientDownloadUrl == "") return;

            l(MFile.Minecraft, profile.Jar, 1, 0);

            string id = profile.Jar;
            var path = Minecraft.Versions + id + "\\" + id + ".jar";
            if (!CheckFileValidation(path, profile.ClientHash))
            {
                Directory.CreateDirectory(Minecraft.Versions + id); 
                web.DownloadFile(profile.ClientDownloadUrl, path);
            }

            l(MFile.Minecraft, profile.Id, 1, 1);
        }

        private void l(MFile file, string name, int max, int value)
        {
            var e = new DownloadFileChangedEventArgs()
            {
                FileKind = file,
                FileName = name,
                TotalFileCount = max,
                ProgressedFileCount = value
            };
            ChangeFile?.Invoke(e);
        }

        private void Web_DownloadProgressChangedEvent(object sender, ProgressChangedEventArgs e)
        {
            ChangeProgress?.Invoke(this, e);
        }

        private bool CheckFileValidation(string path, string hash)
        {
            return File.Exists(path) && CheckSHA1(path, hash);
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
    }
}
