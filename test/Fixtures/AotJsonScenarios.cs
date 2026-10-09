using System.Net;
using System.Text.Json;
using CmlLib.Core.Java;
using CmlLib.Core.ModLoaders.FabricMC;
using CmlLib.Core.ModLoaders.QuiltMC;
using CmlLib.Core.ModLoaders.LiteLoader;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using CmlLib.Core.VersionMetadata;

namespace CmlLib.Core.Test.Fixtures;

internal static class AotJsonScenarios
{
    public static async Task ModLoaders(bool quilt, bool empty = false)
    {
        using var client = new HttpClient(new JsonHandler(request =>
        {
            if (empty) return "null";
            if (request.RequestUri!.AbsolutePath.EndsWith("/game"))
                return """[{"version":"1.20"},{"version":"snapshot","stable":false},{"version":""}]""";
            const string loader = """{"version":"0.15","maven":"example:loader:0.15","separator":".","build":7,"stable":true}""";
            return request.RequestUri.AbsolutePath.EndsWith("/loader")
                ? "[" + loader + "]"
                : "[{\"loader\":" + loader + "},{\"loader\":null},{}]";
        }));

        if (quilt)
        {
            var installer = new QuiltInstaller(client, "https://example.com");
            var supported = await installer.GetSupportedVersionNames();
            var loaders = await installer.GetLoaders();
            if (empty)
            {
                Check(supported.Count == 0 && loaders.Count == 0, "Empty Quilt response");
                return;
            }
            Check(supported.SequenceEqual(new[] { "1.20" }), "Quilt stable game filtering");
            var loader = loaders.Single();
            Check(loader.Version == "0.15" && loader.Build == 7 && loader.Maven == "example:loader:0.15", "Quilt loader metadata");
            Check((await installer.GetLoaders("1.20")).Single().Version == "0.15", "Quilt nested loaders");
            Check((await installer.GetFirstLoader("1.20"))?.Version == "0.15", "Quilt first loader");
        }
        else
        {
            var installer = new FabricInstaller(client, "https://example.com");
            var supported = await installer.GetSupportedVersionNames();
            var loaders = await installer.GetLoaders();
            if (empty)
            {
                Check(supported.Count == 0 && loaders.Count == 0, "Empty Fabric response");
                return;
            }
            Check(supported.SequenceEqual(new[] { "1.20" }), "Fabric stable game filtering");
            var loader = loaders.Single();
            Check(loader.Version == "0.15" && loader.Build == 7 && loader.Maven == "example:loader:0.15", "Fabric loader metadata");
            Check((await installer.GetLoaders("1.20")).Single().Version == "0.15", "Fabric nested loaders");
            Check((await installer.GetFirstLoader("1.20"))?.Version == "0.15", "Fabric first loader");
        }
    }

    public static async Task JavaManifest()
    {
        using var client = new HttpClient(new JsonHandler(_ => """
            {"linux":{"java-runtime":[{"manifest":{"url":"https://example.com/java.json","sha1":"hash","size":42},
            "version":{"name":"17.0.1","released":"2021-10-19"}}]}}
            """));
        var resolver = new MinecraftJavaManifestResolver(client) { ManifestServer = "https://example.com/manifest" };
        var manifest = (await resolver.GetAllManifests()).Single();
        Check(manifest.OS == "linux" && manifest.Component == "java-runtime", "Java platform/component");
        Check(manifest.Metadata?.Url == "https://example.com/java.json" && manifest.Metadata.Size == 42 && manifest.Metadata.Sha1 == "hash", "Java download metadata");
        Check(manifest.VersionName == "17.0.1" && manifest.VersionReleased == "2021-10-19", "Java version");
        Check((await resolver.GetManifestsForOS("linux")).Single().Metadata?.Size == 42, "Java OS lookup");
    }

    public static async Task LiteLoader()
    {
        using var client = new HttpClient(new JsonHandler(_ => """
            {"versions":{"1.8.9":{"artefacts":{"com.mumfrey:liteloader":{"latest":{
            "version":"1.8.9","tweakClass":"example.Tweaker","libraries":[
            {"name":"org.ow2.asm:asm-all:5.2","url":"https://example.com/"}]}}}}}}
            """));
        var installer = new LiteLoaderInstaller(client, "https://example.com/manifest");
        var loader = (await installer.GetAllLiteLoaders()).Single();
        Check(loader.BaseVersion == "1.8.9" && loader.Version == "1.8.9", "LiteLoader manifest");
        using var baseVersion = ParseVersion("""{"id":"1.8.9","minecraftArguments":"--username player"}""");
        var directory = Path.Combine(Path.GetTempPath(), "cmllib-aot", Guid.NewGuid().ToString("N"));
        try
        {
            var path = new MinecraftPath(directory);
            var id = await installer.Install(loader, baseVersion, path);
            using var profile = JsonDocument.Parse(await File.ReadAllTextAsync(path.GetVersionJsonPath(id)));
            var root = profile.RootElement;
            Check(root.GetProperty("id").GetString() == id, "LiteLoader profile ID");
            Check(root.GetProperty("type").GetString() == "release" && root.GetProperty("mainClass").GetString() == "net.minecraft.launchwrapper.Launch", "LiteLoader launch configuration");
            Check(root.GetProperty("inheritsFrom").GetString() == "1.8.9" && root.GetProperty("jar").GetString() == "1.8.9", "LiteLoader inheritance");
            Check(root.GetProperty("minecraftArguments").GetString() == "--tweakClass example.Tweaker --username player", "LiteLoader arguments");
            var libraries = root.GetProperty("libraries");
            Check(libraries.GetArrayLength() == 2 && libraries[0].GetProperty("name").GetString() == "com.mumfrey:liteloader:1.8.9", "LiteLoader libraries");
            Check(libraries[1].GetProperty("url").GetString() == "http://repo.liteloader.com/", "Legacy ASM repository");
            using var parsed = ParseVersion(root.GetRawText());
            Check(parsed.Libraries.Count == 2, "Read generated LiteLoader profile");
        }
        finally
        {
            if (Directory.Exists(directory)) Directory.Delete(directory, recursive: true);
        }
    }

    public static async Task Versions()
    {
        using var client = new HttpClient(new JsonHandler(_ => """
            {"latest":{"release":"1.20"},"versions":[{"id":"1.20","type":"release",
            "url":"https://example.com/version","time":"2023-06-07T12:00:00Z","releaseTime":"invalid"}]}
            """));
        var versions = await new MojangJsonVersionLoader(client, "https://example.com/manifest").GetVersionMetadatasAsync();
        Check(versions.LatestReleaseName == "1.20" && versions.Single().Name == "1.20", "Version manifest");
        Check(versions.ToArray(new MVersionSortOption()).Length == 1, "Version sorting");
        using var version = ParseVersion("""
            {"id":"1.20","type":1,"javaVersion":{"component":"java-runtime","majorVersion":17},
            "assetIndex":{"id":"assets","totalSize":42},"releaseTime":"invalid",
            "downloads":{"client":{"url":"https://example.com/client","size":123}},
            "logging":{"client":{"file":{"id":"log.xml"}}},
            "libraries":[{"name":"example:lib:1","downloads":{"artifact":{"size":7},
            "classifiers":{"natives-linux":{"size":8}}},"natives":{"linux":"natives-linux"},
            "rules":[{"action":"allow","os":{"name":"linux"},"features":{"demo":true}}]}]}
            """);
        Check(version.Type == "1" && version.JavaVersion?.MajorVersion == "17", "Numeric string converters");
        Check(version.ReleaseTime == DateTimeOffset.MinValue && version.AssetIndex?.TotalSize == 42, "Version date/assets");
        Check(version.Client?.Size == 123 && version.Logging?.LogFile?.Id == "log.xml", "Client/log metadata");
        var library = version.Libraries.Single();
        Check(library.Artifact?.Size == 7 && library.Classifiers?["natives-linux"].Size == 8, "Library metadata");
        Check(library.Natives?["linux"] == "natives-linux" && library.Rules.Single().Features?["demo"] == true, "Natives/rules");
    }

    private static JsonVersion ParseVersion(string json) =>
        new(JsonDocument.Parse(json), new JsonVersionParserOptions { SkipError = false });

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private sealed class JsonHandler(Func<HttpRequestMessage, string> response) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(response(request)) });
        }
    }
}
