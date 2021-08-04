using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using CmlLib.Core.VersionLoader.LiteLoader;

namespace CmlLib.Core.Installer
{
    public class LiteLoaderInstaller
    {
        public LiteLoaderInstaller(MinecraftPath path)
        {
            this.minecraftPath = path;
        }

        private MinecraftPath minecraftPath;
        private MVersionCollection? liteLoaderVersions = null;

        public static string GetVersionName(string loaderVersion, string baseVersion)
        {
            loaderVersion = loaderVersion.Replace("LiteLoader", "");
            return $"{loaderVersion}-LiteLoader{baseVersion}";
        }

        public async Task<string[]> GetAllLiteLoaderVersions()
        {
            var llVersionLoader = new LiteLoaderVersionLoader();
            liteLoaderVersions = await llVersionLoader.GetVersionMetadatasAsync()
                .ConfigureAwait(false);

            return liteLoaderVersions.Select(x => x.Name).ToArray();
        }

        // vanilla
        public async Task<string> Install(string liteLoaderVersion)
        {
            var localVersionLoader = new LocalVersionLoader(minecraftPath);
            var versions = await localVersionLoader.GetVersionMetadatasAsync()
                .ConfigureAwait(false);

            var vanilla = versions.GetVersionMetadata("");
            return await Install(liteLoaderVersion, vanilla).ConfigureAwait(false);
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