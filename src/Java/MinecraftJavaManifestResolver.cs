using CmlLib.Core.Files;
using CmlLib.Core.Internals;
using CmlLib.Core.Rules;
using System.Text.Json;

namespace CmlLib.Core.Java;

public class MinecraftJavaManifestResolver
{
    public static string GetOSNameForJava(LauncherOSRule os)
    {
        return (os.Name, os.Arch) switch
        {
            (LauncherOSRule.Windows, "64") => "windows-x64",
            (LauncherOSRule.Windows, "32") => "windows-x86",
            (LauncherOSRule.Windows, "arm64") => "windows-arm64",
            (LauncherOSRule.Windows, _) => $"windows-{os.Arch}",
            (LauncherOSRule.Linux, "64") => "linux",
            (LauncherOSRule.Linux, "32") => "linux-i386",
            (LauncherOSRule.Linux, _) => $"linux-{os.Arch}",
            (LauncherOSRule.OSX, "64") => "mac-os",
            (LauncherOSRule.OSX, "32") => "mac-os",
            (LauncherOSRule.OSX, "arm") => "mac-os-arm64",
            (LauncherOSRule.OSX, "arm64") => "mac-os-arm64",
            (LauncherOSRule.OSX, _) => $"mac-os-{os.Arch}",
            (_, _) => $"{os.Name}-{os.Arch}"
        };
    }

    private readonly HttpClient _httpClient;
    public string ManifestServer { get; set; } = MojangServer.JavaManifest;

    public MinecraftJavaManifestResolver(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<MinecraftJavaManifestMetadata>> GetAllManifests()
    {
        using var json = await requestJsonManifest();
        var root = json.RootElement;

        var list = new List<MinecraftJavaManifestMetadata>();
        foreach (var osProp in root.EnumerateObject())
        {
            if (osProp.Value.ValueKind != JsonValueKind.Object) 
                continue;

            var components = enumerateComponents(osProp.Name, osProp.Value.EnumerateObject());
            foreach (var component in components)
            {
                list.Add(component);
            }
        }
        return list;
    }

    public async Task<IEnumerable<MinecraftJavaManifestMetadata>> GetManifestsForOS(string os)
    {
        using var json = await requestJsonManifest();
        var components = json.RootElement
            .GetPropertyOrNull(os)?
            .EnumerateObject();

        if (components == null)
            return Enumerable.Empty<MinecraftJavaManifestMetadata>();

        return enumerateComponents(os, components).ToArray();
    }

    private async Task<JsonDocument> requestJsonManifest()
    {
        using var stream = await _httpClient.GetStreamAsync(ManifestServer);
        return await JsonDocument.ParseAsync(stream);
    }

    private IEnumerable<MinecraftJavaManifestMetadata> enumerateComponents(string osName, IEnumerable<JsonProperty> components)
    {
        foreach (var componentProp in components)
        {
            var componentName = componentProp.Name;
            var manifest = componentProp.Value;

            if (manifest.ValueKind == JsonValueKind.Object)
                yield return parseManifest(osName, componentName, manifest);
            else if (manifest.ValueKind == JsonValueKind.Array)
            {
                var manifestArray = manifest.EnumerateArray();
                if (manifestArray.Any())
                    yield return parseManifest(osName, componentName, manifestArray.First());
            }
        }
    }

    private MinecraftJavaManifestMetadata parseManifest(string os, string component, JsonElement json)
    {
        return new MinecraftJavaManifestMetadata(os, component)
        {
            Metadata = json.GetPropertyOrNull("manifest")?.Deserialize<MFileMetadata>(),
            VersionName = json.GetPropertyOrNull("version")?.GetPropertyOrNull("name")?.GetString(),
            VersionReleased = json.GetPropertyOrNull("version")?.GetPropertyOrNull("released")?.GetString()
        };
    }

    public async Task<IEnumerable<MinecraftJavaFile>> GetFilesFromManifest(
        MinecraftJavaManifestMetadata manifest,
        CancellationToken cancellationToken)
    {
        var url = manifest.Metadata?.Url;
        if (string.IsNullOrEmpty(url))
            throw new ArgumentException("Url was null");
        return await GetFilesFromManifest(url, cancellationToken);
    }

    public async Task<IEnumerable<MinecraftJavaFile>> GetFilesFromManifest(
        string manifestUrl, 
        CancellationToken cancellationToken)
    {
        using var res = await _httpClient.GetAsync(manifestUrl, cancellationToken);
        using var stream = await res.Content.ReadAsStreamAsync();
        var json = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken); // should be disposed after extraction
        return parseJavaFilesAndDispose(json);
    }

    private IEnumerable<MinecraftJavaFile> parseJavaFilesAndDispose(JsonDocument _json)
    {
        using var json = _json;

        if (!json.RootElement.TryGetProperty("files", out var files))
            yield break;

        var objects = files.EnumerateObject();
        foreach (var prop in objects)
        {
            var name = prop.Name;
            var value = prop.Value;

            var downloadObj = value.GetPropertyOrNull("downloads")?.GetPropertyOrNull("raw");
            yield return new MinecraftJavaFile(name)
            {
                Type = value.GetPropertyValue("type"),
                Executable = value.GetPropertyOrNull("executable")?.GetBoolean() ?? false,
                Sha1 = downloadObj?.GetPropertyValue("sha1"),
                Size = downloadObj?.GetPropertyOrNull("size")?.GetInt64() ?? 0,
                Url = downloadObj?.GetPropertyValue("url")
            };
        }
    }
}
