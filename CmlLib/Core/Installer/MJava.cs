using CmlLib.Utils;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using CmlLib.Core.Java;
using System.Text.Json;
using System.Net;

namespace CmlLib.Core.Installer
{
    // legacy java installer
    // new java installer: CmlLib.Core.Files.JavaChecker
    public class MJava
    {
        public static readonly string DefaultRuntimeDirectory
            = Path.Combine(MinecraftPath.GetOSDefaultPath(), "runtime");

        public event ProgressChangedEventHandler? ProgressChanged;
        public string RuntimeDirectory { get; private set; }

        private IProgress<ProgressChangedEventArgs>? pProgressChanged;

        public IJavaPathResolver JavaPathResolver { get; set; }

        public MJava() : this(DefaultRuntimeDirectory) { }

        public MJava(string runtimePath)
        {
            RuntimeDirectory = runtimePath;
            JavaPathResolver = new MinecraftJavaPathResolver(runtimePath);
        }

        public string GetBinaryPath()
            => JavaPathResolver.GetJavaBinaryPath(MinecraftJavaPathResolver.CmlLegacyVersionName, MRule.OSName);

        public bool CheckJavaExistence()
            => File.Exists(GetBinaryPath());

        public Task<string> CheckJavaAsync()
            => CheckJavaAsync(null);
        
        public async Task<string> CheckJavaAsync(IProgress<ProgressChangedEventArgs>? progress)
        {
            string javapath = GetBinaryPath();

            if (!CheckJavaExistence())
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
                
                string javaUrl = await GetJavaUrlAsync().ConfigureAwait(false);
                string lzmaPath = await downloadJavaLzmaAsync(javaUrl).ConfigureAwait(false);

                Task decompressTask = Task.Run(() => decompressJavaFile(lzmaPath));
                await decompressTask.ConfigureAwait(false);

                if (!File.Exists(javapath))
                    throw new WebException("failed to download");

                if (MRule.OSName != MRule.Windows)
                    NativeMethods.Chmod(javapath, NativeMethods.Chmod755);
            }

            return javapath;
        }

        public async Task<string> GetJavaUrlAsync()
        {
            var json = await HttpUtil.HttpClient.GetStringAsync(MojangServer.LauncherMeta)
                .ConfigureAwait(false);
            return parseLauncherMetadata(json);
        }

        private string parseLauncherMetadata(string json)
        {
            using var jsonDocument = JsonDocument.Parse(json);
            var root = jsonDocument.RootElement;

            var javaUrl = root.SafeGetProperty(MRule.OSName)?
                .SafeGetProperty(MRule.Arch)?
                .SafeGetProperty("jre")?
                .GetPropertyValue("url");

            if (string.IsNullOrEmpty(javaUrl))
                throw new PlatformNotSupportedException("Downloading JRE on current OS is not supported. Set JavaPath manually.");
            return javaUrl;
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

        private void Z_ProgressEvent(object? sender, int e)
        {
            pProgressChanged?.Report(new ProgressChangedEventArgs(50 + e / 2, null));
        }

        private void Downloader_DownloadProgressChangedEvent(object? sender, ProgressChangedEventArgs e)
        { 
            pProgressChanged?.Report(new ProgressChangedEventArgs(e.ProgressPercentage / 2, null));
        }
    }
}
