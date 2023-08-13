using System.ComponentModel;
using CmlLib.Core.Java;
using System.Text.Json;
using System.Net;
using CmlLib.Core.Downloader;
using CmlLib.Core.Rules;
using CmlLib.Core.Internals;
using CmlLib.Core.Tasks;

namespace CmlLib.Core.Installer;

// legacy java installer
// new java installer: CmlLib.Core.Files.JavaChecker
public class MJava
{
    public static readonly string DefaultRuntimeDirectory
        = Path.Combine(MinecraftPath.GetOSDefaultPath(), "runtime");

    public event ProgressChangedEventHandler? ProgressChanged;
    public string RuntimeDirectory { get; private set; }

    private readonly HttpClient _httpClient;
    private IProgress<ProgressChangedEventArgs>? pProgressChanged;
    public IJavaPathResolver JavaPathResolver { get; set; }

    public MJava(HttpClient client) : this(client, DefaultRuntimeDirectory) { }

    public MJava(HttpClient client, string runtimePath)
    {
        RuntimeDirectory = runtimePath;
        JavaPathResolver = new MinecraftJavaPathResolver(runtimePath);
        _httpClient = client;
    }

    public string GetBinaryPath(LauncherOSRule os)
        => JavaPathResolver.GetJavaBinaryPath(
            MinecraftJavaPathResolver.CmlLegacyVersion, 
            os);

    public bool CheckJavaExistence(LauncherOSRule os)
        => File.Exists(GetBinaryPath(os));

    public Task<string> CheckJavaAsync(LauncherOSRule os)
        => CheckJavaAsync(os, null);
    
    public async Task<string> CheckJavaAsync(LauncherOSRule os, IProgress<ProgressChangedEventArgs>? progress)
    {
        string javapath = GetBinaryPath(os);

        if (!CheckJavaExistence(os))
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

            if (LauncherOSRule.Current.Name != LauncherOSRule.Windows)
                NativeMethods.Chmod(javapath, NativeMethods.Chmod755);
        }

        return javapath;
    }

    public async Task<string> GetJavaUrlAsync()
    {
        var json = await _httpClient.GetStringAsync(MojangServer.LauncherMeta)
            .ConfigureAwait(false);
        return parseLauncherMetadata(json);
    }

    private string parseLauncherMetadata(string json)
    {
        using var jsonDocument = JsonDocument.Parse(json);
        var root = jsonDocument.RootElement;

        var javaUrl = root
            .GetPropertyOrNull(LauncherOSRule.Current.Name)?
            .GetPropertyOrNull(LauncherOSRule.Current.Arch)?
            .GetPropertyOrNull("jre")?
            .GetPropertyValue("url");

        if (string.IsNullOrEmpty(javaUrl))
            throw new PlatformNotSupportedException("Downloading JRE on current OS is not supported. Set JavaPath manually.");
        return javaUrl;
    }

    private async Task<string> downloadJavaLzmaAsync(string javaUrl)
    {
        Directory.CreateDirectory(RuntimeDirectory);
        string lzmaPath = Path.Combine(Path.GetTempPath(), "jre.lzma");

        var progress = new Progress<ByteProgressEventArgs>(p =>
        {
            var percent = (float)p.ProgressedBytes / p.TotalBytes * 100;
            pProgressChanged?.Report(new ProgressChangedEventArgs((int)percent / 2, null));
        });
        await HttpClientDownloadHelper.DownloadFileAsync(_httpClient, javaUrl, 0, lzmaPath, progress);
        return lzmaPath;
    }

    private void decompressJavaFile(string lzmaPath)
    {
        string zippath = Path.Combine(Path.GetTempPath(), "jre.zip");
        SevenZipWrapper.DecompressFileLZMA(lzmaPath, zippath);

        SharpZipWrapper.Unzip(zippath, RuntimeDirectory, new Progress<ByteProgressEventArgs>(p => 
        {
            var percent = (float)p.ProgressedBytes / p.TotalBytes * 100;
            pProgressChanged?.Report(new ProgressChangedEventArgs(50 + (int)percent / 2, null));
        }));
    }
}