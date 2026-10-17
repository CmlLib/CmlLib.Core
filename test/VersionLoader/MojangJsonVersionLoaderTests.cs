using System.Net;
using CmlLib.Core.VersionLoader;

namespace CmlLib.Core.Test.VersionLoader;

public class MojangJsonVersionLoaderTests
{
    private const string Manifest = """
        {
            "latest": { "release": "1.20", "snapshot": "23w18a" },
            "versions": [
                {
                    "id": "1.20",
                    "type": "release",
                    "url": "https://example.com/1.20.json",
                    "sha1": "manifest-hash",
                    "size": 123,
                    "time": "2023-06-07T12:00:00Z",
                    "releaseTime": "invalid-date",
                    "complianceLevel": 1
                }
            ]
        }
        """;

    [Fact]
    public async Task parse_manifest_metadata_and_dates()
    {
        using var client = new HttpClient(new ManifestHandler());
        var loader = new MojangJsonVersionLoader(client, "https://example.com/manifest.json");

        var manifest = await loader.GetManifestAsync();

        Assert.NotNull(manifest);
        Assert.Equal("1.20", manifest.Latest?.Release);
        Assert.Equal("23w18a", manifest.Latest?.Snapshot);
        var version = Assert.Single(manifest.Versions);
        Assert.Equal("1.20", version.Id);
        Assert.Equal("release", version.Type);
        Assert.Equal("https://example.com/1.20.json", version.Url);
        Assert.Equal("manifest-hash", version.Sha1);
        Assert.Equal(123, version.Size);
        Assert.Equal(1, version.ComplianceLevel);
        Assert.Equal(new DateTimeOffset(2023, 6, 7, 12, 0, 0, TimeSpan.Zero), version.Time);
        Assert.Equal(DateTimeOffset.MinValue, version.ReleaseTime);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task load_version_collection_from_remote_or_cached_manifest(bool useCachedManifest)
    {
        var directory = Path.Combine(Path.GetTempPath(), "cmllib-manifest-tests", Guid.NewGuid().ToString("N"));
        var path = new MinecraftPath(directory);
        Directory.CreateDirectory(path.Versions);
        var manifestPath = Path.Combine(path.Versions, "version_manifest_v2.json");

        try
        {
            if (useCachedManifest)
                await File.WriteAllTextAsync(manifestPath, Manifest);
            using var client = new HttpClient(new ManifestHandler(useCachedManifest));
            var loader = new MojangJsonVersionLoaderV2(path, client, "https://example.com/manifest.json")
            {
                UseLocalManifestWhenError = useCachedManifest
            };

            var versions = await loader.GetVersionMetadatasAsync();

            Assert.Equal("1.20", versions.LatestReleaseName);
            Assert.Equal("23w18a", versions.LatestSnapshotName);
            Assert.Equal("1.20", Assert.Single(versions).Name);
            Assert.Equal(Manifest, await File.ReadAllTextAsync(manifestPath));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private sealed class ManifestHandler(bool fail = false) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (fail)
                throw new HttpRequestException("Manifest endpoint unavailable");
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(Manifest)
            });
        }
    }
}
