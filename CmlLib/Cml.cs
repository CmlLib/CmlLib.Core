using CmlLib.Core;
using CmlLib.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq;
using System.Diagnostics;

namespace CmlLib
{
    public class Cml
    {
        public Cml(string path)
        {
            this.Minecraft = new Minecraft(path);
        }

        public event DownloadFileChangedHandler FileChanged;
        public event ProgressChangedEventHandler ProgressChanged;

        public Minecraft Minecraft { get; private set; }
        public MProfileInfo[] ProfileInfos { get; private set; }

        private void fire(MFile kind, string name, int total, int progressed)
        {
            FileChanged?.Invoke(new DownloadFileChangedEventArgs()
            {
                FileKind = kind,
                FileName = name,
                TotalFileCount = total,
                ProgressedFileCount = progressed
            });
        }

        private void fire(DownloadFileChangedEventArgs e)
        {
            FileChanged.Invoke(e);
        }

        private void fire(int progress)
        {
            ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(progress, null));
        }

        public MProfileInfo[] UpdateProfileInfos()
        {
            ProfileInfos = MProfileInfo.GetProfiles(Minecraft);
            return ProfileInfos;
        }

        public MProfile GetProfile(string mcVersion, string forgeVersion)
        {
            return GetProfile(GetVersionNameByForge(mcVersion, forgeVersion));
        }

        public MProfile GetProfile(string versionname)
        {
            if (ProfileInfos == null || ProfileInfos.Length == 0)
                UpdateProfileInfos();

            return MProfile.FindProfile(Minecraft, ProfileInfos, versionname);
        }

        public string CheckJRE()
        {
            fire(MFile.Runtime, "java", 1, 0);

            var mjava = new MJava(Minecraft.Runtime);
            mjava.DownloadProgressChanged += (sender, e) => fire(e.ProgressPercentage);
            mjava.DownloadCompleted += (sender, e) =>
            {
                fire(MFile.Runtime, "java", 1, 1);
            };
            return mjava.CheckJava();
        }

        public string CheckForge(string mcversion, string forgeversion)
        {
            if (ProfileInfos == null || ProfileInfos.Length == 0)
                UpdateProfileInfos();

            var versionname = GetVersionNameByForge(mcversion, forgeversion);
            if (!ProfileInfos.Any(x => x.Name == versionname))
            {
                var mforge = new MForge(Minecraft);
                mforge.FileChanged += (e) => fire(e);
                mforge.InstallForge(mcversion, forgeversion);

                UpdateProfileInfos();
            }

            return versionname;
        }

        public void CheckGameFiles(MProfile profile, bool downloadAsset = true)
        {
            var downloader = new MDownloader(profile);
            downloader.ChangeFile += (e) => fire(e);
            downloader.ChangeProgress += (sender, e) => fire(e.ProgressPercentage);
            downloader.DownloadAll(downloadAsset);
        }

        public string GetVersionNameByForge(string mcVersion, string forgeVersion)
        {
            return $"{mcVersion}-forge{mcVersion}-{forgeVersion}";
        }

        public Process CreateProcess(string mcversion, string forgeversion, MLaunchOption option)
        {
            return CreateProcess(CheckForge(mcversion, forgeversion), option);
        }

        public Process CreateProcess(string versionname, MLaunchOption option)
        {
            option.StartProfile = GetProfile(versionname);
            return CreateProcess(option);
        }

        public Process CreateProcess(MLaunchOption option)
        {
            if (string.IsNullOrEmpty(option.JavaPath))
                option.JavaPath = CheckJRE();

            CheckGameFiles(option.StartProfile);

            var launch = new MLaunch(option);
            return launch.GetProcess();
        }
    }
}
