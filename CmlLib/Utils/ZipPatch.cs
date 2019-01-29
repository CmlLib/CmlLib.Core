using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Net;
using Ionic.Zip;

namespace CmlLib.Utils
{
    public class ZipPatch
    {
        public event DownloadProgressChangedEventHandler DownloadProgressChanged;

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
            if (Directory.Exists(tmpZipPath))
                Directory.Delete(tmpZipPath, true);
            Directory.CreateDirectory(tmpZipPath);

            //압축파일 저장 경로
            var path = tmpZipPath + @"\tmp.zip";

            try
            {
                var dir = new DirectoryInfo(PatchPath);
                foreach (var item in dir.GetDirectories())
                {
                    if (!noupdate.Contains(item.Name))
                    {
                        DeleteDirectory(item.FullName);
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

            using (var zip = ZipFile.Read(path))
            {
                zip.ExtractAll(PatchPath, ExtractExistingFileAction.OverwriteSilently);
            }

            File.WriteAllText(LocalVerPath, webver);
        }

        private static void DeleteDirectory(string target_dir)
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
