using System.Diagnostics;
using CmlLib.Core.FileExtractors;
using CmlLib.Core.Installers;
using CmlLib.Core.Java;
using CmlLib.Core.Natives;
using CmlLib.Core.ProcessBuilder;
using CmlLib.Core.Rules;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using CmlLib.Core.VersionMetadata;

namespace CmlLib.Core;

public class MinecraftLauncher
{
    private readonly Progress<InstallerProgressChangedEventArgs> _fileProgress;
    private readonly Progress<ByteProgress> _byteProgress;
    public event EventHandler<InstallerProgressChangedEventArgs>? FileProgressChanged;
    public event EventHandler<ByteProgress>? ByteProgressChanged;

    public MinecraftPath MinecraftPath { get; }
    public IVersionLoader VersionLoader { get; }
    public IJavaPathResolver JavaPathResolver { get; }
    public FileExtractorCollection FileExtractors { get; }
    public IGameInstaller GameInstaller { get; }
    public INativeLibraryExtractor NativeLibraryExtractor { get; }
    public IRulesEvaluator RulesEvaluator { get; }

    public RulesEvaluatorContext RulesContext { get; set; }
    public VersionMetadataCollection? Versions { get; private set; }

    public MinecraftLauncher(string path) : this(WithMinecraftPath(new MinecraftPath(path)))
    {

    }

    public MinecraftLauncher(MinecraftPath path) : this(WithMinecraftPath(path))
    {

    }

    private static MinecraftLauncherParameters WithMinecraftPath(MinecraftPath path)
    {
        var parameters = MinecraftLauncherParameters.CreateDefault();
        parameters.MinecraftPath = path;
        return parameters;
    }

    public MinecraftLauncher(MinecraftLauncherParameters parameters)
    {
        MinecraftPath = parameters.MinecraftPath
            ?? throw new ArgumentException(nameof(parameters.MinecraftPath) + " was null");
        VersionLoader = parameters.VersionLoader
            ?? throw new ArgumentException(nameof(parameters.VersionLoader) + " was null");
        JavaPathResolver = parameters.JavaPathResolver
            ?? throw new ArgumentException(nameof(parameters.JavaPathResolver) + " was null");
        FileExtractors = parameters.FileExtractors
            ?? throw new ArgumentException(nameof(parameters.FileExtractors) + " was null");
        GameInstaller = parameters.GameInstaller 
            ?? throw new ArgumentException(nameof(parameters.GameInstaller) + " was null");
        NativeLibraryExtractor = parameters.NativeLibraryExtractor
            ?? throw new ArgumentException(nameof(parameters.NativeLibraryExtractor) + " was null");
        RulesEvaluator = parameters.RulesEvaluator
            ?? throw new ArgumentException(nameof(parameters.RulesEvaluator) + " was null");
        RulesContext = new RulesEvaluatorContext(LauncherOSRule.Current);

        _fileProgress = new Progress<InstallerProgressChangedEventArgs>(e => FileProgressChanged?.Invoke(this, e));
        _byteProgress = new Progress<ByteProgress>(e => ByteProgressChanged?.Invoke(this, e));
    }

    public async ValueTask<VersionMetadataCollection> GetAllVersionsAsync()
    {
        Versions = await VersionLoader.GetVersionMetadatasAsync();
        return Versions;
    }

    public async ValueTask<IVersion> GetVersionAsync(string versionName)
    {
        if (Versions == null)
            Versions = await GetAllVersionsAsync();

        if (Versions.TryGetVersionMetadata(versionName, out var version))
        {
            return await Versions.GetAndSaveVersionAsync(version, MinecraftPath);
        }
        else
        {
            Versions = await GetAllVersionsAsync();
            return await Versions.GetAndSaveVersionAsync(versionName, MinecraftPath);
        }
    }

    public async ValueTask<IReadOnlyList<GameFile>> ExtractFiles(
        string versionName,
        CancellationToken cancellationToken = default)
    {
        var version = await GetVersionAsync(versionName);
        return await ExtractFiles(version, cancellationToken);
    }

    public async ValueTask<IReadOnlyList<GameFile>> ExtractFiles(
        IVersion version,
        CancellationToken cancellationToken)
    {
        var files = new List<GameFile>();
        foreach (var extractor in FileExtractors)
        {
            files.AddRange(await extractor.Extract(MinecraftPath, version, RulesContext, cancellationToken));
        }
        return files;
    }

    public async ValueTask InstallAsync(
        string versionName, 
        CancellationToken cancellationToken = default)
    {
        var version = await GetVersionAsync(versionName);
        await InstallAsync(version, cancellationToken);
    }

    public async ValueTask InstallAsync(
        IVersion version,
        CancellationToken cancellationToken = default)
    {
        var files = await ExtractFiles(version, cancellationToken);
        await GameInstaller.Install(
            files,
            _fileProgress,
            _byteProgress,
            cancellationToken);
    }

    // Install and build game process
    public async ValueTask<Process> CreateProcessAsync(
        string versionName,
        MLaunchOption launchOption,
        bool checkAndDownload = true)
    {
        if (checkAndDownload)
        {
            await InstallAsync(versionName, default);
        }

        return await BuildProcessAsync(versionName, launchOption);
    }

    public async ValueTask<Process> BuildProcessAsync(
        string versionName, 
        MLaunchOption launchOption)
    {
        var version = await GetVersionAsync(versionName);
        return BuildProcess(version, launchOption);
    }

    public Process BuildProcess(
        IVersion version, 
        MLaunchOption launchOption)
    {
        launchOption.NativesDirectory ??= createNativePath(version);
        launchOption.Path ??= MinecraftPath;
        launchOption.StartVersion ??= version;
        launchOption.JavaPath ??= GetJavaPath(version);
        launchOption.RulesContext ??= RulesContext;

        var processBuilder = new MinecraftProcessBuilder(RulesEvaluator, launchOption);
        var process = processBuilder.CreateProcess();
        return process;
    }

    public string? GetJavaPath(IVersion version)
    {
        if (!version.JavaVersion.HasValue || string.IsNullOrEmpty(version.JavaVersion?.Component))
            return null;

        return JavaPathResolver.GetJavaBinaryPath(
            version.JavaVersion.Value,
            RulesContext);
    }

    public string? GetDefaultJavaPath()
    {
        return JavaPathResolver.GetDefaultJavaBinaryPath(RulesContext);
    }
 
    private string createNativePath(IVersion version)
    {
        NativeLibraryExtractor.Clean(MinecraftPath, version);
        return NativeLibraryExtractor.Extract(MinecraftPath, version, RulesContext);
    }
}
