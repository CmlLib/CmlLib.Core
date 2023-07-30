using System.IO;

namespace CmlLib.Core.Files
{
    public class ModFileFactory
    {
        public ModFile GetCurseForgeModFile(string modName, string fileId)
        {
            string path = Path.Combine("mods", modName + ".jar");
            string url = $"https://www.curseforge.com/minecraft/mc-mods/{modName}/download/{fileId}/file";
            
            return new ModFile(path, url)
            {
                Name = modName
            };
        }

        public ModFile GetCurseForgeModFile(string modName, string fileId, string fileHash)
        {
            ModFile mod = GetCurseForgeModFile(modName, fileId);
            mod.Hash = fileHash;
            return mod;
        }
    }
}
