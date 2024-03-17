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

    public async ValueTask<IEnumerable<GameFile>> ExtractFiles(
        string versionName,
        CancellationToken cancellationToken = default)
    {
        var version = await GetVersionAsync(versionName);
        return await ExtractFiles(version, cancellationToken);
    }

    public async ValueTask<IEnumerable<GameFile>> ExtractFiles(
        IVersion version,
        CancellationToken cancellationToken)
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
        var version = await GetVersionAsync(versionName);
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
        var files = await ExtractFiles(version, cancellationToken);
        await GameInstaller.Install(
            files,
            fileProgress ?? _fileProgress,
            byteProgress ?? _byteProgress,
            cancellationToken);
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
        launchOption.JavaPath ??= GetJavaPath(version) ?? GetDefaultJavaPath();
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
        var version = await GetVersionAsync(versionName);
        await InstallAsync(version, fileProgress, byteProgress, cancellationToken);
        return BuildProcess(version, launchOption);
    }

    // legacy api
    public ValueTask<Process> CreateProcessAsync(string versionName, MLaunchOption launchOption) =>
        InstallAndBuildProcessAsync(versionName, launchOption, null, null, default);
}
