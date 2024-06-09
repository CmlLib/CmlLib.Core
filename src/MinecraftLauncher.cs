using System.Diagnostics;
using CmlLib.Core.FileExtractors;
using CmlLib.Core.Installers;
using CmlLib.Core.Java;
using CmlLib.Core.Natives;
using CmlLib.Core.ProcessBuilder;
using CmlLib.Core.Rules;
using CmlLib.Core.Files;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using CmlLib.Core.VersionMetadata;

namespace CmlLib.Core;

public class MinecraftLauncher
{
    private readonly IProgress<InstallerProgressChangedEventArgs> _fileProgress;
    private readonly IProgress<ByteProgress> _byteProgress;
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

    public MinecraftLauncher() : 
        this(new MinecraftPath())
    {

    }

    public MinecraftLauncher(string path) : 
        this(MinecraftLauncherParameters.CreateDefault(new MinecraftPath(path)))
    {

    }

    public MinecraftLauncher(MinecraftPath path) : 
        this(MinecraftLauncherParameters.CreateDefault(path))
    {

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

    public async ValueTask<VersionMetadataCollection> GetAllVersionsAsync(CancellationToken cancellationToken = default)
    {
        Versions = await VersionLoader.GetVersionMetadatasAsync(cancellationToken);
        return Versions;
    }

    public async ValueTask<IVersion> GetVersionAsync(
        string versionName, 
        CancellationToken cancellationToken = default)
    {
        if (Versions == null)
            Versions = await GetAllVersionsAsync(cancellationToken);

        if (Versions.TryGetVersionMetadata(versionName, out var version))
        {
            return await Versions.GetAndSaveVersionAsync(version, MinecraftPath, cancellationToken);
        }
        else
        {
            Versions = await GetAllVersionsAsync(cancellationToken);
            return await Versions.GetAndSaveVersionAsync(versionName, MinecraftPath, cancellationToken);
        }
    }

    public async ValueTask<IEnumerable<GameFile>> ExtractFiles(
        string versionName, 
        CancellationToken cancellationToken = default)
    {
        var version = await GetVersionAsync(versionName, cancellationToken);
        return await ExtractFiles(version, cancellationToken);
    }

    public async ValueTask<IEnumerable<GameFile>> ExtractFiles(
        IVersion version,
        CancellationToken cancellationToken = default)
    {
        var allFiles = Enumerable.Empty<GameFile>();
        foreach (var extractor in FileExtractors)
        {
            var files = await extractor.Extract(MinecraftPath, version, RulesContext, cancellationToken);
            allFiles = allFiles.Concat(files);
        }
        return allFiles;
    }

    public ValueTask InstallAsync(
        string versionName,
        CancellationToken cancellationToken = default) =>
        InstallAsync(versionName, null, null, cancellationToken);

    public async ValueTask InstallAsync(
        string versionName,
        IProgress<InstallerProgressChangedEventArgs>? fileProgress,
        IProgress<ByteProgress>? byteProgress,
        CancellationToken cancellationToken = default)
    {
        var version = await GetVersionAsync(versionName, cancellationToken);
        await InstallAsync(version, fileProgress, byteProgress, cancellationToken);
    }

    public ValueTask InstallAsync(
        IVersion version,
        CancellationToken cancellationToken = default) =>
        InstallAsync(version, null, null, cancellationToken);

    public async ValueTask InstallAsync(
        IVersion version,
        IProgress<InstallerProgressChangedEventArgs>? fileProgress,
        IProgress<ByteProgress>? byteProgress,
        CancellationToken cancellationToken = default)
    {
        var files = Enumerable.Empty<GameFile>();
        foreach (var v in version.EnumerateToParent())
        {
            var f = await ExtractFiles(v, cancellationToken);
            files = files.Concat(f);
        }

        await GameInstaller.Install(
            files,
            fileProgress ?? _fileProgress,
            byteProgress ?? _byteProgress,
            cancellationToken);
    }

    public async ValueTask<Process> BuildProcessAsync(
        string versionName,
        MLaunchOption launchOption,
        CancellationToken cancellationToken = default)
    {
        var version = await GetVersionAsync(versionName, cancellationToken);
        return BuildProcess(version, launchOption);
    }

    public Process BuildProcess(
        IVersion version,
        MLaunchOption launchOption)
    {
        launchOption.Path ??= MinecraftPath;
        launchOption.StartVersion ??= version;
        launchOption.NativesDirectory ??= createNativePath(version);
        launchOption.JavaPath ??= GetJavaPath(version) ?? GetDefaultJavaPath();

        var processBuilder = new MinecraftProcessBuilder(RulesEvaluator, RulesContext, launchOption);
        var process = processBuilder.CreateProcess();
        return process;
    }

    public string? GetJavaPath(IVersion version)
    {
        var javaVersion = version.GetInheritedProperty(v => v.JavaVersion);
        if (javaVersion == null)
            return null;
        return JavaPathResolver.GetJavaBinaryPath(
            javaVersion,
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

    public ValueTask<Process> InstallAndBuildProcessAsync(
        string versionName,
        MLaunchOption launchOption,
        CancellationToken cancellationToken = default) =>
        InstallAndBuildProcessAsync(versionName, launchOption, null, null, cancellationToken);

    public async ValueTask<Process> InstallAndBuildProcessAsync(
        string versionName,
        MLaunchOption launchOption,
        IProgress<InstallerProgressChangedEventArgs>? fileProgress,
        IProgress<ByteProgress>? byteProgress,
        CancellationToken cancellationToken = default)
    {
        var version = await GetVersionAsync(versionName, cancellationToken);
        await InstallAsync(version, fileProgress, byteProgress, cancellationToken);
        return BuildProcess(version, launchOption);
    }

    // legacy api
    public ValueTask<Process> CreateProcessAsync(string versionName, MLaunchOption launchOption) =>
        InstallAndBuildProcessAsync(versionName, launchOption, null, null, default);
}
