using System;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using CmlLib.Utils;

namespace CmlLib.Core
{
    public class MJava
    {
        public static string DefaultRuntimeDirectory = Path.Combine(Minecraft.GetOSDefaultPath(), "runtime");

        public event ProgressChangedEventHandler ProgressChanged;
        public event EventHandler DownloadCompleted;
        public string RuntimeDirectory { get; private set; }

        public MJava() : this(DefaultRuntimeDirectory) { }

        public MJava(string runtimePath)
        {
            RuntimeDirectory = runtimePath;
        }

        public string CheckJava()
        {
            var binaryName = "java";
            if (MRule.OSName == "windows")
                binaryName = "javaw.exe";

            return CheckJava(binaryName);
        }

        public string CheckJava(string binaryName)
        {
            var javapath = Path.Combine(RuntimeDirectory, "bin", binaryName);

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
                    json = wc.DownloadString(MojangServer.LauncherMeta);

                    var job = JObject.Parse(json)[MRule.OSName];
                    javaUrl = job[MRule.Arch]?["jre"]?["url"]?.ToString();

                    if (string.IsNullOrEmpty(javaUrl))
                        throw new Exception("unsupport os");

                    Directory.CreateDirectory(RuntimeDirectory);
                }

                var downloader = new WebDownload();
                downloader.DownloadProgressChangedEvent += Downloader_DownloadProgressChangedEvent;
                downloader.DownloadFile(javaUrl, Path.Combine(WorkingPath, "javatemp.lzma"));
                DownloadCompleted?.Invoke(this, new EventArgs());

                var lzma = Path.Combine(WorkingPath, "javatemp.lzma");
                var zip = Path.Combine(WorkingPath, "javatemp.zip");

                var szip = new SevenZipWrapper();
                szip.ProgressChange += Szip_ProgressChange;
                szip.DecompressFileLZMA(lzma, zip);

                var z = new SharpZip(zip);
                z.ProgressEvent += Z_ProgressEvent;
                z.Unzip(RuntimeDirectory);

                if (!File.Exists(javapath))
                {
                    IOUtil.DeleteDirectory(WorkingPath);
                    throw new Exception("Failed Download");
                }

                if (MRule.OSName != "windows")
                    IOUtil.Chmod(javapath, IOUtil.Chmod755);
            }

            return javapath;
        }

        private void Z_ProgressEvent(object sender, int e)
        {
            ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(e, null));
        }

        private void Szip_ProgressChange(object sender, ProgressChangedEventArgs e)
        {
            ProgressChanged?.Invoke(this, e);
        }

        private void Downloader_DownloadProgressChangedEvent(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            ProgressChanged?.Invoke(this, e);
        }
    }
}
