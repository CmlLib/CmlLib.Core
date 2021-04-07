using CmlLib.Core.Version;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CmlLib.Core.VersionLoader
{
    public class DefaultVersionLoader : IVersionLoader
    {
        public DefaultVersionLoader(MinecraftPath path)
        {
            MinecraftPath = path;
        }

        protected MinecraftPath MinecraftPath;

        public MVersionCollection GetVersionMetadatas()
        {
            var localVersionLoader = new LocalVersionLoader(MinecraftPath);
            var mojangVersionLoader = new MojangVersionLoader();

            var mojangVersions = mojangVersionLoader.GetVersionMetadatas();
            var localVersions = localVersionLoader.GetVersionMetadatas();

            localVersions.Merge(mojangVersions);
            return localVersions;
        }

        public async Task<MVersionCollection> GetVersionMetadatasAsync()
        {
            var localVersionLoader = new LocalVersionLoader(MinecraftPath);
            var mojangVersionLoader = new MojangVersionLoader();

            var mojangVersions = await mojangVersionLoader.GetVersionMetadatasAsync();
            var localVersions = await localVersionLoader.GetVersionMetadatasAsync();

            localVersions.Merge(mojangVersions);
            return localVersions;
        }
    }
}
