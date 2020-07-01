using CmlLib.Core.Version;
using CmlLib.Utils;
using System;
using System.IO;

namespace CmlLib.Core
{
    public class MNative
    {
        public MNative(MLaunchOption launchOption)
        {
            this.LaunchOption = launchOption;
        }

        public MLaunchOption LaunchOption { get; private set; }

        public void CreateNatives()
        {
            var path = ExtractNatives(LaunchOption.StartVersion);
            LaunchOption.StartVersion.NativePath = path;
        }

        private string ExtractNatives(MVersion version)
        {
            var ran = new Random();
            int random = ran.Next(10000, 99999);
            string path = Path.Combine(version.Minecraft.Versions, version.Id, "natives-" + random.ToString());
            ExtractNatives(version, path);
            return path;
        }

        private void ExtractNatives(MVersion version, string path)
        {
            Directory.CreateDirectory(path);

            foreach (var item in version.Libraries)
            {
                try
                {
                    if (item.IsNative)
                    {
                        var z = new SharpZip(item.Path);
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
                var path = Path.Combine(LaunchOption.StartVersion.Minecraft.Versions, LaunchOption.StartVersion.Id);
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
