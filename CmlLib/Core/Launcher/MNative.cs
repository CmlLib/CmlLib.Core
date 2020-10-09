using CmlLib.Core.Version;
using CmlLib.Utils;
using System;
using System.IO;

namespace CmlLib.Core
{
    public class MNative
    {
        public MNative(MinecraftPath _path, MVersion _version)
        {
            version = _version;
            gamePath = _path;
        }

        MVersion version;
        MinecraftPath gamePath;

        public string ExtractNatives()
        {
            var path = gamePath.GetNativePath(version.Id);
            Directory.CreateDirectory(path);

            foreach (var item in version.Libraries)
            {
                try
                {
                    if (item.IsRequire && item.IsNative)
                    {
                        var z = new SharpZip(Path.Combine(gamePath.Library, item.Path));
                        z.Unzip(path);
                    }
                }
                catch { }
            }

            return path;
        }

        public void CleanNatives()
        {
            try
            {
                var path = gamePath.GetNativePath(version.Id);
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (var item in di.GetFiles())
                {
                    item.Delete();
                }
            }
            catch { }
        }
    }
}
