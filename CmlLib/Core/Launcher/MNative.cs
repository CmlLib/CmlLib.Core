using CmlLib.Core.Version;
using CmlLib.Utils;
using System.IO;

namespace CmlLib.Core
{
    public class MNative
    {
        public MNative(MinecraftPath gamePath, MVersion version)
        {
            this.version = version;
            this.gamePath = gamePath;
        }

        private readonly MVersion version;
        private readonly MinecraftPath gamePath;

        [MethodTimer.Time]
        public string ExtractNatives()
        {
            string path = gamePath.GetNativePath(version.Id);
            Directory.CreateDirectory(path);

            if (version.Libraries == null) return path;
            
            foreach (var item in version.Libraries)
            {
                // do not ignore exception
                if (item.IsRequire && item.IsNative && !string.IsNullOrEmpty(item.Path))
                {
                    string zPath = Path.Combine(gamePath.Library, item.Path);
                    if (File.Exists(zPath))
                    {
                        var z = new SharpZip(zPath);
                        z.Unzip(path);
                    }
                }
            }

            return path;
        }

        public void CleanNatives()
        {

            try
            {
                string path = gamePath.GetNativePath(version.Id);
                DirectoryInfo di = new DirectoryInfo(path);

                if (!di.Exists)
                    return;

                foreach (var item in di.GetFiles())
                {
                    item.Delete();
                }
            }
            catch
            {
                // ignore exception
                // will be overwriten to new file
            }
        }
    }
}
