using CmlLib.Core.Internals;
using CmlLib.Core.Java;
using CmlLib.Core.Rules;
using CmlLib.Core.Tasks;
using CmlLib.Core.Files;
using CmlLib.Core.Version;
using System.Text.Json;

namespace CmlLib.Core.FileExtractors;

public class LegacyJavaFileExtractor : IFileExtractor
{
    private readonly HttpClient _httpClient;
    private readonly IJavaPathResolver _javaPathResolver;

    public LegacyJavaFileExtractor(
        HttpClient httpClient, 
        IJavaPathResolver resolver) => 
        (_httpClient, _javaPathResolver) = (httpClient, resolver);

    public JavaVersion JavaVersion = new JavaVersion("m-legacy", "17");

    public async ValueTask<IEnumerable<GameFile>> Extract(
        MinecraftPath path, 
        IVersion version,
        RulesEvaluatorContext rulesContext,
        CancellationToken cancellationToken)
    {
        var javaBinaryPath = _javaPathResolver.GetJavaBinaryPath(JavaVersion, rulesContext);
        if (!File.Exists(javaBinaryPath))
        {
            return new [] { await createTask(rulesContext, cancellationToken) };
        }
        else
        {
            return Enumerable.Empty<GameFile>();
        }
    }

    private async Task<GameFile> createTask(
        RulesEvaluatorContext rulesContext,
        CancellationToken cancellationToken)
    {
        var javaBinaryPath = _javaPathResolver.GetJavaBinaryPath(JavaVersion, rulesContext);
        var javaBinaryDir = _javaPathResolver.GetJavaDirPath(JavaVersion, rulesContext);

        var javaUrl = await GetJavaUrlAsync(cancellationToken);
        return new GameFile("jre.lzma")
        {
            Path = Path.Combine(Path.GetTempPath(), "jre.lzma"),
            Hash = "0", // since the file is temporary it should be always downloaded again
            Url = javaUrl,
            UpdateTask = 
            [
                new LegacyJavaExtractionTask(javaBinaryDir),
                new ChmodTask(NativeMethods.Chmod755, javaBinaryPath)
            ]
        };
    }

    public async Task<string> GetJavaUrlAsync(CancellationToken cancellationToken)
    {
        using var res = await _httpClient.GetAsync(MojangServer.LauncherMeta, cancellationToken);
        var resStr = await res.Content.ReadAsStringAsync();
        return parseLauncherMetadata(resStr);
    }

    private string parseLauncherMetadata(string json)
    {
        using var jsonDocument = JsonDocument.Parse(json);
        var root = jsonDocument.RootElement;

        var javaUrl = root
            .GetPropertyOrNull(LauncherOSRule.Current.Name ?? string.Empty)?
            .GetPropertyOrNull(LauncherOSRule.Current.Arch ?? string.Empty)?
            .GetPropertyOrNull("jre")?
            .GetPropertyValue("url");

        if (string.IsNullOrEmpty(javaUrl))
            throw new PlatformNotSupportedException("Downloading JRE on current OS is not supported. Set JavaPath manually.");
        return javaUrl;
    }
}