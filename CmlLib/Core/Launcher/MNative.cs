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
            var ran = new Random();
            int random = ran.Next(10000, 99999);
            string path = Path.Combine(gamePath.Versions, version.Id, "natives-" + random.ToString());
            ExtractNatives(path);
            return path;
        }

        private void ExtractNatives(string path)
        {
            Directory.CreateDirectory(path);

            foreach (var item in version.Libraries)
            {
                try
                {
                    if (item.IsNative)
                    {
                        var z = new SharpZip(Path.Combine(gamePath.Library, item.Path));
                        z.Unzip(path);
                    }
                }
                catch { }
            }

            version.NativePath = path;
        }

        public void CleanNatives()
        {
            try
            {
                var path = Path.Combine(gamePath.Versions, version.Id);
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (var item in di.GetDirectories())
                {
                    if (item.Name.Contains("natives"))
                        IOUtil.DeleteDirectory(item.FullName);
                }
            }
            catch { }
        }
    }
}
