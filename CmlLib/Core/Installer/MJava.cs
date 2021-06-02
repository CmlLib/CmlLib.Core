using CmlLib.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace CmlLib.Core.Installer
{
    public class MJava
    {
        public static readonly string DefaultRuntimeDirectory
            = Path.Combine(MinecraftPath.GetOSDefaultPath(), "runtime");

        public event ProgressChangedEventHandler ProgressChanged;
        public string RuntimeDirectory { get; private set; }

        private IProgress<ProgressChangedEventArgs> pProgressChanged;

        public MJava() : this(DefaultRuntimeDirectory) { }

        public MJava(string runtimePath)
        {
            RuntimeDirectory = runtimePath;
        }

        public string GetDefaultBinaryName()
        {
            string binaryName = "java";
            if (MRule.OSName == MRule.Windows)
                binaryName = "javaw.exe";
            return binaryName;
        }

        public string CheckJava()
        {
            string binaryName = GetDefaultBinaryName();
            return CheckJava(binaryName);
        }

        public string CheckJava(string binaryName)
        {
            pProgressChanged = new Progress<ProgressChangedEventArgs>(
                (e) => ProgressChanged?.Invoke(this, e));

            string javapath = Path.Combine(RuntimeDirectory, "bin", binaryName);

            if (!File.Exists(javapath))
            {
                string javaUrl = getJavaUrl();
                string lzmaPath = downloadJavaLzma(javaUrl);

                decompressJavaFile(lzmaPath);

                if (!File.Exists(javapath))
                    throw new WebException("failed to download");

                if (MRule.OSName != MRule.Windows)
                    IOUtil.Chmod(javapath, IOUtil.Chmod755);
            }

            return javapath;
        }

        public Task<string> CheckJavaAsync()
            => CheckJavaAsync(null);

        public Task<string> CheckJavaAsync(IProgress<ProgressChangedEventArgs> progress)
        {
            string binaryName = GetDefaultBinaryName();
            return CheckJavaAsync(binaryName, progress);
        }

        public async Task<string> CheckJavaAsync(string binaryName, IProgress<ProgressChangedEventArgs> progress)
        {
            string javapath = Path.Combine(RuntimeDirectory, "bin", binaryName);

            if (!File.Exists(javapath))
            {
                if (progress == null)
                {
                    pProgressChanged = new Progress<ProgressChangedEventArgs>(
                        (e) => ProgressChanged?.Invoke(this, e));
                }
                else
                {
                    pProgressChanged = progress;
                }
                
                string javaUrl = await getJavaUrlAsync().ConfigureAwait(false);
                string lzmaPath = await downloadJavaLzmaAsync(javaUrl).ConfigureAwait(false);

                Task decompressTask = Task.Run(() => decompressJavaFile(lzmaPath));
                await decompressTask.ConfigureAwait(false);

                if (!File.Exists(javapath))
                    throw new WebException("failed to download");

                if (MRule.OSName != MRule.Windows)
                    IOUtil.Chmod(javapath, IOUtil.Chmod755);
            }

            return javapath;
        }

        private string getJavaUrl()
        {
            using (var wc = new WebClient())
            {
                string json = wc.DownloadString(MojangServer.LauncherMeta);
                return parseLauncherMetadata(json);
            }
        }

        private async Task<string> getJavaUrlAsync()
        {
            using (var wc = new WebClient())
            {
                string json = await wc.DownloadStringTaskAsync(MojangServer.LauncherMeta)
                    .ConfigureAwait(false);
                return parseLauncherMetadata(json);
            }
        }

        private string parseLauncherMetadata(string json)
        {
            var javaUrl = JObject.Parse(json)
                 [MRule.OSName]
                ?[MRule.Arch]
                ?["jre"]
                ?["url"]
                ?.ToString();

            if (string.IsNullOrEmpty(javaUrl))
                throw new PlatformNotSupportedException("Downloading JRE on current OS is not supported. Set JavaPath manually.");
            return javaUrl;
        }

        private string downloadJavaLzma(string javaUrl)
        {
            Directory.CreateDirectory(RuntimeDirectory);
            string lzmapath = Path.Combine(Path.GetTempPath(), "jre.lzma");

            var webdownloader = new WebDownload();
            webdownloader.DownloadProgressChangedEvent += Downloader_DownloadProgressChangedEvent;
            webdownloader.DownloadFile(javaUrl, lzmapath);

            return lzmapath;
        }

        private async Task<string> downloadJavaLzmaAsync(string javaUrl)
        {
            Directory.CreateDirectory(RuntimeDirectory);
            string lzmapath = Path.Combine(Path.GetTempPath(), "jre.lzma");

            using (var wc = new WebClient())
            {
                wc.DownloadProgressChanged += Downloader_DownloadProgressChangedEvent;
                await wc.DownloadFileTaskAsync(javaUrl, lzmapath)
                    .ConfigureAwait(false);
            }

            return lzmapath;
        }

        private void decompressJavaFile(string lzmaPath)
        {
            string zippath = Path.Combine(Path.GetTempPath(), "jre.zip");
            SevenZipWrapper.DecompressFileLZMA(lzmaPath, zippath);

            var z = new SharpZip(zippath);
            z.ProgressEvent += Z_ProgressEvent;
            z.Unzip(RuntimeDirectory);
        }

        private void Z_ProgressEvent(object sender, int e)
        {
            pProgressChanged.Report(new ProgressChangedEventArgs(50 + e / 2, null));
        }

        private void Downloader_DownloadProgressChangedEvent(object sender, ProgressChangedEventArgs e)
        { 
            pProgressChanged.Report(new ProgressChangedEventArgs(e.ProgressPercentage / 2, null));
        }
    }
}
