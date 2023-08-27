using System.Diagnostics;
using System.Text.Json;
using CmlLib.Core.Java;
using CmlLib.Core.Rules;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;
using CmlLib.Core.Internals;

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
        return await extractFromJavaVersion(
                path, 
                javaVersion, 
                version,
                rulesContext,
                cancellationToken);
    }

    private async ValueTask<IEnumerable<LinkedTaskHead>> extractFromJavaVersion(
        MinecraftPath minecraftPath, 
        JavaVersion javaVersion, 
        IVersion version,
        RulesEvaluatorContext rulesContext,
        CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetStreamAsync(JavaManifestServer);
        using var jsonDocument = JsonDocument.Parse(response);
        var root = jsonDocument.RootElement;

        var osName = getJavaOSName();
        if (string.IsNullOrEmpty(osName))
            return await createLegacyJavaTask(minecraftPath, version, rulesContext, cancellationToken);
        if (!root.TryGetProperty(osName, out var javaVersionsForOS))
            return await createLegacyJavaTask(minecraftPath, version, rulesContext, cancellationToken);

        var currentVersionManifestUrl = getManifestUrl(javaVersionsForOS, javaVersion);

        if (!string.IsNullOrEmpty(currentVersionManifestUrl))
            return await extractFromManifestUrl(minecraftPath, currentVersionManifestUrl, javaVersion, rulesContext, cancellationToken);
        else if (javaVersion.Component != MinecraftJavaPathResolver.JreLegacyVersion.Component)
        {
            var legacyVersionManifestUrl = getManifestUrl(javaVersionsForOS, MinecraftJavaPathResolver.JreLegacyVersion);
            if (!string.IsNullOrEmpty(legacyVersionManifestUrl))
                return await extractFromManifestUrl(minecraftPath, legacyVersionManifestUrl, javaVersion, rulesContext, cancellationToken);
        }

        return await createLegacyJavaTask(minecraftPath, version, rulesContext, cancellationToken);
    }

    private string? getJavaOSName() // TODO: find exact version name
    {
        return (LauncherOSRule.Current.Name, LauncherOSRule.Current.Arch) switch
        {
            (LauncherOSRule.Windows, "64") => "windows-x64",
            (LauncherOSRule.Windows, "32") => "windows-x86",
            (LauncherOSRule.Windows, _) => null,
            (LauncherOSRule.Linux, "64") => "linux",
            (LauncherOSRule.Linux, "32") => "linux-i386",
            (LauncherOSRule.Linux, _) => "linux",
            (LauncherOSRule.OSX, "64") => "mac-os",
            (LauncherOSRule.OSX, "32") => "mac-os",
            (LauncherOSRule.OSX, "arm") => "mac-os", // TODO
            (LauncherOSRule.OSX, "arm64") => "mac-os",
            (LauncherOSRule.OSX, _) => null,
            (_, _) => null
        };
    }

    private string? getManifestUrl(JsonElement element, JavaVersion version)
    {
        return element
            .GetPropertyOrNull(version.Component)?
            .EnumerateArray()
            .FirstOrDefault()
            .GetPropertyOrNull("manifest")?
            .GetPropertyOrNull("url")?
            .GetString();
    }

    private async ValueTask<IEnumerable<LinkedTaskHead>> extractFromManifestUrl(
        MinecraftPath minecraftPath, 
        string manifestUrl, 
        JavaVersion version,
        RulesEvaluatorContext rulesContext,
        CancellationToken cancellationToken)
    {
        using var res = await _httpClient.GetAsync(manifestUrl, cancellationToken);
        using var stream = await res.Content.ReadAsStreamAsync();
        var json = JsonDocument.Parse(stream); // should be disposed after extraction
        return extractFromManifestJson(minecraftPath, json, version, rulesContext);
    }

    private IEnumerable<LinkedTaskHead> extractFromManifestJson(
        MinecraftPath minecraftPath, 
        JsonDocument _json, 
        JavaVersion version,
        RulesEvaluatorContext rulesContext)
    {
        using var json = _json;
        var manifest = json.RootElement;

        var path = _javaPathResolver.GetJavaDirPath(minecraftPath, version, rulesContext);

        if (!manifest.TryGetProperty("files", out var files))
            yield break;

        var objects = files.EnumerateObject();
        foreach (var prop in objects)
        {
            var name = prop.Name;
            var value = prop.Value;

            var type = value.GetPropertyValue("type");
            if (type == "file")
            {
                var filePath = Path.Combine(path, name);
                filePath = IOUtil.NormalizePath(filePath);

                var task = createTask(value, name, filePath);
                if (task.HasValue)
                    yield return task.Value;
            }
            else
            {
                if (type != "directory")
                    Debug.WriteLine(type);
            }

        }
    }

    private LinkedTaskHead? createTask(JsonElement value, string name, string filePath)
    {
        var downloadObj = value.GetPropertyOrNull("downloads")?.GetPropertyOrNull("raw");
        if (downloadObj == null)
            return null;

        var url = downloadObj.Value.GetPropertyValue("url");
        if (string.IsNullOrEmpty(url))
            return null;

        var hash = downloadObj.Value.GetPropertyValue("sha1");
        var size = downloadObj.Value.GetPropertyOrNull("size")?.GetInt64() ?? 0;

        var executable = value.GetPropertyOrNull("executable")?.GetBoolean() ?? false;

        var file = new TaskFile(name)
        {
            Hash = hash,
            Path = filePath,
            Url = url,
            Size = size
        };

        var checkTask = new FileCheckTask(file);
        var downloadTask = new DownloadTask(file, _httpClient);
        var chmodTask = new ChmodTask(file.Name, file.Path);
        var progressTask = ProgressTask.CreateDoneTask(file);

        checkTask.OnTrue = progressTask;
        checkTask.OnFalse = downloadTask;

        if (executable)
            downloadTask.InsertNextTask(chmodTask);

        return new LinkedTaskHead(checkTask, file);
    }

    private async ValueTask<IEnumerable<LinkedTaskHead>> createLegacyJavaTask(
        MinecraftPath path, 
        IVersion version,
        RulesEvaluatorContext rulesContext,
        CancellationToken cancellationToken)
    {
        var legacyJava = new LegacyJavaFileExtractor(_httpClient, _javaPathResolver);
        return await legacyJava.Extract(path, version, rulesContext, cancellationToken);
    }
}