using CmlLib.Core.Downloader;
using CmlLib.Core.FileChecker;
using CmlLib.Core.Java;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using CmlLib.Core.VersionMetadata;
using System.ComponentModel;
using System.Diagnostics;

namespace CmlLib.Core;

public class CMLauncher
{
    private readonly HttpClient _httpClient;

    public CMLauncher(string path, HttpClient httpClient) : this(new MinecraftPath(path), httpClient)
    {
    }

    public CMLauncher(MinecraftPath path, HttpClient httpClient)
    {
        _httpClient = httpClient;

        MinecraftPath = path;
        GameFileCheckers = FileCheckerCollection.CreateDefault(_httpClient);
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

        // to prevent null-reference warning, set both field, property
        JavaPathResolver = new MinecraftJavaPathResolver(path);
    }

    public event DownloadFileChangedHandler? FileChanged;
    public event ProgressChangedEventHandler? ProgressChanged;
    
    private readonly IProgress<DownloadFileChangedEventArgs> pFileChanged;
    private readonly IProgress<ProgressChangedEventArgs> pProgressChanged;

    public MinecraftPath MinecraftPath { get; private set; }
    public MVersionCollection? Versions { get; private set; }
    public IVersionLoader VersionLoader { get; set; }
    
    public FileCheckerCollection GameFileCheckers { get; private set; }
    public IDownloader? FileDownloader { get; set; }

    public IJavaPathResolver JavaPathResolver { get; set; }

    public async Task<MVersionCollection> GetAllVersionsAsync()
    {
        Versions = await VersionLoader.GetVersionMetadatasAsync()
            .ConfigureAwait(false);
        return Versions;
    }

    public async Task<MVersion> GetVersionAsync(string versionName)
    {
        if (Versions == null)
            await GetAllVersionsAsync().ConfigureAwait(false);

        var version = await Versions!.GetVersionAsync(versionName)
            .ConfigureAwait(false);
        return version;
    }

    public DownloadFile[] CheckLostGameFiles(MVersion version)
    {
        var lostFiles = new List<DownloadFile>();
        foreach (IFileChecker checker in this.GameFileCheckers)
        {
            DownloadFile[]? files = checker.CheckFiles(MinecraftPath, version, pFileChanged);
            if (files != null)
                lostFiles.AddRange(files);
        }

        return lostFiles.ToArray();
    }

    public async Task<DownloadFile[]> CheckLostGameFilesTaskAsync(MVersion version)
    {
        var lostFiles = new List<DownloadFile>();
        foreach (IFileChecker checker in this.GameFileCheckers)
        {
            DownloadFile[]? files = await checker.CheckFilesTaskAsync(MinecraftPath, version, pFileChanged)
                .ConfigureAwait(false);
            if (files != null)
                lostFiles.AddRange(files);
        }

        return lostFiles.ToArray();
    }

    public async Task DownloadGameFiles(DownloadFile[] files)
    {
        if (this.FileDownloader == null)
            return;

        await FileDownloader.DownloadFiles(files, pFileChanged, pProgressChanged)
            .ConfigureAwait(false);
    }

    public void CheckAndDownload(MVersion version)
    {
        foreach (var checker in this.GameFileCheckers)
        {
            DownloadFile[]? files = checker.CheckFiles(MinecraftPath, version, pFileChanged);

            if (files == null || files.Length == 0)
                continue;

            DownloadGameFiles(files).GetAwaiter().GetResult();
        }
    }

    public async Task CheckAndDownloadAsync(MVersion version)
    {
        foreach (var checker in this.GameFileCheckers)
        {
            DownloadFile[]? files = await checker.CheckFilesTaskAsync(MinecraftPath, version, pFileChanged)
                .ConfigureAwait(false);

            if (files == null || files.Length == 0)
                continue;

            await DownloadGameFiles(files).ConfigureAwait(false);
        }
    }

    public Process CreateProcess(MVersion version, MLaunchOption option, bool checkAndDownload=true)
    {
        option.StartVersion = version;
        
        if (checkAndDownload)
            CheckAndDownload(option.StartVersion);
        
        return CreateProcess(option);
    }

    public async Task<Process> CreateProcessAsync(string versionName, MLaunchOption option, 
        bool checkAndDownload=true)
    {
        var version = await GetVersionAsync(versionName).ConfigureAwait(false);
        return await CreateProcessAsync(version, option, checkAndDownload).ConfigureAwait(false);
    }

    public async Task<Process> CreateProcessAsync(MVersion version, MLaunchOption option, 
        bool checkAndDownload=true)
    {
        option.StartVersion = version;
        
        if (checkAndDownload)
            await CheckAndDownloadAsync(option.StartVersion).ConfigureAwait(false);
        
        return await CreateProcessAsync(option).ConfigureAwait(false);
    }
    
    public Process CreateProcess(MLaunchOption option)
    {
        checkLaunchOption(option);
        var launch = new MLaunch(option);
        return launch.GetProcess();
    }
    
    public async Task<Process> CreateProcessAsync(MLaunchOption option)
    {
        checkLaunchOption(option);
        var launch = new MLaunch(option);
        return await Task.Run(launch.GetProcess).ConfigureAwait(false);
    }

    public async Task<Process> LaunchAsync(string versionName, MLaunchOption option)
    {
        Process process = await CreateProcessAsync(versionName, option)
            .ConfigureAwait(false);
        process.Start();
        return process;
    }

    private void checkLaunchOption(MLaunchOption option)
    {
        if (option.Path == null)
            option.Path = MinecraftPath;
        if (option.StartVersion != null)
        {
            if (!string.IsNullOrEmpty(option.JavaPath))
                option.StartVersion.JavaBinaryPath = option.JavaPath;
            else if (!string.IsNullOrEmpty(option.JavaVersion))
                option.StartVersion.JavaVersion = option.JavaVersion;
            else if (string.IsNullOrEmpty(option.StartVersion.JavaBinaryPath))
                option.StartVersion.JavaBinaryPath = 
                    GetJavaPath(option.StartVersion) ?? GetDefaultJavaPath();
        }
    }

    public string? GetJavaPath(MVersion version)
    {
        if (string.IsNullOrEmpty(version.JavaVersion))
            return null;
        
        return JavaPathResolver.GetJavaBinaryPath(version.JavaVersion, MRule.OSName);
    }

    public string? GetDefaultJavaPath() => JavaPathResolver.GetDefaultJavaBinaryPath();
}
