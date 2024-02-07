using CmlLib.Core.Files;
using CmlLib.Core.Internals;
using CmlLib.Core.Java;
using CmlLib.Core.Rules;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;

namespace CmlLib.Core.FileExtractors;

public class JavaFileExtractor : IFileExtractor
{
    private readonly HttpClient _httpClient;
    private readonly IJavaPathResolver _javaPathResolver;
    public string JavaManifestServer { get; set; } = MojangServer.JavaManifest;

    public JavaFileExtractor(
        HttpClient httpClient,
        IJavaPathResolver javaPathResolver)
    {
        _httpClient = httpClient;
        _javaPathResolver = javaPathResolver;
    }

    public async ValueTask<IEnumerable<LinkedTaskHead>> Extract(
        ITaskFactory taskFactory,
        MinecraftPath path,
        IVersion version,
        RulesEvaluatorContext rulesContext,
        CancellationToken cancellationToken)
    {
        JavaVersion javaVersion;
        if (!version.JavaVersion.HasValue || string.IsNullOrEmpty(version.JavaVersion.Value.Component))
            javaVersion = MinecraftJavaPathResolver.JreLegacyVersion;
        else
            javaVersion = version.JavaVersion.Value;

        var manifestResolver = new MinecraftJavaManifestResolver(_httpClient);
        manifestResolver.ManifestServer = JavaManifestServer;

        var extractor = new Extractor(
            taskFactory, 
            _javaPathResolver, 
            rulesContext, 
            manifestResolver);
        return await extractor.ExtractFromJavaVersion(javaVersion, cancellationToken);
    }

    public class Extractor
    {
        private readonly ITaskFactory _taskFactory;
        private readonly IJavaPathResolver _javaPathResolver;
        private readonly RulesEvaluatorContext _rulesContext;
        private readonly MinecraftJavaManifestResolver _manifestResolver;

        public Extractor(
            ITaskFactory taskFactory, 
            IJavaPathResolver javaPathResolver,
            RulesEvaluatorContext rulesContext, 
            MinecraftJavaManifestResolver manifestResolver)
        {
            _taskFactory = taskFactory;
            _javaPathResolver = javaPathResolver;
            _rulesContext = rulesContext;
            _manifestResolver = manifestResolver;
        }

        public async ValueTask<IEnumerable<LinkedTaskHead>> ExtractFromJavaVersion(
            JavaVersion javaVersion, 
            CancellationToken cancellationToken)
        {
            var osName = MinecraftJavaManifestResolver.GetOSNameForJava(_rulesContext.OS) ?? "";
            var manifests = await _manifestResolver.GetManifestsForOS(osName);
            var manifestUrl = findManifestUrl(manifests, javaVersion.Component);

            if (string.IsNullOrEmpty(manifestUrl) &&
                javaVersion.Component != MinecraftJavaPathResolver.JreLegacyVersion.Component)
                manifestUrl = findManifestUrl(manifests, MinecraftJavaPathResolver.JreLegacyVersion.Component);

            if (string.IsNullOrEmpty(manifestUrl))
                return Enumerable.Empty<LinkedTaskHead>();

            var installPath = _javaPathResolver.GetJavaDirPath(javaVersion, _rulesContext);
            var files = await _manifestResolver.GetFilesFromManifest(manifestUrl, cancellationToken);
            return iterateFileToTask(installPath, files);
        }

        private string? findManifestUrl(IEnumerable<MinecraftJavaManifestMetadata> metadatas, string component)
        {
            return metadatas.FirstOrDefault(v => v.Component == component)?.Metadata?.Url;
        }

        private IEnumerable<LinkedTaskHead> iterateFileToTask(
            string path,
            IEnumerable<MinecraftJavaFile> files)
        {
            foreach (var javaFile in files)
            {
                if (javaFile.Type == "file")
                {
                    var filePath = Path.Combine(path, javaFile.Name);
                    filePath = IOUtil.NormalizePath(filePath);

                    var taskFile = new TaskFile(javaFile.Name)
                    {
                        Hash = javaFile.Sha1,
                        Path = filePath,
                        Url = javaFile.Url,
                        Size = javaFile.Size
                    };

                    yield return LinkedTaskBuilder.Create(taskFile, _taskFactory)
                        .CheckFile(
                            onSuccess => onSuccess.ReportDone(),
                            onFail => onFail
                                .Download()
                                .ThenIf(javaFile.Executable).Then(new ChmodTask(javaFile.Name, filePath)))
                        .BuildHead();
                }
            }
        }
    }
}