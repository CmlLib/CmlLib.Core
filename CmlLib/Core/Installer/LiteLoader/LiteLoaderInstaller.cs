using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using CmlLib.Core.VersionMetadata;

namespace CmlLib.Core.Installer.LiteLoader
{
    // 1.8.9 freezing
    public class LiteLoaderInstaller
    {
        public LiteLoaderInstaller(MinecraftPath path)
        {
            this.minecraftPath = path;
        }

        private readonly MinecraftPath minecraftPath;
        private MVersionCollection? liteLoaderVersions;

        public static string GetVersionName(string loaderVersion, string baseVersion)
        {
            loaderVersion = loaderVersion.Replace("LiteLoader", "");
            return $"{loaderVersion}-LiteLoader{baseVersion}";
        }

        public async Task<LiteLoaderVersionMetadata[]> GetAllLiteLoaderVersions()
        {
            var llVersionLoader = new LiteLoaderVersionLoader();
            liteLoaderVersions = await llVersionLoader.GetVersionMetadatasAsync()
                .ConfigureAwait(false);

            return liteLoaderVersions
                .Select(x => x as LiteLoaderVersionMetadata)
                .Where(x => x != null)
                .ToArray()!;
        }

        // vanilla
        public async Task<string> Install(string liteLoaderVersion)
        {
            var localVersionLoader = new LocalVersionLoader(minecraftPath);
            var localVersions = await localVersionLoader.GetVersionMetadatasAsync()
                .ConfigureAwait(false);
            
            liteLoaderVersions = await localVersionLoader.GetVersionMetadatasAsync()
                .ConfigureAwait(false);

            var liteLoader = liteLoaderVersions?.GetVersionMetadata(liteLoaderVersion) as LiteLoaderVersionMetadata;
            if (liteLoader == null)
                throw new KeyNotFoundException(liteLoaderVersion);

            var vanillaVersionName = liteLoader.VanillaVersionName;
            var vanillaVersion = await localVersions.GetVersionAsync(vanillaVersionName)
                .ConfigureAwait(false);

            if (vanillaVersion == null)
                throw new KeyNotFoundException(vanillaVersionName);

            return liteLoader.Install(minecraftPath, vanillaVersion);
        }
        
        public Task<string> Install(string liteLoaderVersion, MVersionMetadata target)
        {
            return Install(liteLoaderVersion, target.GetVersion());
        }

        public async Task<string> Install(string liteLoaderVersion, MVersion target)
        {
            if (liteLoaderVersions == null)
                await GetAllLiteLoaderVersions().ConfigureAwait(false);

            var liteLoader = liteLoaderVersions?.GetVersionMetadata(liteLoaderVersion) as LiteLoaderVersionMetadata;
            if (liteLoader == null)
                throw new KeyNotFoundException(liteLoaderVersion);

            return liteLoader.Install(minecraftPath, target);
        }
    }
}