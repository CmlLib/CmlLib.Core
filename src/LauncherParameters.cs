using CmlLib.Core.FileExtractors;
using CmlLib.Core.Installers;
using CmlLib.Core.Internals;
using CmlLib.Core.Java;
using CmlLib.Core.Natives;
using CmlLib.Core.Rules;
using CmlLib.Core.VersionLoader;
using CmlLib.Core.VersionMetadata;

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
        parameters.VersionLoader = new MojangJsonVersionLoaderV2(parameters.MinecraftPath, httpClient);
        parameters.JavaPathResolver = new MinecraftJavaPathResolver(parameters.MinecraftPath);
        parameters.GameInstaller = ParallelGameInstaller.CreateAsCoreCount(httpClient);
        parameters.NativeLibraryExtractor = new NativeLibraryExtractor(parameters.RulesEvaluator);
        var extractors = DefaultFileExtractors.CreateDefault(
            parameters.HttpClient, 
            parameters.RulesEvaluator, 
            parameters.JavaPathResolver);
        parameters.FileExtractors = extractors.ToExtractorCollection();
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
    public IGameInstaller? GameInstaller { get; set; }
    public INativeLibraryExtractor? NativeLibraryExtractor { get; set; }
    public IRulesEvaluator? RulesEvaluator { get; set; }
}