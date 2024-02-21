using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using CmlLib.Core.Java;
using CmlLib.Utils;
using Newtonsoft.Json.Linq;

namespace CmlLib.Core.Installer;

// legacy java installer
// new java installer: CmlLib.Core.Files.JavaChecker
public class MJava
{
    public static readonly string DefaultRuntimeDirectory
        = Path.Combine(MinecraftPath.GetOSDefaultPath(), "runtime");

    private IProgress<ProgressChangedEventArgs>? pProgressChanged;

    public MJava() : this(DefaultRuntimeDirectory)
    {
    }

    public MJava(string runtimePath)
    {
        RuntimeDirectory = runtimePath;
        JavaPathResolver = new MinecraftJavaPathResolver(runtimePath);
    }

    public string RuntimeDirectory { get; }

    public IJavaPathResolver JavaPathResolver { get; set; }

    public event ProgressChangedEventHandler? ProgressChanged;

    public string GetBinaryPath()
    {
        return JavaPathResolver.GetJavaBinaryPath(MinecraftJavaPathResolver.CmlLegacyVersionName, MRule.OSName);
    }

    public bool CheckJavaExistence()
    {
        return File.Exists(GetBinaryPath());
    }

    public string CheckJava()
    {
        pProgressChanged = new Progress<ProgressChangedEventArgs>(
            e => ProgressChanged?.Invoke(this, e));

        var javaPath = GetBinaryPath();

        if (!CheckJavaExistence())
        {
            var javaUrl = GetJavaUrl();
            var lzmaPath = downloadJavaLzma(javaUrl);

            decompressJavaFile(lzmaPath);

            if (!File.Exists(javaPath))
                throw new WebException("failed to download");

            if (MRule.OSName != MRule.Windows)
                NativeMethods.Chmod(javaPath, NativeMethods.Chmod755);
        }

        return javaPath;
    }

    public Task<string> CheckJavaAsync()
    {
        return CheckJavaAsync(null);
    }

    public async Task<string> CheckJavaAsync(IProgress<ProgressChangedEventArgs>? progress)
    {
        var javapath = GetBinaryPath();

        if (!CheckJavaExistence())
        {
            if (progress == null)
                pProgressChanged = new Progress<ProgressChangedEventArgs>(
                    e => ProgressChanged?.Invoke(this, e));
            else
                pProgressChanged = progress;

            var javaUrl = await GetJavaUrlAsync().ConfigureAwait(false);
            var lzmaPath = await downloadJavaLzmaAsync(javaUrl).ConfigureAwait(false);

            var decompressTask = Task.Run(() => decompressJavaFile(lzmaPath));
            await decompressTask.ConfigureAwait(false);

            if (!File.Exists(javapath))
                throw new WebException("failed to download");

            if (MRule.OSName != MRule.Windows)
                NativeMethods.Chmod(javapath, NativeMethods.Chmod755);
        }

        return javapath;
    }

    public string GetJavaUrl()
    {
        using (var wc = new WebClient())
        {
            var json = wc.DownloadString(MojangServer.LauncherMeta);
            return parseLauncherMetadata(json);
        }
    }

    public async Task<string> GetJavaUrlAsync()
    {
        using (var wc = new WebClient())
        {
            var json = await wc.DownloadStringTaskAsync(MojangServer.LauncherMeta)
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
            throw new PlatformNotSupportedException(
                "Downloading JRE on current OS is not supported. Set JavaPath manually.");
        return javaUrl;
    }

    private string downloadJavaLzma(string javaUrl)
    {
        Directory.CreateDirectory(RuntimeDirectory);
        var lzmapath = Path.Combine(Path.GetTempPath(), "jre.lzma");

        var webdownloader = new WebDownload();
        webdownloader.DownloadProgressChangedEvent += Downloader_DownloadProgressChangedEvent;
        webdownloader.DownloadFile(javaUrl, lzmapath);

        return lzmapath;
    }

    private async Task<string> downloadJavaLzmaAsync(string javaUrl)
    {
        Directory.CreateDirectory(RuntimeDirectory);
        var lzmapath = Path.Combine(Path.GetTempPath(), "jre.lzma");

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
        var zippath = Path.Combine(Path.GetTempPath(), "jre.zip");
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
