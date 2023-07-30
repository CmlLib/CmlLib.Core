using CmlLib.Core.Version;
using System.Threading.Tasks;

namespace CmlLib.Core.VersionLoader
{
    public class DefaultVersionLoader : IVersionLoader
    {
        private readonly MinecraftPath minecraftPath;
    
        public DefaultVersionLoader(MinecraftPath path)
        {
            minecraftPath = path;
        }

        public async Task<MVersionCollection> GetVersionMetadatasAsync()
        {
            var localVersionLoader = new LocalVersionLoader(minecraftPath);
            var mojangVersionLoader = new MojangVersionLoader();

            var mojangVersions = await mojangVersionLoader.GetVersionMetadatasAsync()
                .ConfigureAwait(false);
            var localVersions = await localVersionLoader.GetVersionMetadatasAsync()
                .ConfigureAwait(false);

            //below code could break the order of version list
            //mojangVersions.Merge(localVersions);

            // normal order: local versions before mojang versions
            // local 1.16.~~
            // local 1.15.~~
            // mojang 1.14.~~

            localVersions.Merge(mojangVersions);
            return localVersions;
        }
    }
}
