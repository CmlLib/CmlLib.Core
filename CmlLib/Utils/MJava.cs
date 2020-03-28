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
        public string RuntimeDirectory { get; private set; }

        public MJava() : this(DefaultRuntimeDirectory) { }

        public MJava(string runtimePath)
        {
            RuntimeDirectory = runtimePath;
        }

        public string CheckJava()
        {
            var javapath = Path.Combine(RuntimeDirectory, "bin", "java.exe");

            if (!File.Exists(javapath))
            {
                string json = "";

                var WorkingPath = Path.Combine(Path.GetTempPath(), "temp_download_runtime");

                if (Directory.Exists(WorkingPath))
                    IOUtil.DeleteDirectory(WorkingPath);
                Directory.CreateDirectory(WorkingPath);

                var javaUrl = "";
                using (var wc = new WebClient())
                {
                    json = wc.DownloadString("http://launchermeta.mojang.com/mc/launcher.json");

                    var job = JObject.Parse(json)[MRule.OSName];
                    javaUrl = job[MRule.Arch]["jre"]["url"].ToString();

                    Directory.CreateDirectory(RuntimeDirectory);
                }

                var downloader = new CmlLib.Launcher.WebDownload();
                downloader.DownloadProgressChangedEvent += Downloader_DownloadProgressChangedEvent;
                downloader.DownloadFile(javaUrl, Path.Combine(WorkingPath, "javatemp.lzma"));

                DownloadCompleted?.Invoke(this, new EventArgs());

                var lzma = Path.Combine(WorkingPath, "javatemp.lzma");
                var zip = Path.Combine(WorkingPath, "javatemp.zip");

                SevenZipWrapper.DecompressFileLZMA(lzma, zip);
                var z = new SharpZip(zip);
                z.Unzip(RuntimeDirectory);

                if (!File.Exists(javapath))
                {
                    IOUtil.DeleteDirectory(WorkingPath);
                    throw new Exception("Failed Download");
                }

            }

            return javapath;
        }

        private void Downloader_DownloadProgressChangedEvent(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            DownloadProgressChanged?.Invoke(this, e);
        }
    }
}
