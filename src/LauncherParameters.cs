using CmlLib.Core.FileExtractors;
using CmlLib.Core.Installers;
using CmlLib.Core.Internals;
using CmlLib.Core.Java;
using CmlLib.Core.Natives;
using CmlLib.Core.Rules;
using CmlLib.Core.VersionLoader;

namespace CmlLib.Core;

public class LauncherParameters
{
    public static LauncherParameters CreateDefault() => 
        CreateDefault(HttpUtil.SharedHttpClient.Value);

    public static LauncherParameters CreateDefault(HttpClient httpClient)
    {
        var parameters = new LauncherParameters();
        parameters.HttpClient = httpClient;
        parameters.MinecraftPath = new MinecraftPath();
        parameters.RulesEvaluator = new RulesEvaluator();
        parameters.VersionLoader = new VersionLoaderCollection
        {
            new LocalVersionLoader(parameters.MinecraftPath),
            new MojangVersionLoader(httpClient)
        };
        parameters.JavaPathResolver = new MinecraftJavaPathResolver();
        parameters.GameInstaller = new TPLGameInstaller(TPLGameInstaller.BestMaxParallelism);
        parameters.NativeLibraryExtractor = new NativeLibraryExtractor(parameters.RulesEvaluator);
        var extractors = DefaultFileExtractors.CreateDefault(
            parameters.HttpClient, 
            parameters.RulesEvaluator, 
            parameters.JavaPathResolver);
        parameters.FileExtractors = extractors.ToExtractorCollection();
        return parameters;
    }

    public static LauncherParameters CreateEmpty()
    {
        return new LauncherParameters();
    }

    private LauncherParameters()
    {

    }

    private HttpClient? _httpClient;
    public HttpClient HttpClient
    {
        get => _httpClient ??= HttpUtil.SharedHttpClient.Value;
        set => _httpClient = value;
    }

    public MinecraftPath? MinecraftPath { get; set; }
    public IVersionLoader? VersionLoader { get; set; }
    public IJavaPathResolver? JavaPathResolver { get; set; }
    public FileExtractorCollection? FileExtractors { get; set; }
    public IGameInstaller? GameInstaller { get; set; }
    public INativeLibraryExtractor? NativeLibraryExtractor { get; set; }
    public IRulesEvaluator? RulesEvaluator { get; set; }
}