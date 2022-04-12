using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using CmlLib.Core.VersionMetadata;
using CmlLib.Utils;

namespace CmlLib.Core.Installer.LiteLoader
{
    public class LiteLoaderVersionLoader : IVersionLoader
    {
        public const string LiteLoaderLibName = "com.mumfrey:liteloader";
        public const string ManifestServer = "http://dl.liteloader.com/versions/versions.json";
        
        public async Task<MVersionCollection> GetVersionMetadatasAsync()
        {
            var res = await HttpUtil.HttpClient.GetStringAsync(ManifestServer)
                .ConfigureAwait(false);

            return parseVersions(res);
        }

        private MVersionCollection parseVersions(string json)
        {
            using var jsonDocument = JsonDocument.Parse(json);
            var root = jsonDocument.RootElement; 
            
            var metadataList = new List<MVersionMetadata>();
            if (root.TryGetProperty("versions", out var versions))
            {
                foreach (var item in versions.EnumerateObject())
                {
                    var vanillaVersion = item.Name;
                    var versionObj = item.Value;

                    var libObj = versionObj.SafeGetProperty("artefacts") ?? versionObj.SafeGetProperty("snapshots");
                    var latestLiteLoader = libObj?.SafeGetProperty(LiteLoaderLibName)?.SafeGetProperty("latest");

                    if (latestLiteLoader == null)
                        continue;

                    var metadata = new LiteLoaderVersionMetadata(vanillaVersion, latestLiteLoader.Value);
                    metadata.Type = versionObj.SafeGetProperty("repo")?.GetPropertyValue("stream");

                    metadataList.Add(metadata);
                }
            }

            return new MVersionCollection(metadataList.ToArray());
        }
    }
}