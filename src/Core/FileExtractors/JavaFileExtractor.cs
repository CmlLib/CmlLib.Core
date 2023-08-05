using System.Diagnostics;
using System.Text.Json;
using CmlLib.Core.Installer;
using CmlLib.Core.Java;
using CmlLib.Core.Rules;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;
using CmlLib.Utils;

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

    public async ValueTask<IEnumerable<LinkedTask>> Extract(MinecraftPath path, IVersion version)
    {
        //if (!string.IsNullOrEmpty(version.JavaBinaryPath) && File.Exists(version.JavaBinaryPath))
        //    return null
        JavaVersion javaVersion;
        if (!version.JavaVersion.HasValue || string.IsNullOrEmpty(version.JavaVersion.Value.Component))
            javaVersion = MinecraftJavaPathResolver.JreLegacyVersion;
        else
            javaVersion = version.JavaVersion.Value;

        // try three versions
        //  - latest
        //  - JreLegacyVersionName(jre-legacy)
        //  - legacy java (MJava)
        try
        {
            // get all java version
            using var response = await _httpClient.GetStreamAsync(JavaManifestServer);
            using var jsonDocument = await JsonDocument.ParseAsync(response);

            var root = jsonDocument.RootElement;
            var javaVersions = root.GetProperty(getJavaOSName()); // get os specific java version

            var latestVersionUrl = getJavaUrl(javaVersions, javaVersion);
            var legacyVersionUrl = getJavaUrl(javaVersions, MinecraftJavaPathResolver.JreLegacyVersion);

            if (!string.IsNullOrEmpty(latestVersionUrl))
            {
                var res = await _httpClient.GetStreamAsync(latestVersionUrl);
                return extractTasks(res, javaVersion);
            }
            else if (!string.IsNullOrEmpty(legacyVersionUrl) && 
                javaVersion.Component != MinecraftJavaPathResolver.JreLegacyVersion.Component)
            {
                var res = await _httpClient.GetStreamAsync(legacyVersionUrl);
                return extractTasks(res, MinecraftJavaPathResolver.JreLegacyVersion);
            }
            else
            {
                throw new Exception();
            }
        }
        catch
        {
            return await legacyJavaChecker();
        }
    }

    private string getJavaOSName()
    {
        string osName = "";

        if (_os.Name == MRule.Windows)
        {
            if (_os.Arch == "64")
                osName = "windows-x64";
            else
                osName = "windows-x86";
        }
        else if (_os.Name == MRule.Linux)
        {
            if (_os.Arch == "64")
                osName = "linux";
            else
                osName = "linux-i386";
        }
        else if (_os.Name == MRule.OSX)
        {
            osName = "mac-os";
        }

        return osName;
    }

    private string? getJavaUrl(JsonElement element, JavaVersion javaVersion)
    {
        return element
            .GetPropertyOrNull(javaVersion.Component)?
            .EnumerateArray()
            .FirstOrDefault()
            .GetPropertyOrNull("manifest")?
            .GetPropertyOrNull("url")?
            .GetString();
    }

    // compare local files with `manifest`
    private IEnumerable<LinkedTask> extractTasks(Stream stream, JavaVersion version)
    {
        var path = _javaPathResolver.GetJavaDirPath(version);
        using var s = stream;
        using var manifestDocument = JsonDocument.Parse(stream);
        var manifest = manifestDocument.RootElement;

        var files = manifestDocument.RootElement.GetPropertyOrNull("files");
        if (files == null)
            yield break;

        var objects = files.Value.EnumerateObject();
        foreach (var prop in objects)
        {
            var name = prop.Name;
            var value = prop.Value;

            var type = value.GetPropertyValue("type");
            if (type == "file")
            {
                var filePath = Path.Combine(path, name);
                filePath = IOUtil.NormalizePath(filePath);

                var file = createTask(value, name, filePath);
                if (file != null)
                    yield return file; // TODO: tryChmod
            }
            else
            {
                if (type != "directory")
                    Debug.WriteLine(type);
            }

        }
    }

    private LinkedTask? createTask(JsonElement value, string name, string filePath)
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

        var file = new TaskFile
        {
            Name = name,
            Hash = hash,
            Path = filePath,
            Url = url,
            Size = size
        };

        var checkTask = new FileCheckTask(file);
        checkTask.OnFalse = new DownloadTask(file);

        if (executable)
            checkTask.InsertNextTask(new ChmodTask(file.Path));

        return checkTask;
    }

    // legacy java checker that use MJava
    private async ValueTask<IEnumerable<LinkedTask>> legacyJavaChecker()
    {
        var legacyJavaPath = _javaPathResolver.GetJavaDirPath(MinecraftJavaPathResolver.CmlLegacyVersion);

        var mJava = new MJava(_httpClient, legacyJavaPath);
        if (mJava.CheckJavaExistence(_os))
            return Enumerable.Empty<LinkedTask>();

        var javaUrl = await mJava.GetJavaUrlAsync();
        var lzmaPath = Path.Combine(Path.GetTempPath(), "jre.lzma");
        var zipPath = Path.Combine(Path.GetTempPath(), "jre.zip");

        var file = new TaskFile
        {
            Name = "jre.lzma",
            Url = javaUrl,
            Path = lzmaPath
        };

        var download = new DownloadTask(file);
        var decompressLZMA = new LZMADecompressTask(lzmaPath, zipPath);
        var unzip = new UnzipTask(zipPath, legacyJavaPath);
        var chmod = new ChmodTask(mJava.GetBinaryPath(_os));

        return new LinkedTask[]
        {
            LinkedTask.LinkTasks(download, decompressLZMA, unzip, chmod)!
        };
    }
}