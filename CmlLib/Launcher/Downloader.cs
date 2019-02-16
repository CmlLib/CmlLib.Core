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

    /// <summary>
    /// 게임 실행에 필요한 라이브러리, 게임, 리소스 등을 다운로드합니다.
    /// </summary>
    public class MDownloader
    {
        public event DownloadFileChangedHandler ChangeFile;
        public event ProgressChangedEventHandler ChangeProgress;

        public bool CheckHash { get; set; } = true;

        MProfile profile;
        WebDownload web;

        /// <summary>
        /// 실행에 필요한 파일들을 profile 에서 불러옵니다.
        /// </summary>
        /// <param name="_profile">불러올 프로파일</param>
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
        /// 실행에 필요한 라이브러리들을 프로파일에서 가져와 모두 다운로드합니다.
        /// </summary>
        public void DownloadLibraries()
        {
            int index = 0; // 현재 다운로드중인 파일의 순서 (이벤트 생성용)
            int maxCount = profile.Libraries.Count; // 모든 파일의 갯수
            foreach (var item in profile.Libraries) // 프로파일의 모든 라이브러리 반복
            {
                try
                {
                    if (CheckDownloadRequireLibrary(item)) // 파일이 존재하지 않을 때만
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(item.Path)); //파일 다운로드
                        web.DownloadFile(item.Url, item.Path);
                    } 
                }
                catch
                {
                }

                l(MFile.Library, item.Name, maxCount, ++index); // 이벤트 발생
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
        /// 다운로드 받아야 할 리소스 파일들이 저장된 인덱스 파일을 다운로드합니다.
        /// </summary>
        public void DownloadIndex()
        {
            string path = Minecraft.Index + profile.AssetId + ".json"; //로컬 인덱스파일의 경로

            if (profile.AssetUrl != "" && !CheckFileValidation(path, profile.AssetHash))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)); //폴더생성

                using (var wc = new WebClient())
                {
                    wc.DownloadFile(profile.AssetUrl, path);
                }
            }
        }

        /// <summary>
        /// BGM, 효과음, 언어팩 등 리소스파일들을 다운로드합니다.
        /// </summary>
        public void DownloadResource()
        {
            var indexpath = Minecraft.Index + profile.AssetId + ".json";
            if (!File.Exists(indexpath)) return;

            using (var wc = new WebClient())
            {
                bool Isvirtual = false;

                var json = File.ReadAllText(indexpath);
                var index = JObject.Parse(json);

                if ((index["virtual"]?.ToString()?.ToLower()) == "true") //virtual 이 true 인지 확인
                    Isvirtual = true;

                var list = (JObject)index["objects"]; //리소스 리스트를 생성
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
                        wc.DownloadFile(hashUrl, hashPath); //다운로드

                    if (Isvirtual) //virtual 이 true 이고 파일이 없을떄
                    {
                        var resPath = Minecraft.AssetLegacy + item.Key;

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
        /// 마인크래프트를 다운로드합니다.
        /// </summary>
        public void DownloadMinecraft()
        {
            if (profile.ClientDownloadUrl == "") return;

            l(MFile.Minecraft, profile.Id, 1, 0);

            string id = profile.Id;
            var path = Minecraft.Versions + id + "\\" + id + ".jar";
            if (!CheckFileValidation(path, profile.ClientHash))
            {
                Directory.CreateDirectory(Minecraft.Versions + id); //폴더생성
                web.DownloadFile(profile.ClientDownloadUrl, Minecraft.Versions + id + "\\" + id + ".jar");
            }

            l(MFile.Minecraft, profile.Id, 1, 1);
        }

        private void l(MFile file, string name, int max, int value)
        {
            var e = new DownloadFileChangedEventArgs()
            {
                FileKind = file,
                FileName = name,
                MaxValue = max,
                CurrentValue = value
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
