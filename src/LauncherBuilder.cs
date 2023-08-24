using CmlLib.Core.FileExtractors;
using CmlLib.Core.Installers;
using CmlLib.Core.Internals;
using CmlLib.Core.Java;
using CmlLib.Core.Natives;
using CmlLib.Core.Rules;
using CmlLib.Core.VersionLoader;

namespace CmlLib.Core;

public class LauncherBuilder
{
    public static LauncherBuilder Create() => new LauncherBuilder();
    private LauncherBuilder() {}

    private HttpClient? _httpClient = null;
    public HttpClient HttpClient 
    { 
        get => _httpClient ??= HttpUtil.SharedHttpClient.Value;
        set => _httpClient = value;
    }

    private MinecraftPath? _minecraftPath = null;
    public MinecraftPath MinecraftPath 
    { 
        get => _minecraftPath ??= new MinecraftPath();
        set => _minecraftPath = value;
    }

    private IVersionLoader? _versionLoader;
    private IJavaPathResolver? _javaPathResolver;
    private FileExtractorCollection? _extractors;
    private IGameInstaller? _gameInstaller;
    private INativeLibraryExtractor? _nativeExtractor;
    private IRulesEvaluator? _rulesEvaluator;

    public LauncherBuilder WithHttpClient(HttpClient httpClient)
    {  
        HttpClient = httpClient;
        return this;
    }

    public LauncherBuilder WithMinecraftPath(MinecraftPath path)
    {
        MinecraftPath = path;
        return this;
    }

    public LauncherBuilder WithMinecraftPath(string path)
    {
        MinecraftPath = new MinecraftPath(path);
        return this;
    }

    public LauncherBuilder WithVersionLoader(IVersionLoader versionLoader)
    {
        _versionLoader = versionLoader;
        return this;
    }

    public LauncherBuilder WithJavaPathResolver(IJavaPathResolver javaPathResolver)
    {
        _javaPathResolver = javaPathResolver;
        return this;
    }

    public LauncherBuilder WithFileExtractors(FileExtractorCollection extractors)
    {
        _extractors = extractors;
        return this;
    }

    public LauncherBuilder WithGameInstaller(IGameInstaller installer)
    {
        _gameInstaller = installer;
        return this;
    }

    public LauncherBuilder WithNativeLibraryExtractor(INativeLibraryExtractor nativeLibraryExtractor)
    {
        _nativeExtractor = nativeLibraryExtractor;
        return this;
    }

    public LauncherBuilder WithRulesEvaluator(IRulesEvaluator rulesEvaluator)
    {
        _rulesEvaluator = rulesEvaluator;
        return this;
    }

    public MinecraftLauncher Build()
    {
        _versionLoader ??= new VersionLoaderCollection
        {
            new LocalVersionLoader(MinecraftPath),
            new MojangVersionLoader(HttpClient)
        };
        _rulesEvaluator ??= new RulesEvaluator();
        _javaPathResolver ??= new MinecraftJavaPathResolver();
        _extractors ??= new FileExtractorCollection();
        _gameInstaller ??= new TPLGameInstaller(getBestParallism());
        _nativeExtractor ??= new NativeLibraryExtractor(_rulesEvaluator);

        return new MinecraftLauncher(
            minecraftPath: MinecraftPath,
            versionLoader: _versionLoader,
            javaPathResolver: _javaPathResolver,
            fileExtractors: _extractors,
            gameInstaller: _gameInstaller,
            nativeLibraryExtractor: _nativeExtractor,
            rulesEvaluator: _rulesEvaluator
        );
    }

    private int getBestParallism()
    {
        // 2 <= p <= 8
        var p = Environment.ProcessorCount;
        p = Math.Max(p, 2);
        p = Math.Min(p, 8);
        return p;
    }
}