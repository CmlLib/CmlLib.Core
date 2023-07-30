using CmlLib.Utils;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using CmlLib.Core.Java;
using System.Text.Json;
using System.Net;
using System.Net.Http;
using CmlLib.Core.Downloader;

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

        private readonly HttpClient httpClient;
        private IProgress<ProgressChangedEventArgs>? pProgressChanged;
        public IJavaPathResolver JavaPathResolver { get; set; }

        public MJava() : this(HttpUtil.HttpClient) { }
        public MJava(HttpClient client) : this(client, DefaultRuntimeDirectory) { }
        public MJava(string runtimePath) : this(HttpUtil.HttpClient, runtimePath) { }

        public MJava(HttpClient client, string runtimePath)
        {
            RuntimeDirectory = runtimePath;
            JavaPathResolver = new MinecraftJavaPathResolver(runtimePath);
            httpClient = client;
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
            string lzmaPath = Path.Combine(Path.GetTempPath(), "jre.lzma");

            var downloader = new HttpClientDownloadHelper(httpClient);
            var progress = new Progress<DownloadFileByteProgress>(p =>
            {
                var percent = (float)p.ProgressedBytes / p.TotalBytes * 100;
                pProgressChanged?.Report(new ProgressChangedEventArgs((int)percent / 2, null));
            });
            await downloader.DownloadFileAsync(new DownloadFile(lzmaPath, javaUrl), progress);

            return lzmaPath;
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
    }
}