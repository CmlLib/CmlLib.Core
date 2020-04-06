using System;
using System.IO;
using CmlLib.Utils;

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
            var path = ExtractNatives(LaunchOption.StartProfile);
            LaunchOption.StartProfile.NativePath = path;
        }

        private string ExtractNatives(MProfile profile)
        {
            var ran = new Random();
            int random = ran.Next(10000, 99999);
            string path = Path.Combine(profile.Minecraft.Versions, profile.Id, "natives-" + random.ToString());
            ExtractNatives(profile, path);
            return path;
        }

        private void ExtractNatives(MProfile profile, string path)
        {
            Directory.CreateDirectory(path);

            foreach (var item in profile.Libraries)
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

            profile.NativePath = path;
        }

        public void CleanNatives()
        {
            try
            {
                var path = Path.Combine(LaunchOption.StartProfile.Minecraft.Versions, LaunchOption.StartProfile.Id);
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
