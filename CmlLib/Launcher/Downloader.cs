using System;
using System.IO.Compression;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace CmlLib.Launcher
{
    public delegate void MChangeDownloadProgress(ChangeProgressEventArgs e);

    /// <summary>
    /// 게임 실행에 필요한 라이브러리, 게임, 리소스 등을 다운로드합니다.
    /// </summary>
    public class MDownloader
    {
        public event MChangeDownloadProgress ChangeProgressEvent;
        public event DownloadProgressChangedEventHandler ChangeFileProgressEvent;

        MProfile profile;

        /// <summary>
        /// 실행에 필요한 파일들을 profile 에서 불러옵니다.
        /// </summary>
        /// <param name="_profile">불러올 프로파일</param>
        public MDownloader(MProfile _profile)
        {
            ChangeProgressEvent += delegate { };
            ChangeFileProgressEvent += delegate { };
            this.profile = _profile;
        }

        /// <summary>
        /// 실행에 필요한 라이브러리들을 프로파일에서 가져와 모두 다운로드합니다.
        /// </summary>
        public void DownloadLibraries()
        {
            using (var wc = new WebClient()) // 웹클라이언트 객체생성, 이벤트등록
            {
                wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
                wc.DownloadFileCompleted += wcd;

                int index = 0; // 현재 다운로드중인 파일의 순서 (이벤트 생성용)
                int maxCount = profile.Libraries.Count; // 모든 파일의 갯수
                foreach (var item in profile.Libraries) // 프로파일의 모든 라이브러리 반복
                {
                    try
                    {
                        l(MFile.Library, item.Name, maxCount, index); // 이벤트 발생
                        if (item.Path != "" && !File.Exists(item.Path)) // 파일이 존재하지 않을 때만
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(item.Path)); //파일 다운로드
                            d(wc, item.Url, item.Path);
                        }
                        index++;
                    }
                    catch { }
                }
            }
        }

        // 아래 코드는 비동기 코드를 동기적으로 실행하는 코드

        bool iscom = false;
        void d(WebClient wc, string a, string b)
        {
            if (a == null) return;

            iscom = false;
            wc.DownloadFileAsync(new Uri(a), b);
            while (!iscom)
            {
                Thread.Sleep(50);
            }
        }
        private void wcd(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            iscom = true;
        }

        ////////////////////////////////////////////////////

        /// <summary>
        /// 네이티브 라이브러리들의 압축을 해제해 랜덤 폴더에 저장합니다.
        /// </summary>
        public void ExtractNatives()
        {
            var ran = new Random();
            int random = ran.Next(10000, 99999); //랜덤숫자 생성
            string path = Minecraft.Versions + profile.Id + "\\natives-" + random.ToString(); //랜덤 숫자를 만들어 경로생성
            ExtractNatives(path);
        }

        /// <summary>
        /// 네이티브 라이브러리들을 설정한 경로에 압축을 해제해 저장합니다.
        /// </summary>
        /// <param name="_path">압축 풀 폴더의 경로</param>
        public void ExtractNatives(string path)
        {
            CleanNatives();

            Directory.CreateDirectory(path); //폴더생성

            foreach (var item in profile.Libraries) //네이티브 라이브러리 리스트를 foreach 로 하나씩 돌림
            {
                try
                {
                    if (item.IsNative)
                    {
                        ZipFile.ExtractToDirectory(item.Path, path); //압축풀기
                    }
                }
                catch { }
            }

            profile.NativePath = path;
        }

        /// <summary>
        /// 저장된 네이티브 라이브러리들을 모두 제거합니다.
        /// </summary>
        public void CleanNatives()
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(Minecraft.Versions + profile.Id);
                foreach (var item in di.GetDirectories("native*")) //native 라는 이름이 포함된 폴더를 모두 가져옴
                {
                    DeleteDirectory(item.FullName);
                }
            }
            catch { }
        }

        /// <summary>
        /// 다운로드 받아야 할 리소스 파일들이 저장된 인덱스 파일을 다운로드합니다.
        /// </summary>
        public void DownloadIndex()
        {
            string path = Minecraft.Index + profile.AssetId + ".json"; //로컬 인덱스파일의 경로

            if (!File.Exists(path)) //로컬에 없을때
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)); //폴더생성
                using (var wc = new WebClient())
                {
                    wc.DownloadFile(profile.AssetUrl, path); //파일 다운로드
                }
            }
        }

        /// <summary>
        /// BGM, 효과음, 언어팩 등 리소스파일들을 다운로드합니다.
        /// </summary>
        public void DownloadResource()
        {
            using (var wc = new WebClient())
            {
                bool Isvirtual = false;

                var json = File.ReadAllText(Minecraft.Index + profile.AssetId + ".json");
                var index = JObject.Parse(json);

                try
                {
                    if (index["virtual"].ToString().ToLower() == "true") //virtual 이 true 인지 확인
                        Isvirtual = true;
                }
                catch { }

                var list = (JObject)index["objects"]; //리소스 리스트를 생성 ('objects' 오브젝트)

                int pi = 0;
                foreach (var item in list)
                {
                    pi++;
                    l(MFile.Resource,"", list.Count, pi);
                    JObject job = (JObject)item.Value;
                    string path = job["hash"].ToString()[0].ToString() + job["hash"].ToString()[1].ToString() + "/" + job["hash"].ToString(); //리소스 경로를 설정 ex) a9\a9ea생략85ad93d
                    string hashpath = (Minecraft.Assets + "objects\\" + path).Replace("/", "\\"); //해쉬 리소스 경로 설정
                    string filepath = (Minecraft.Assets + "virtual\\legacy\\" + item.Key).Replace("/", "\\"); //legacy 폴더에 저장할 리소스경로 설정
                    Directory.CreateDirectory(Path.GetDirectoryName(hashpath)); //폴더생성

                    if (!File.Exists(hashpath)) //해쉬 리소스 경로에 파일이 없을때
                    {
                        wc.DownloadFile("http://resources.download.minecraft.net/" + path, hashpath); //다운로드
                    }

                    if (Isvirtual && !File.Exists(filepath)) //virtual 이 true 이고 파일이 없을떄
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(filepath));
                        File.Copy(hashpath, filepath, true); //다운로드
                    }
                }
            }
        }

        bool iscomp = false;

        /// <summary>
        /// 마인크래프트를 다운로드합니다.
        /// </summary>
        public void DownloadMinecraft()
        {
            if (profile.ClientDownloadUrl == "") return;

            string id = profile.Id;
            if (!File.Exists(Minecraft.Versions + id + "\\" + id + ".jar")) //파일이 없을때
            {
                Directory.CreateDirectory(Minecraft.Versions + id); //폴더생성
                using (var wc = new WebClient())
                {
                    iscomp = false;
                    wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
                    wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                    wc.DownloadFileAsync(new Uri(profile.ClientDownloadUrl), Minecraft.Versions + id + "\\" + id + ".jar");

                    while (!iscomp)
                    {
                        Thread.Sleep(100);
                    }
                }
            }
        }

        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            iscomp = true;
        }

        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ChangeFileProgressEvent(sender, e);
        }

        private void l(MFile filetype , string filename, int max, int value)
        {
            try
            {
                ChangeProgressEvent(new ChangeProgressEventArgs() {
                    FileKind = filetype,
                    FileName = filename,
                    MaxValue = max,
                    CurrentValue = value
                });
            } catch { }
        }

        private void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, true);
        }
    }
}
