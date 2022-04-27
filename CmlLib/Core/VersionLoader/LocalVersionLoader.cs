using CmlLib.Core.Version;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CmlLib.Core.VersionMetadata;

namespace CmlLib.Core.VersionLoader
{
    public class LocalVersionLoader : IVersionLoader
    {
        public LocalVersionLoader(MinecraftPath path)
        {
            minecraftPath = path;
        }

        private readonly MinecraftPath minecraftPath;

        public MVersionCollection GetVersionMetadatas()
        {
            var list = getFromLocal(minecraftPath).ToArray();
            return new MVersionCollection(list, minecraftPath);
        }

        public Task<MVersionCollection> GetVersionMetadatasAsync()
        {
            return Task.FromResult(GetVersionMetadatas());
        }

        private List<MVersionMetadata> getFromLocal(MinecraftPath path)
        {
            var versionDirectory = new DirectoryInfo(path.Versions);
            if (!versionDirectory.Exists)
                return new List<MVersionMetadata>();
            
            var dirs = versionDirectory.GetDirectories();
            var arr = new List<MVersionMetadata>(dirs.Length);

            foreach (var dir in dirs)
            {
                var filepath = Path.Combine(dir.FullName, dir.Name + ".json");
                if (!File.Exists(filepath)) continue;
                
                var info = new LocalVersionMetadata(dir.Name)
                {
                    Path = filepath,
                    Type = "local",
                    MType = MVersionType.Custom
                };
                arr.Add(info);
            }

            return arr;
        }
    }
}
