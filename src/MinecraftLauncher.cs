using System.Diagnostics;
using CmlLib.Core.FileExtractors;
using CmlLib.Core.Installers;
using CmlLib.Core.Java;
using CmlLib.Core.Natives;
using CmlLib.Core.ProcessBuilder;
using CmlLib.Core.Rules;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using CmlLib.Core.VersionMetadata;

namespace CmlLib.Core;

public class MinecraftLauncher
{
    public MinecraftPath MinecraftPath { get; }
    public IVersionLoader VersionLoader { get; }
    public IJavaPathResolver JavaPathResolver { get; }
    public FileExtractorCollection FileExtractors { get; }
    public IGameInstaller GameInstaller { get; }
    public INativeLibraryExtractor NativeLibraryExtractor { get; }
    public IRulesEvaluator RulesEvaluator { get; }

    private readonly Progress<InstallerProgressChangedEventArgs> _fileProgress;
    private readonly Progress<ByteProgress> _byteProgress;
    public event EventHandler<InstallerProgressChangedEventArgs>? FileProgressEvent;
    public event EventHandler<ByteProgress>? ByteProgressEvent;

    public MinecraftLauncher(
        MinecraftPath minecraftPath, 
        IVersionLoader versionLoader, 
        IJavaPathResolver javaPathResolver, 
        FileExtractorCollection fileExtractors, 
        IGameInstaller gameInstaller, 
        INativeLibraryExtractor nativeLibraryExtractor, 
        IRulesEvaluator rulesEvaluator)
    {
        MinecraftPath = minecraftPath;
        VersionLoader = versionLoader;
        JavaPathResolver = javaPathResolver;
        FileExtractors = fileExtractors;
        GameInstaller = gameInstaller;
        NativeLibraryExtractor = nativeLibraryExtractor;
        RulesEvaluator = rulesEvaluator;
        RulesContext = new RulesEvaluatorContext(LauncherOSRule.Current);

        _fileProgress = new Progress<InstallerProgressChangedEventArgs>(e => FileProgressEvent?.Invoke(this, e));
        _byteProgress = new Progress<ByteProgress>(e => ByteProgressEvent?.Invoke(this, e));
    }

    public RulesEvaluatorContext RulesContext { get; set; }
    public VersionCollection? Versions { get; private set; }

    public async ValueTask<VersionCollection> GetAllVersionsAsync()
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

    public async ValueTask InstallAsync(
        string versionName, 
        CancellationToken cancellationToken = default)
    {
        var version = await GetVersionAsync(versionName);
        await InstallAsync(version);
    }

    public async ValueTask InstallAsync(
        IVersion version,
        CancellationToken cancellationToken = default)
    {
        await GameInstaller.Install(
            FileExtractors, 
            MinecraftPath, 
            version, 
            RulesContext,
            _fileProgress,
            _byteProgress,
            cancellationToken);
    }

    public Process BuildProcess(
        IVersion version, 
        MLaunchOption launchOption)
    {
        launchOption.NativesDirectory = createNativePath(version);
        setLaunchOption(version, launchOption);
        var processBuilder = new MinecraftProcessBuilder(RulesEvaluator, launchOption);
        var process = processBuilder.CreateProcess();
        return process;
    }

    private void setLaunchOption(IVersion version, MLaunchOption option)
    {
        option.Path ??= MinecraftPath;
        option.StartVersion ??= version;
        option.JavaPath = GetJavaPath(version);
    }

    public string? GetJavaPath(IVersion version)
    {
        if (!version.JavaVersion.HasValue || string.IsNullOrEmpty(version.JavaVersion?.Component))
            return null;

        return JavaPathResolver.GetJavaBinaryPath(
            MinecraftPath,
            version.JavaVersion.Value,
            RulesContext);
    }

    public string? GetDefaultJavaPath() => JavaPathResolver
        .GetDefaultJavaBinaryPath(MinecraftPath, RulesContext);

    private string createNativePath(IVersion version)
    {
        NativeLibraryExtractor.Clean(MinecraftPath, version);
        return NativeLibraryExtractor.Extract(MinecraftPath, version, RulesContext);
    }
}
