using System.Diagnostics;
using System.Text.Json;
using CmlLib.Core.Installer;
using CmlLib.Core.Java;
using CmlLib.Core.Rules;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;
using CmlLib.Core.Internals;

namespace CmlLib.Core.FileExtractors;

public class JavaFileExtractor : IFileExtractor
{
    private readonly LauncherOSRule _os;
    private readonly HttpClient _httpClient;
    public readonly IJavaPathResolver _javaPathResolver;
    public string JavaManifestServer { get; set; } = MojangServer.JavaManifest;

    public JavaFileExtractor(
        HttpClient httpClient,
        IJavaPathResolver javaPathResolver,
        LauncherOSRule rule)
    {
        _httpClient = httpClient;
        _javaPathResolver = javaPathResolver;
        _os = rule;
    }

    public async ValueTask<IEnumerable<LinkedTaskHead>> Extract(MinecraftPath path, IVersion version)
    {
        if (!version.JavaVersion.HasValue || string.IsNullOrEmpty(version.JavaVersion.Value.Component))
            return await extractFromJavaVersion(MinecraftJavaPathResolver.JreLegacyVersion);
        else
            return await extractFromJavaVersion(version.JavaVersion.Value);
    }

    private async ValueTask<IEnumerable<LinkedTaskHead>> extractFromJavaVersion(JavaVersion javaVersion)
    {
        using var response = await _httpClient.GetStreamAsync(JavaManifestServer);
        using var jsonDocument = JsonDocument.Parse(response);
        var root = jsonDocument.RootElement;

        var osName = getJavaOSName();
        if (string.IsNullOrEmpty(osName))
            return await legacyJavaChecker();
        if (!root.TryGetProperty(osName, out var javaVersionsForOS))
            return await legacyJavaChecker();

        var currentVersionManifestUrl = getManifestUrl(javaVersionsForOS, javaVersion);

        if (!string.IsNullOrEmpty(currentVersionManifestUrl))
            return await extractFromManifestUrl(currentVersionManifestUrl, javaVersion);
        else if (javaVersion.Component != MinecraftJavaPathResolver.JreLegacyVersion.Component)
        {
            var legacyVersionManifestUrl = getManifestUrl(javaVersionsForOS, MinecraftJavaPathResolver.JreLegacyVersion);
            if (!string.IsNullOrEmpty(legacyVersionManifestUrl))
                return await extractFromManifestUrl(legacyVersionManifestUrl, javaVersion);
        }

        return await legacyJavaChecker();
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

    private async ValueTask<IEnumerable<LinkedTaskHead>> extractFromManifestUrl(string manifestUrl, JavaVersion version)
    {
        using var res = await _httpClient.GetStreamAsync(manifestUrl);
        var json = JsonDocument.Parse(res); // should be disposed after extraction
        return extractFromManifestJson(json, version);
    }

    private IEnumerable<LinkedTaskHead> extractFromManifestJson(
        JsonDocument _json, JavaVersion version)
    {
        using var json = _json;
        var manifest = json.RootElement;

        var path = _javaPathResolver.GetJavaDirPath(version);

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
        checkTask.OnFalse = new DownloadTask(file, _httpClient);

        if (executable)
            checkTask.InsertNextTask(new ChmodTask(file.Name, file.Path));

        return new LinkedTaskHead(checkTask, file);
    }

    // legacy java checker using MJava
    private async ValueTask<IEnumerable<LinkedTaskHead>> legacyJavaChecker()
    {
        var legacyJavaPath = _javaPathResolver.GetJavaDirPath(MinecraftJavaPathResolver.CmlLegacyVersion);

        var mJava = new MJava(_httpClient, legacyJavaPath);
        if (mJava.CheckJavaExistence(_os))
            return Enumerable.Empty<LinkedTaskHead>();

        var javaUrl = await mJava.GetJavaUrlAsync();
        var lzmaPath = Path.Combine(Path.GetTempPath(), "jre.lzma");
        var zipPath = Path.Combine(Path.GetTempPath(), "jre.zip");

        var file = new TaskFile("jre.lzma")
        {
            Url = javaUrl,
            Path = lzmaPath
        };

        var task = LinkedTask.LinkTasks(new LinkedTask[]
        {
            new DownloadTask(file, _httpClient),
            new LZMADecompressTask(file.Name, lzmaPath, zipPath),
            new UnzipTask(file.Name, zipPath, legacyJavaPath),
            new ChmodTask(file.Name, mJava.GetBinaryPath(_os))
        });
        
        return new LinkedTaskHead[]
        {
            new LinkedTaskHead(task!, file)
        };
    }
}