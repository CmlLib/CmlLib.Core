using CmlLib.Core;
using CmlLib.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq;
using System.Diagnostics;
using CmlLib.Core.Downloader;
using CmlLib.Core.Version;

namespace CmlLib.Core
{
    public class CMLauncher
    {
        public CMLauncher(string path)
        {
            this.Minecraft = new MinecraftPath(path);
        }

        public CMLauncher(MinecraftPath mc)
        {
            this.Minecraft = mc;
        }

        public event DownloadFileChangedHandler FileChanged;
        public event ProgressChangedEventHandler ProgressChanged;

        public MinecraftPath Minecraft { get; private set; }
        public MVersionCollection Versions { get; private set; }

        private void fire(MFile kind, string name, int total, int progressed)
        {
            FileChanged?.Invoke(new DownloadFileChangedEventArgs(kind, name, total, progressed));
        }

        private void fire(DownloadFileChangedEventArgs e)
        {
            FileChanged?.Invoke(e);
        }

        private void fire(int progress)
        {
            ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(progress, null));
        }

        public MVersionCollection UpdateVersions()
        {
            Versions = MVersionLoader.GetVersionMetadatas(Minecraft);
            return Versions;
        }

        public MVersionCollection GetVersions()
        {
            if (Versions == null)
                Versions = UpdateVersions();

            return Versions;
        }

        public MVersion GetVersion(string mcVersion, string forgeVersion)
        {
            return GetVersion(GetVersionNameByForge(mcVersion, forgeVersion));
        }

        public MVersion GetVersion(string versionname)
        {
            if (Versions == null)
                UpdateVersions();

            return Versions.GetVersion(versionname);
        }

        public string CheckJRE()
        {
            fire(MFile.Runtime, "java", 1, 0);

            var mjava = new MJava(Minecraft.Runtime);
            mjava.ProgressChanged += (sender, e) => fire(e.ProgressPercentage);
            mjava.DownloadCompleted += (sender, e) =>
            {
                fire(MFile.Runtime, "java", 1, 1);
            };
            return mjava.CheckJava();
        }

        public string CheckForge(string mcversion, string forgeversion)
        {
            if (Versions == null)
                UpdateVersions();

            var versionname = GetVersionNameByForge(mcversion, forgeversion);
            if (!Versions.Any(x => x.Name == versionname))
            {
                var mforge = new MForge(Minecraft);
                mforge.FileChanged += (e) => fire(e);
                mforge.InstallForge(mcversion, forgeversion);

                UpdateVersions();
            }

            return versionname;
        }

        public void CheckGameFiles(MVersion version, bool downloadAsset = true)
        {
            var downloader = new MDownloader(version);
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
            option.StartVersion = GetVersion(versionname);
            return CreateProcess(option);
        }

        public Process CreateProcess(MLaunchOption option)
        {
            if (string.IsNullOrEmpty(option.JavaPath))
                option.JavaPath = CheckJRE();

            CheckGameFiles(option.StartVersion);

            var launch = new MLaunch(option);
            return launch.GetProcess();
        }
    }
}
