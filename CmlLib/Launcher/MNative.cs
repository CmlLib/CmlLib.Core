using System;
using System.IO;
using CmlLib.Launcher;
using Ionic.Zip;

namespace CmlLib.Launcher
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
            string path = Minecraft.Versions + profile.Id + "\\natives-" + random.ToString();
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
                        using (var zip = ZipFile.Read(item.Path))
                        {
                            zip.ExtractAll(path, ExtractExistingFileAction.OverwriteSilently);
                        }
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
                DirectoryInfo di = new DirectoryInfo(Minecraft.Versions + LaunchOption.StartProfile.Id);
                foreach (var item in di.GetDirectories("native*"))
                {
                    DeleteDirectory(item.FullName);
                }
            }
            catch { }
        }

        private void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, true);
        }
    }
}
