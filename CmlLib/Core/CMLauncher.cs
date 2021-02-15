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
using CmlLib.Core.Files;
using System.Threading.Tasks;

namespace CmlLib.Core
{
    public class CMLauncher
    {
        public CMLauncher(string path) : this(new MinecraftPath(path))
        {
        }

        public CMLauncher(MinecraftPath mc)
        {
            this.MinecraftPath = mc;

            GameFileCheckers = new FileCheckerCollection();

            FileDownloader = new SequenceDownloader();
            FileDownloader.ChangeFile += (e) => FileChanged?.Invoke(e);
            FileDownloader.ChangeProgress += (s, e) => ProgressChanged?.Invoke(this, e);
        }

        public event DownloadFileChangedHandler FileChanged;
        public event ProgressChangedEventHandler ProgressChanged;
        public event EventHandler<string> LogOutput;

        public MinecraftPath MinecraftPath { get; private set; }
        public MVersionCollection Versions { get; private set; }

        public FileCheckerCollection GameFileCheckers { get; private set; }

        public IDownloader FileDownloader { get; set; }

        public MVersionCollection UpdateVersions()
        {
            Versions = new MVersionLoader().GetVersionMetadatas(MinecraftPath);
            return Versions;
        }

        public MVersionCollection GetAllVersions()
        {
            if (Versions == null)
                Versions = UpdateVersions();

            return Versions;
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

            var mjava = new MJava(MinecraftPath.Runtime);
            mjava.ProgressChanged += (sender, e) => fire(e.ProgressPercentage);
            mjava.DownloadCompleted += (sender, e) =>
            {
                fire(MFile.Runtime, "java", 1, 1);
            };
            return mjava.CheckJava();
        }

        public string CheckForge(string mcversion, string forgeversion, string java)
        {
            if (Versions == null)
                UpdateVersions();

            var forgeNameOld = MForge.GetOldForgeName(mcversion, forgeversion);
            var forgeName = MForge.GetForgeName(mcversion, forgeversion);

            var exist = false;
            var name = "";
            foreach (var item in Versions)
            {
                if (item.Name == forgeName)
                {
                    exist = true;
                    name = forgeName;
                    break;
                }
                else if (item.Name == forgeNameOld)
                {
                    exist = true;
                    name = forgeNameOld;
                    break;
                }
            }

            if (!exist)
            {
                var mforge = new MForge(MinecraftPath, java);
                mforge.FileChanged += (e) => fire(e);
                mforge.InstallerOutput += (s, e) => LogOutput?.Invoke(this, e);
                name = mforge.InstallForge(mcversion, forgeversion);

                UpdateVersions();
            }

            return name;
        }

        public DownloadFile[] CheckLostGameFiles(MVersion version)
        {
            var lostFiles = new List<DownloadFile>();
            foreach (IFileChecker checker in this.GameFileCheckers)
            {
                DownloadFile[] files = checker.CheckFiles(MinecraftPath, version);
                if (files != null)
                    lostFiles.AddRange(files);
            }

            return lostFiles.ToArray();
        }

        public async Task DownloadGameFiles(DownloadFile[] files)
        {
            if (this.FileDownloader == null)
                throw new ArgumentNullException("this.FileDownloader");

            await FileDownloader.DownloadFiles(files).ConfigureAwait(false);
        }

        public void CheckAndDownload(MVersion version)
        {
            CheckAndDownloadAsync(version).Wait();
        }

        public async Task CheckAndDownloadAsync(MVersion version)
        {
            foreach (IFileChecker checker in this.GameFileCheckers)
            {
                DownloadFile[] files = checker.CheckFiles(MinecraftPath, version);

                if (files == null || files.Length == 0)
                    continue;

                await DownloadGameFiles(files);
            }
        }

        public Process CreateProcess(string mcversion, string forgeversion, MLaunchOption option)
        {
            if (string.IsNullOrEmpty(option.JavaPath))
                option.JavaPath = CheckJRE();

            CheckAndDownload(GetVersion(mcversion));

            var versionName = CheckForge(mcversion, forgeversion, option.JavaPath);
            UpdateVersions();

            return CreateProcess(versionName, option);
        }

        public Process CreateProcess(string versionname, MLaunchOption option)
        {
            option.StartVersion = GetVersion(versionname);
            CheckAndDownload(option.StartVersion);
            return CreateProcess(option);
        }

        public async Task<Process> CreateProcessAsync(string versionname, MLaunchOption option)
        {
            option.StartVersion = GetVersion(versionname);
            await CheckAndDownloadAsync(option.StartVersion);
            return CreateProcess(option);
        }

        public Process CreateProcess(MLaunchOption option)
        {
            if (option.Path == null)
                option.Path = MinecraftPath;

            if (string.IsNullOrEmpty(option.JavaPath))
                option.JavaPath = CheckJRE();

            var launch = new MLaunch(option);
            return launch.GetProcess();
        }

        public Process Launch(string versionname, MLaunchOption option)
        {
            Process process = CreateProcess(versionname, option);
            process.Start();
            return process;
        }

        public async Task<Process> LaunchAsync(string versionname, MLaunchOption option)
        {
            Process process = await CreateProcessAsync(versionname, option);
            process.Start();
            return process;
        }

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
    }
}
