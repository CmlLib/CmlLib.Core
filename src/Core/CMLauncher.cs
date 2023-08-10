using CmlLib.Core.Downloader;
using CmlLib.Core.FileExtractors;
using CmlLib.Core.Java;
using CmlLib.Core.Rules;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using CmlLib.Core.VersionMetadata;
using System.ComponentModel;

namespace CmlLib.Core;

public class CMLauncher
{
    private readonly HttpClient _httpClient;

    public CMLauncher(string path, HttpClient httpClient) : this(new MinecraftPath(path), httpClient, new LauncherOSRule())
    {
    }

    public CMLauncher(MinecraftPath path, HttpClient httpClient, LauncherOSRule os)
    {
        _httpClient = httpClient;

        this.os = os;
        MinecraftPath = path;
        FileDownloader = new AsyncParallelDownloader(_httpClient);
        VersionLoader = new VersionLoaderCollection
        {
            new LocalVersionLoader(MinecraftPath),
            new MojangVersionLoader(_httpClient),
        };

        pFileChanged = new Progress<DownloadFileChangedEventArgs>(
            e => FileChanged?.Invoke(e));
        pProgressChanged = new Progress<ProgressChangedEventArgs>(
            e => ProgressChanged?.Invoke(this, e));

        JavaPathResolver = new MinecraftJavaPathResolver(path);
        GameFileCheckers = FileExtractorCollection.CreateDefault(_httpClient, JavaPathResolver, new RulesEvaluator(), new RulesEvaluatorContext(os));
    }

    public event DownloadFileChangedHandler? FileChanged;
    public event ProgressChangedEventHandler? ProgressChanged;
    
    private readonly IProgress<DownloadFileChangedEventArgs> pFileChanged;
    private readonly IProgress<ProgressChangedEventArgs> pProgressChanged;

    public readonly LauncherOSRule os;

    public MinecraftPath MinecraftPath { get; private set; }
    public VersionCollection? Versions { get; private set; }
    public IVersionLoader VersionLoader { get; set; }
    
    public FileExtractorCollection GameFileCheckers { get; private set; }
    public IDownloader? FileDownloader { get; set; }

    public IJavaPathResolver JavaPathResolver { get; set; }

    public async Task<VersionCollection> GetAllVersionsAsync()
    {
        Versions = await VersionLoader.GetVersionMetadatasAsync()
            .ConfigureAwait(false);
        return Versions;
    }

    public async Task<IVersion> GetVersionAsync(string versionName)
    {
        if (Versions == null)
            await GetAllVersionsAsync().ConfigureAwait(false);

        var version = await Versions!.GetAndSaveVersionAsync(versionName, MinecraftPath)
            .ConfigureAwait(false);
        return version;
    }

    private void checkLaunchOption(MLaunchOption option)
    {
        if (option.Path == null)
            option.Path = MinecraftPath;
    }

    public string? GetJavaPath(IVersion version)
    {
        if (!version.JavaVersion.HasValue || string.IsNullOrEmpty(version.JavaVersion?.Component))
            return null;
        
        return JavaPathResolver.GetJavaBinaryPath(version.JavaVersion.Value, os);
    }

    public string? GetDefaultJavaPath() => JavaPathResolver.GetDefaultJavaBinaryPath(os);
}
