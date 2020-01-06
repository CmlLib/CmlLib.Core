using System;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using CmlLib.Launcher;

namespace CmlLib.Utils
{
    public class MJava
    {
        public static string DefaultRuntimeDirectory = Path.Combine(Minecraft.GetOSDefaultPath(), "runtime");

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
            return File.Exists(Path.Combine(RuntimeDirectory, "bin", "java.exe"));
        }

        public bool CheckJavaw()
        {
            return File.Exists(Path.Combine(RuntimeDirectory, "bin", "javaw.exe"));
        }

        string WorkingPath;
        public void DownloadJavaAsync()
        {
            string json = "";

            WorkingPath = Path.Combine(Path.GetTempPath(), "temp_download_runtime");

            if (Directory.Exists(WorkingPath))
                IOUtil.DeleteDirectory(WorkingPath);
            Directory.CreateDirectory(WorkingPath);

            using (var wc = new WebClient())
            {
                json = wc.DownloadString("http://launchermeta.mojang.com/mc/launcher.json");

                var job = JObject.Parse(json)["windows"];
                var url = job[Environment.Is64BitOperatingSystem ? "64" : "32"]["jre"]["url"].ToString();

                Directory.CreateDirectory(RuntimeDirectory);
                wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
                wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                wc.DownloadFileAsync(new Uri(url), Path.Combine(WorkingPath, "javatemp.lzma"));
            }
        }

        public async Task DownloadJavaTaskAsync()
        {
            string json = "";

            WorkingPath = Path.Combine(Path.GetTempPath(), "temp_download_runtime");

            if (Directory.Exists(WorkingPath))
                IOUtil.DeleteDirectory(WorkingPath);
            Directory.CreateDirectory(WorkingPath);

            using (var wc = new WebClient())
            {
                json = await wc.DownloadStringTaskAsync("http://launchermeta.mojang.com/mc/launcher.json");

                var job = JObject.Parse(json)["windows"];
                var url = job[Environment.Is64BitOperatingSystem ? "64" : "32"]["jre"]["url"].ToString();

                Directory.CreateDirectory(RuntimeDirectory);
                wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
                wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                await wc.DownloadFileTaskAsync(new Uri(url), Path.Combine(WorkingPath, "javatemp.lzma"));
            }
        }

        public void DownloadJava()
        {
            string json = "";

            WorkingPath = Path.Combine(Path.GetTempPath(), "temp_download_runtime");

            if (Directory.Exists(WorkingPath))
                IOUtil.DeleteDirectory(WorkingPath);
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
            downloader.DownloadFile(javaUrl, Path.Combine(WorkingPath, "javatemp.lzma"));

            DownloadComplete();
        }

        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            DownloadComplete();
        }

        private void DownloadComplete()
        {
            DownloadCompleted?.Invoke(this, new EventArgs());

            var lzma = Path.Combine(WorkingPath, "javatemp.lzma");
            var zip = Path.Combine(WorkingPath, "javatemp.zip");

            SevenZipWrapper.DecompressFileLZMA(lzma, zip);
            var z = new SharpZip(zip);
            z.Unzip(RuntimeDirectory);

            if (!CheckJavaw())
            {
                IOUtil.DeleteDirectory(WorkingPath);
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
    }
}
