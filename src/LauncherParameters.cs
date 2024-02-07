using CmlLib.Core.FileExtractors;
using CmlLib.Core.Installers;
using CmlLib.Core.Internals;
using CmlLib.Core.Java;
using CmlLib.Core.Natives;
using CmlLib.Core.Rules;
using CmlLib.Core.Tasks;
using CmlLib.Core.VersionLoader;

namespace CmlLib.Core;

public class MinecraftLauncherParameters
{
    public static MinecraftLauncherParameters CreateDefault() => 
        CreateDefault(HttpUtil.SharedHttpClient.Value);

    public static MinecraftLauncherParameters CreateDefault(HttpClient httpClient)
    {
        var parameters = new MinecraftLauncherParameters();
        parameters.HttpClient = httpClient;
        parameters.MinecraftPath = new MinecraftPath();
        parameters.RulesEvaluator = new RulesEvaluator();
        parameters.VersionLoader = new VersionLoaderCollection
        {
            new LocalJsonVersionLoader(parameters.MinecraftPath),
            new MojangJsonVersionLoader(httpClient)
        };
        parameters.JavaPathResolver = new MinecraftJavaPathResolver(parameters.MinecraftPath);
        parameters.GameInstaller = new TPLGameInstaller(TPLGameInstaller.BestMaxParallelism);
        parameters.NativeLibraryExtractor = new NativeLibraryExtractor(parameters.RulesEvaluator);
        var extractors = DefaultFileExtractors.CreateDefault(
            parameters.HttpClient, 
            parameters.RulesEvaluator, 
            parameters.JavaPathResolver);
        parameters.FileExtractors = extractors.ToExtractorCollection();
        parameters.TaskFactory = new Tasks.TaskFactory(httpClient);
        return parameters;
    }

    public static MinecraftLauncherParameters CreateEmpty()
    {
        return new MinecraftLauncherParameters();
    }

    private MinecraftLauncherParameters()
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
    public ITaskFactory? TaskFactory { get; set; }
    public IGameInstaller? GameInstaller { get; set; }
    public INativeLibraryExtractor? NativeLibraryExtractor { get; set; }
    public IRulesEvaluator? RulesEvaluator { get; set; }
}