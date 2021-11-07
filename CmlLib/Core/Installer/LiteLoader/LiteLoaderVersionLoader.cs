using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using CmlLib.Core.VersionMetadata;
using Newtonsoft.Json.Linq;

namespace CmlLib.Core.Installer.LiteLoader
{
    public class LiteLoaderVersionLoader : IVersionLoader
    {
        private const string LiteLoaderLibName = "com.mumfrey:liteloader";
        private const string ManifestServer = "http://dl.liteloader.com/versions/versions.json";
        
        public async Task<MVersionCollection> GetVersionMetadatasAsync()
        {
            using var wc = new WebClient();
            var res = await wc.DownloadStringTaskAsync(ManifestServer)
                .ConfigureAwait(false);

            return parseVersions(res);
        }

        public MVersionCollection GetVersionMetadatas()
        {
            using var wc = new WebClient();
            var res = wc.DownloadString(ManifestServer);

            return parseVersions(res);
        }

        private MVersionCollection parseVersions(string json)
        {
            var job = JObject.Parse(json);
            var versions = job["versions"] as JObject;
            
            List<MVersionMetadata> metadataList = new List<MVersionMetadata>();
            if (versions != null)
            {
                foreach (var item in versions)
                {
                    var vanillaVersion = item.Key;
                    var versionObj = item.Value;

                    var type = versionObj?["repo"]?["stream"]?.ToString();

                    var libObj = versionObj?["artefacts"] ?? versionObj?["snapshots"];
                    var latestLLN = libObj?[LiteLoaderLibName]?["latest"];

                    if (latestLLN == null)
                        continue;

                    var tweakClass = latestLLN["tweakClass"]?.ToString();
                    var libraries = latestLLN["libraries"] as JArray;
                    var llVersion = latestLLN["version"]?.ToString();

                    if (libraries != null)
                    {
                        foreach (var lib in libraries)
                        {
                            // asm-all:5.2 is only available on LiteLoader server
                            var libName = lib["name"]?.ToString();
                            if (libName == "org.ow2.asm:asm-all:5.2")
                                lib["url"] = "http://repo.liteloader.com/";
                        }
                    }

                    var llName = $"{LiteLoaderLibName}:{llVersion}";
                    var versionName = $"LiteLoader{vanillaVersion}";

                    var metadata = new LiteLoaderVersionMetadata(
                        versionName, vanillaVersion, tweakClass, libraries, llName);
                    metadata.Type = type;

                    metadataList.Add(metadata);
                }
            }

            return new MVersionCollection(metadataList.ToArray());
        }
    }
}