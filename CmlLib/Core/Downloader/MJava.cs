using CmlLib.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;

namespace CmlLib.Core.Downloader
{
    public class MJava
    {
        public static string DefaultRuntimeDirectory = Path.Combine(MinecraftPath.GetOSDefaultPath(), "runtime");

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

                var javaUrl = "";
                using (var wc = new WebClient())
                {
                    json = wc.DownloadString(MojangServer.LauncherMeta);

                    var job = JObject.Parse(json)[MRule.OSName];
                    javaUrl = job[MRule.Arch]?["jre"]?["url"]?.ToString();

                    if (string.IsNullOrEmpty(javaUrl))
                        throw new PlatformNotSupportedException("Downloading JRE on current OS is not supported. Set JavaPath manually.");

                    Directory.CreateDirectory(RuntimeDirectory);
                }

                var lzmapath = Path.Combine(Path.GetTempPath(), "jre.lzma");
                var zippath = Path.Combine(Path.GetTempPath(), "jre.zip");

                var webdownloader = new WebDownload();
                webdownloader.DownloadProgressChangedEvent += Downloader_DownloadProgressChangedEvent;
                webdownloader.DownloadFile(javaUrl, lzmapath);

                DownloadCompleted?.Invoke(this, new EventArgs());

                SevenZipWrapper.DecompressFileLZMA(lzmapath, zippath);

                var z = new SharpZip(zippath);
                z.ProgressEvent += Z_ProgressEvent;
                z.Unzip(RuntimeDirectory);

                if (!File.Exists(javapath))
                    throw new Exception("Failed Download");

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

        private void Downloader_DownloadProgressChangedEvent(object sender, ProgressChangedEventArgs e)
        {
            ProgressChanged?.Invoke(this, e);
        }
    }
}
