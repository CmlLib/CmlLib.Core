using System.IO;

namespace CmlLib.Core.Files
{
    public class ModFileFactory
    {
        public ModFile GetCurseForgeModFile(string modname, string fileid)
        {
            return new ModFile
            {
                Name = modname,
                Path = Path.Combine("mods", modname + ".jar"),
                Url = $"https://www.curseforge.com/minecraft/mc-mods/{modname}/download/{fileid}/file"
            };
        }

        public ModFile GetCurseForgeModFile(string modname, string fileid, string filehash)
        {
            ModFile mod = GetCurseForgeModFile(modname, fileid);
            mod.Hash = filehash;
            return mod;
        }
    }
}
