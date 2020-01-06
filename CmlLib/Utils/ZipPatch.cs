using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Net;
using System.ComponentModel;

namespace CmlLib.Utils
{
    public class ZipPatch
    {
        public event ProgressChangedEventHandler DownloadProgressChanged;

        public string PatchVerUrl = "";
        public string PatchZipUrl = "";
        public string LocalVerPath = "";
        public string PatchPath = "";
        private string webver = "";
        private string[] noupdate = new string[] { "" };

        public ZipPatch(string verUrl, string zipUrl, string patchPath, string localPath, string[] noupdate)
        {
            DownloadProgressChanged += delegate { };
            PatchVerUrl = verUrl;
            PatchZipUrl = zipUrl;
            PatchPath = patchPath;
            LocalVerPath = localPath;
            this.noupdate = noupdate;
        }

        public bool CheckHasUpdate()
        {
            string localver = "";
            if (File.Exists(LocalVerPath))
                localver = File.ReadAllText(LocalVerPath, Encoding.UTF8);

            using (var wc = new WebClient())
            {
                webver = wc.DownloadString(PatchVerUrl);
            }

            return localver != webver;
        }

        public void Patch()
        {
            //랜덤 경로 생성
            var rnd = new Random().Next(1000000, 9999999).ToString();
            var tmpZipPath = Path.Combine(Path.GetTempPath(), rnd);

            //폴더 생성
            Directory.CreateDirectory(tmpZipPath);

            //압축파일 저장 경로
            var path = Path.Combine(tmpZipPath, "tmp.zip");

            try
            {
                var dir = new DirectoryInfo(PatchPath);
                foreach (var item in dir.GetDirectories())
                {
                    if (!noupdate.Contains(item.Name))
                    {
                        IOUtil.DeleteDirectory(item.FullName);
                    }

                }
            }
            catch { }

            bool iscom = false;
            using (var wc = new WebClient())
            {
                wc.DownloadProgressChanged += (sender, e) =>
                {
                    DownloadProgressChanged(sender, e);
                };
                wc.DownloadFileCompleted += delegate
                {
                    iscom = true;
                };
                wc.DownloadFileAsync(new Uri(PatchZipUrl), path);
            }

            while (!iscom) Thread.Sleep(100);

            Directory.CreateDirectory(PatchPath);

            var z = new SharpZip(path);
            z.ProgressEvent += (object s, int p) =>
            {
                DownloadProgressChanged?.Invoke(this, new ProgressChangedEventArgs(p, null));
            };
            z.Unzip(PatchPath);

            File.WriteAllText(LocalVerPath, webver);
        }
    }
}
