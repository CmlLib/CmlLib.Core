using System;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Ionic.Zip;
using System.ComponentModel;

namespace CmlLib.Utils
{
    public class MJava
    {
        public static string DefaultRuntimeDirectory = CmlLib.Launcher.Minecraft.DefaultPath + "\\runtime";

        public event ProgressChangedEventHandler DownloadProgressChanged;
        public event EventHandler DownloadCompleted;
        public event EventHandler UnzipCompleted;
        public string RuntimeDirectory { get; private set; }

        public MJava() : this(DefaultRuntimeDirectory) { }

        public MJava(string runtimePath)
        {
            RuntimeDirectory = runtimePath;
        }

        public bool CheckJava()
        {
            return File.Exists(RuntimeDirectory + "\\bin\\java.exe");
        }

        public bool CheckJavaw()
        {
            return File.Exists(RuntimeDirectory + "\\bin\\javaw.exe");
        }

        string WorkingPath;
        public void DownloadJavaAsync()
        {
            string json = "";

            WorkingPath = Path.GetTempPath() + "\\temp_download_runtime";

            if (Directory.Exists(WorkingPath))
                DeleteDirectory(WorkingPath);
            Directory.CreateDirectory(WorkingPath);

            using (var wc = new WebClient())
            {
                json = wc.DownloadString("http://launchermeta.mojang.com/mc/launcher.json");

                var job = JObject.Parse(json)["windows"];
                var url = job[Environment.Is64BitOperatingSystem ? "64" : "32"]["jre"]["url"].ToString();

                Directory.CreateDirectory(RuntimeDirectory);
                wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
                wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                wc.DownloadFileAsync(new Uri(url), WorkingPath + "\\javatemp.lzma");
            }
        }

        public async Task DownloadJavaTaskAsync()
        {
            string json = "";

            WorkingPath = Path.GetTempPath() + "\\temp_download_runtime";

            if (Directory.Exists(WorkingPath))
                DeleteDirectory(WorkingPath);
            Directory.CreateDirectory(WorkingPath);

            using (var wc = new WebClient())
            {
                json = await wc.DownloadStringTaskAsync("http://launchermeta.mojang.com/mc/launcher.json");

                var job = JObject.Parse(json)["windows"];
                var url = job[Environment.Is64BitOperatingSystem ? "64" : "32"]["jre"]["url"].ToString();

                Directory.CreateDirectory(RuntimeDirectory);
                wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
                wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                await wc.DownloadFileTaskAsync(new Uri(url), WorkingPath + "\\javatemp.lzma");
            }
        }

        public void DownloadJava()
        {
            string json = "";

            WorkingPath = Path.GetTempPath() + "\\temp_download_runtime";

            if (Directory.Exists(WorkingPath))
                DeleteDirectory(WorkingPath);
            Directory.CreateDirectory(WorkingPath);

            var javaUrl = "";
            using (var wc = new WebClient())
            {
                json = wc.DownloadString("http://launchermeta.mojang.com/mc/launcher.json");

                var job = JObject.Parse(json)["windows"];
                javaUrl = job[Environment.Is64BitOperatingSystem ? "64" : "32"]["jre"]["url"].ToString();

                Directory.CreateDirectory(RuntimeDirectory);
            }

            var downloader = new CmlLib.Launcher.WebDownload();
            downloader.DownloadProgressChangedEvent += Downloader_DownloadProgressChangedEvent;
            downloader.DownloadFile(javaUrl, WorkingPath + "\\javatemp.lzma");

            DownloadComplete();
        }

        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            DownloadComplete();
        }

        private void DownloadComplete()
        {
            DownloadCompleted?.Invoke(this, new EventArgs());

            var lzma = WorkingPath + "\\javatemp.lzma";
            var zip = WorkingPath + "\\javatemp.zip";

            SevenZipWrapper.DecompressFileLZMA(lzma, zip);
            using (var extractor = ZipFile.Read(zip))
            {
                extractor.ExtractAll(RuntimeDirectory, ExtractExistingFileAction.OverwriteSilently);
            }

            if (!File.Exists(RuntimeDirectory + "\\bin\\javaw.exe"))
            {
                try
                {
                    DeleteDirectory(RuntimeDirectory);
                }
                catch { }
                throw new Exception("Failed Download");
            }

            UnzipCompleted?.Invoke(this, new EventArgs());
        }

        private void Downloader_DownloadProgressChangedEvent(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            DownloadProgressChanged?.Invoke(this, e);
        }

        private void Wc_DownloadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DownloadProgressChanged?.Invoke(this, e);
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
