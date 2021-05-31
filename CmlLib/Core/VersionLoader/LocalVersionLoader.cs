using CmlLib.Core.Version;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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
            return Task.Run(GetVersionMetadatas);
        }

        private List<MVersionMetadata> getFromLocal(MinecraftPath path)
        {
            var dirs = new DirectoryInfo(path.Versions).GetDirectories();
            var arr = new List<MVersionMetadata>(dirs.Length);

            for (int i = 0; i < dirs.Length; i++)
            {
                var dir = dirs[i];
                var filepath = Path.Combine(dir.FullName, dir.Name + ".json");
                if (File.Exists(filepath))
                {
                    var info = new MVersionMetadata();
                    info.IsLocalVersion = true;
                    info.Name = dir.Name;
                    info.Path = filepath;
                    info.Type = "local";
                    info.MType = MVersionType.Custom;
                    arr.Add(info);
                }
            }

            return arr;
        }
    }
}
