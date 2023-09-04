using System.ComponentModel;
using CmlLib.Core.Java;
using System.Text.Json;
using System.Net;
using CmlLib.Core.Rules;
using CmlLib.Core.Internals;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;

namespace CmlLib.Core.FileExtractors;

public class LegacyJavaFileExtractor : IFileExtractor
{
    private readonly HttpClient _httpClient;
    private readonly IJavaPathResolver _javaPathResolver;

    public LegacyJavaFileExtractor(
        HttpClient httpClient, 
        IJavaPathResolver resolver) => 
        (_httpClient, _javaPathResolver) = (httpClient, resolver);

    public ValueTask<IEnumerable<LinkedTaskHead>> Extract(
        MinecraftPath path, 
        IVersion version,
        RulesEvaluatorContext rulesContext,
        CancellationToken cancellationToken)
    {
        var task = createTask(path, rulesContext, cancellationToken);
        return new ValueTask<IEnumerable<LinkedTaskHead>>(new LinkedTaskHead[] { task });
    }

    private LinkedTaskHead createTask(
        MinecraftPath path,
        RulesEvaluatorContext rulesContext,
        CancellationToken cancellationToken)
    {
        var javaVersion = new JavaVersion
        {
            Component = "m-legacy",
            MajorVersion = 17
        };

        var javaBinaryPath = _javaPathResolver.GetJavaBinaryPath(path, javaVersion, rulesContext);
        var javaBinaryDir = _javaPathResolver.GetJavaDirPath(path, javaVersion, rulesContext);
        var file = new TaskFile(javaVersion.Component)
        {
            Path = javaBinaryPath
        };

        var checkTask = new FileCheckTask(file);
        var javaUrlTask = new ActionTask(file.Name, async task =>
        {
            var javaUrl = await GetJavaUrlAsync();
            var zipPath = Path.Combine(Path.GetTempPath(), "jre.zip");
            var lzmaFile = new TaskFile("jre.lzma")
            {
                Path = Path.Combine(Path.GetTempPath(), "jre.lzma"),
                Url = javaUrl
            };

            var linkedTask = LinkedTask.LinkTasks(
                new DownloadTask(lzmaFile, _httpClient),
                new LZMADecompressTask("jre.lzma", lzmaFile.Path, zipPath),
                new UnzipTask("jre.zip", zipPath, javaBinaryDir),
                new ChmodTask(javaVersion.Component, javaBinaryPath)
            );
            return task.InsertNextTask(linkedTask!);
        });

        checkTask.OnFalse = javaUrlTask;
        return new LinkedTaskHead(checkTask, file);
    }

    public async Task<string> GetJavaUrlAsync()
    {
        var json = await _httpClient.GetStringAsync(MojangServer.LauncherMeta)
            .ConfigureAwait(false);
        return parseLauncherMetadata(json);
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