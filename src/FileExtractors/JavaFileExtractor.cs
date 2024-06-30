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

    public async ValueTask<IEnumerable<GameFile>> Extract(
        MinecraftPath path,
        IVersion version,
        RulesEvaluatorContext rulesContext,
        CancellationToken cancellationToken)
    {
        JavaVersion javaVersion;
        if (string.IsNullOrEmpty(version.JavaVersion?.Component))
            javaVersion = MinecraftJavaPathResolver.JreLegacyVersion;
        else
            javaVersion = version.JavaVersion;

        var manifestResolver = new MinecraftJavaManifestResolver(_httpClient);
        manifestResolver.ManifestServer = JavaManifestServer;

        var extractor = new Extractor(
            _javaPathResolver,
            rulesContext,
            manifestResolver);
        return await extractor.ExtractFromJavaVersion(javaVersion, cancellationToken);
    }

    public class Extractor
    {
        private readonly IJavaPathResolver _javaPathResolver;
        private readonly RulesEvaluatorContext _rulesContext;
        private readonly MinecraftJavaManifestResolver _manifestResolver;

        public Extractor(
            IJavaPathResolver javaPathResolver,
            RulesEvaluatorContext rulesContext,
            MinecraftJavaManifestResolver manifestResolver)
        {
            _javaPathResolver = javaPathResolver;
            _rulesContext = rulesContext;
            _manifestResolver = manifestResolver;
        }

        public async ValueTask<IEnumerable<GameFile>> ExtractFromJavaVersion(
            JavaVersion javaVersion,
            CancellationToken cancellationToken)
        {
            var manifestUrl = await findManifestUrl(javaVersion);
            if (string.IsNullOrEmpty(manifestUrl))
                return Enumerable.Empty<GameFile>();
                
            var installPath = _javaPathResolver.GetJavaDirPath(javaVersion, _rulesContext);
            var files = await _manifestResolver.GetFilesFromManifest(manifestUrl, cancellationToken);
            return extractFiles(installPath, files);
        }

        private async ValueTask<string?> findManifestUrl(JavaVersion javaVersion)
        {
            var osName = MinecraftJavaManifestResolver.GetOSNameForJava(_rulesContext.OS);
            var manifests = await _manifestResolver.GetManifestsForOS(osName);
            var manifestUrl = findManifestUrlFromMetadatas(manifests, javaVersion.Component);

            if (string.IsNullOrEmpty(manifestUrl) &&
                javaVersion.Component != MinecraftJavaPathResolver.JreLegacyVersion.Component)
                manifestUrl = findManifestUrlFromMetadatas(manifests, MinecraftJavaPathResolver.JreLegacyVersion.Component);

            return manifestUrl;
        }

        private string? findManifestUrlFromMetadatas(IEnumerable<MinecraftJavaManifestMetadata> metadatas, string component)
        {
            return metadatas.FirstOrDefault(v => v.Component == component)?.Metadata?.Url;
        }

        private IEnumerable<GameFile> extractFiles(
            string path,
            IEnumerable<MinecraftJavaFile> files)
        {
            foreach (var javaFile in files)
            {
                if (javaFile.Type == "file")
                {
                    var gameFile = extractFile(path, javaFile);
                    yield return gameFile;
                }
            }
        }

        private GameFile extractFile(string path, MinecraftJavaFile javaFile)
        {
            var filePath = Path.Combine(path, javaFile.Name);
            filePath = IOUtil.NormalizePath(filePath);

            return new GameFile(javaFile.Name)
            {
                Hash = javaFile.Sha1,
                Path = filePath,
                Url = javaFile.Url,
                Size = javaFile.Size,
                UpdateTask = javaFile.Executable ? [new ChmodTask(NativeMethods.Chmod755)] : []
            };
        }
    }
}