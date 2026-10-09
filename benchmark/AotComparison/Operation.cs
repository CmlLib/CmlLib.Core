using System.Net;
using System.Text.Json;
using CmlLib.Core.Java;
using CmlLib.Core.ModLoaders.FabricMC;
using CmlLib.Core.ModLoaders.LiteLoader;
using CmlLib.Core.ModLoaders.QuiltMC;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using CmlLib.Core.VersionMetadata;

namespace CmlLib.Core.Benchmarks.AotComparison;

internal sealed class Operation(Func<ValueTask<long>> execute, Action? cleanup = null) : IDisposable
{
    public ValueTask<long> Execute() => execute();
    public void Dispose() => cleanup?.Invoke();

    public static async Task<Operation> Create(string name, string fixtures)
    {
        string Read(string file) => File.ReadAllText(Path.Combine(fixtures, file + ".json"));
        HttpClient Client(string json) => new(new FixtureHandler(json));
        switch (name)
        {
            case "version":
            {
                var json = Read("version");
                var options = new JsonVersionParserOptions { SkipError = false };
                return new Operation(() =>
                {
                    using var version = new JsonVersion(JsonDocument.Parse(json), options);
                    long checksum = version.Id.Length + (version.AssetIndex?.TotalSize ?? 0) + (version.Client?.Size ?? 0);
                    checksum += version.Logging?.LogFile?.Size ?? 0;
                    foreach (var library in version.Libraries)
                        checksum += (library.Artifact?.Size ?? 0) + (library.Classifiers?.Count ?? 0) + library.Rules.Count;
                    checksum += version.GetGameArguments(false).Count + version.GetJvmArguments(false).Count;
                    return ValueTask.FromResult(checksum);
                });
            }
            case "manifest":
            case "sort":
            {
                var client = Client(Read("manifest"));
                var loader = new MojangJsonVersionLoader(client, "https://fixture/manifest");
                if (name == "manifest")
                    return new Operation(async () =>
                    {
                        var versions = await loader.GetVersionMetadatasAsync();
                        return versions.Count() + (versions.LatestReleaseName?.Length ?? 0);
                    }, client.Dispose);
                var metadata = await loader.GetVersionMetadatasAsync();
                var sorter = new VersionMetadataSorter(new MVersionSortOption());
                return new Operation(() =>
                {
                    var sorted = sorter.Sort(metadata);
                    return ValueTask.FromResult((long)sorted.Length + sorted[0].Name.Length);
                }, client.Dispose);
            }
            case "java":
            {
                var client = Client(Read("java"));
                var resolver = new MinecraftJavaManifestResolver(client) { ManifestServer = "https://fixture/java" };
                return new Operation(async () =>
                {
                    var manifests = await resolver.GetAllManifests();
                    return manifests.Sum(m => m.Metadata?.Size ?? 0);
                }, client.Dispose);
            }
            case "fabric":
            {
                var client = Client(Read("loaders"));
                var installer = new FabricInstaller(client, "https://fixture");
                return new Operation(async () =>
                {
                    var loaders = await installer.GetLoaders();
                    return loaders.Count + loaders.Sum(l => l.Build);
                }, client.Dispose);
            }
            case "quilt":
            {
                var client = Client(Read("loaders"));
                var installer = new QuiltInstaller(client, "https://fixture");
                return new Operation(async () =>
                {
                    var loaders = await installer.GetLoaders();
                    return loaders.Count + loaders.Sum(l => l.Build ?? 0);
                }, client.Dispose);
            }
            case "liteloader_write":
            {
                var client = Client("{}");
                var installer = new LiteLoaderInstaller(client);
                var baseVersion = new JsonVersion(JsonDocument.Parse(Read("version")), new JsonVersionParserOptions { SkipError = false });
                var loader = new LiteLoaderVersion
                {
                    Version = "1.8.9", TweakClass = "example.Tweaker",
                    Libraries = Enumerable.Range(0, 10).Select(i => new LiteLoaderLibrary { Name = $"example:library:{i}", Url = "https://fixture/" }).ToArray()
                };
                var directory = Path.Combine(Path.GetTempPath(), "cmllib-benchmark", Guid.NewGuid().ToString("N"));
                var path = new MinecraftPath(directory);
                return new Operation(async () => (await installer.Install(loader, baseVersion, path)).Length,
                    () =>
                    {
                        baseVersion.Dispose(); client.Dispose();
                        if (Directory.Exists(directory)) Directory.Delete(directory, recursive: true);
                    });
            }
            default: throw new ArgumentException("Unknown operation: " + name);
        }
    }

    private sealed class FixtureHandler(string json) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) });
    }
}
