using CmlLib.Core.Downloader;
using CmlLib.Core.Files;
using CmlLib.Core.Installer;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
            FileDownloader = new AsyncParallelDownloader();
            VersionLoader = new DefaultVersionLoader(MinecraftPath);
        }

        public event DownloadFileChangedHandler FileChanged;
        public event ProgressChangedEventHandler ProgressChanged;
        public event EventHandler<string> LogOutput;

        public MinecraftPath MinecraftPath { get; private set; }
        public MVersionCollection Versions { get; private set; }

        public IVersionLoader VersionLoader { get; set; }
        public FileCheckerCollection GameFileCheckers { get; private set; }

        private IDownloader _fileDownloader;
        public IDownloader FileDownloader
        {
            get => _fileDownloader;
            set
            {
                if (_fileDownloader != null)
                {
                    _fileDownloader.ChangeFile -= fireFileChangeEvent;
                    _fileDownloader.ChangeProgress -= FileDownloader_ChangeProgress;
                }

                _fileDownloader = value;

                if (_fileDownloader != null)
                {
                    _fileDownloader.ChangeFile += fireFileChangeEvent;
                    _fileDownloader.ChangeProgress += FileDownloader_ChangeProgress;
                }
            }
        }

        private void FileDownloader_ChangeProgress(object sender, ProgressChangedEventArgs e)
        {
            ProgressChanged?.Invoke(this, e);
        }

        public MVersionCollection GetAllVersions()
        {
            Versions = VersionLoader.GetVersionMetadatas();
            return Versions;
        }

        public async Task<MVersionCollection> GetAllVersionsAsync()
        {
            Versions = await VersionLoader.GetVersionMetadatasAsync();
            return Versions;
        }

        public MVersion GetVersion(string versionname)
        {
            if (Versions == null)
                GetAllVersions();

            return Versions.GetVersion(versionname);
        }

        public async Task<MVersion> GetVersionAsync(string versionname)
        {
            if (Versions == null)
                await GetAllVersionsAsync();

            var version = await Task.Run(() => Versions.GetVersion(versionname));
            return version;
        }

        public string CheckJRE()
        {
            fireFileChangeEvent(MFile.Runtime, "java", 2, 0);

            var mjava = new MJava(MinecraftPath.Runtime);
            mjava.ProgressChanged += (sender, e) => fireProgressChangeEvent(e.ProgressPercentage);
            mjava.DownloadCompleted += (sender, e) =>
            {
                fireFileChangeEvent(MFile.Runtime, "java", 2, 1);
            };
            var j = mjava.CheckJava();
            fireFileChangeEvent(MFile.Runtime, "java", 2, 2);
            return j;
        }

        public async Task<string> CheckJREAsync()
        {
            fireFileChangeEvent(MFile.Runtime, "java", 2, 0);

            var mjava = new MJava(MinecraftPath.Runtime);
            mjava.ProgressChanged += (sender, e) => fireProgressChangeEvent(e.ProgressPercentage);
            mjava.DownloadCompleted += (sender, e) =>
            {
                fireFileChangeEvent(MFile.Runtime, "java", 2, 1);
            };
            var j = await mjava.CheckJavaAsync();
            fireFileChangeEvent(MFile.Runtime, "java", 2, 2);
            return j;
        }

        public string CheckForge(string mcversion, string forgeversion, string java)
        {
            if (Versions == null)
                GetAllVersions();

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
                mforge.FileChanged += (e) => fireFileChangeEvent(e);
                mforge.InstallerOutput += (s, e) => LogOutput?.Invoke(this, e);
                name = mforge.InstallForge(mcversion, forgeversion);

                GetAllVersions();
            }

            return name;
        }

        public DownloadFile[] CheckLostGameFiles(MVersion version)
        {
            return CheckLostGameFilesTaskAsync(version).GetAwaiter().GetResult();
        }

        public async Task<DownloadFile[]> CheckLostGameFilesTaskAsync(MVersion version)
        {
            var lostFiles = new List<DownloadFile>();
            foreach (IFileChecker checker in this.GameFileCheckers)
            {
                checker.ChangeFile += fireFileChangeEvent;

                DownloadFile[] files = await checker.CheckFilesTaskAsync(MinecraftPath, version);
                if (files != null)
                    lostFiles.AddRange(files);

                checker.ChangeFile -= fireFileChangeEvent;
            }

            return lostFiles.ToArray();
        }

        public async Task DownloadGameFiles(DownloadFile[] files)
        {
            if (this.FileDownloader == null)
                throw new ArgumentNullException(nameof(this.FileDownloader));
            
            await FileDownloader.DownloadFiles(files);
        }

        public void CheckAndDownload(MVersion version)
        {
            CheckAndDownloadAsync(version).GetAwaiter().GetResult();
        }

        public async Task CheckAndDownloadAsync(MVersion version)
        {
            foreach (var checker in this.GameFileCheckers)
            {
                checker.ChangeFile += fireFileChangeEvent;

                DownloadFile[] files = await checker.CheckFilesTaskAsync(MinecraftPath, version);

                checker.ChangeFile -= fireFileChangeEvent;

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

            return CreateProcess(versionName, option);
        }

        [MethodTimer.Time]
        public Process CreateProcess(string versionname, MLaunchOption option)
        {
            option.StartVersion = GetVersion(versionname);

            if (this.FileDownloader != null)
                CheckAndDownload(option.StartVersion);

            return CreateProcess(option);
        }

        [MethodTimer.Time]
        public async Task<Process> CreateProcessAsync(string versionname, MLaunchOption option)
        {
            option.StartVersion = await GetVersionAsync(versionname);

            if (this.FileDownloader != null)
                await CheckAndDownloadAsync(option.StartVersion);

            return await CreateProcessAsync(option);
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
        
        public async Task<Process> CreateProcessAsync(MLaunchOption option)
        {
            if (option.Path == null)
                option.Path = MinecraftPath;

            if (string.IsNullOrEmpty(option.JavaPath))
                option.JavaPath = await CheckJREAsync();

            var launch = new MLaunch(option);
            return await Task.Run(launch.GetProcess);
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

        private void fireFileChangeEvent(MFile kind, string name, int total, int progressed)
        {
            FileChanged?.Invoke(new DownloadFileChangedEventArgs(kind, name, total, progressed));
        }

        private void fireFileChangeEvent(DownloadFileChangedEventArgs e)
        {
            FileChanged?.Invoke(e);
        }

        private void fireProgressChangeEvent(int progress)
        {
            ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(progress, null));
        }
    }
}
