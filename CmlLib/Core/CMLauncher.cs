using CmlLib.Core.Downloader;
using CmlLib.Core.Files;
using CmlLib.Core.Installer;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CmlLib.Core.Java;

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

            pFileChanged = new Progress<DownloadFileChangedEventArgs>(
                e => FileChanged?.Invoke(e));
            pProgressChanged = new Progress<ProgressChangedEventArgs>(
                e => ProgressChanged?.Invoke(this, e));
        }

        public event DownloadFileChangedHandler? FileChanged;
        public event ProgressChangedEventHandler? ProgressChanged;
        public event EventHandler<string>? LogOutput;
        
        private readonly IProgress<DownloadFileChangedEventArgs> pFileChanged;
        private readonly IProgress<ProgressChangedEventArgs> pProgressChanged;

        public MinecraftPath MinecraftPath { get; private set; }
        public MVersionCollection? Versions { get; private set; }

        public IVersionLoader VersionLoader { get; set; }
        public FileCheckerCollection GameFileCheckers { get; private set; }

        public IDownloader? FileDownloader { get; set; }

        public MVersionCollection GetAllVersions()
        {
            Versions = VersionLoader.GetVersionMetadatas();
            return Versions;
        }

        public async Task<MVersionCollection> GetAllVersionsAsync()
        {
            Versions = await VersionLoader.GetVersionMetadatasAsync()
                .ConfigureAwait(false);
            return Versions;
        }

        public MVersion GetVersion(string versionname)
        {
            if (Versions == null)
                GetAllVersions();

            return Versions!.GetVersion(versionname);
        }

        public async Task<MVersion> GetVersionAsync(string versionname)
        {
            if (Versions == null)
                await GetAllVersionsAsync().ConfigureAwait(false);

            var version = await Versions!.GetVersionAsync(versionname)
                .ConfigureAwait(false);
            return version;
        }
        
        public string CheckForge(string mcversion, string forgeversion, string java)
        {
            if (Versions == null)
                GetAllVersions();

            var forgeNameOld = MForge.GetOldForgeName(mcversion, forgeversion);
            var forgeName = MForge.GetForgeName(mcversion, forgeversion);

            var exist = false;
            var name = "";
            foreach (var item in Versions!)
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
                mforge.FileChanged += (e) => FileChanged?.Invoke(e);
                mforge.InstallerOutput += (s, e) => LogOutput?.Invoke(this, e);
                name = mforge.InstallForge(mcversion, forgeversion);

                GetAllVersions();
            }

            return name;
        }

        public DownloadFile[] CheckLostGameFiles(MVersion version)
        {
            var lostFiles = new List<DownloadFile>();
            foreach (IFileChecker checker in this.GameFileCheckers)
            {
                DownloadFile[]? files = checker.CheckFiles(MinecraftPath, version, pFileChanged);
                if (files != null)
                    lostFiles.AddRange(files);
            }

            return lostFiles.ToArray();
        }

        public async Task<DownloadFile[]> CheckLostGameFilesTaskAsync(MVersion version)
        {
            var lostFiles = new List<DownloadFile>();
            foreach (IFileChecker checker in this.GameFileCheckers)
            {
                DownloadFile[]? files = await checker.CheckFilesTaskAsync(MinecraftPath, version, pFileChanged)
                    .ConfigureAwait(false);
                if (files != null)
                    lostFiles.AddRange(files);
            }

            return lostFiles.ToArray();
        }

        public async Task DownloadGameFiles(DownloadFile[] files)
        {
            if (this.FileDownloader == null)
                return;

            await FileDownloader.DownloadFiles(files, pFileChanged, pProgressChanged)
                .ConfigureAwait(false);
        }

        public void CheckAndDownload(MVersion version)
        {
            foreach (var checker in this.GameFileCheckers)
            {
                DownloadFile[]? files = checker.CheckFiles(MinecraftPath, version, pFileChanged);

                if (files == null || files.Length == 0)
                    continue;

                DownloadGameFiles(files).GetAwaiter().GetResult();
            }
        }

        public async Task CheckAndDownloadAsync(MVersion version)
        {
            foreach (var checker in this.GameFileCheckers)
            {
                DownloadFile[]? files = await checker.CheckFilesTaskAsync(MinecraftPath, version, pFileChanged)
                    .ConfigureAwait(false);

                if (files == null || files.Length == 0)
                    continue;

                await DownloadGameFiles(files).ConfigureAwait(false);
            }
        }

        // not stable
        public Process CreateProcess(string mcversion, string forgeversion, MLaunchOption option)
        {
            CheckAndDownload(GetVersion(mcversion));

            var versionName = CheckForge(mcversion, forgeversion, option.JavaPath);

            return CreateProcess(versionName, option);
        }
        
        public Process CreateProcess(string versionName, MLaunchOption option, bool checkAndDownload=true)
            => CreateProcess(GetVersion(versionName), option, checkAndDownload);

        [MethodTimer.Time]
        public Process CreateProcess(MVersion version, MLaunchOption option, bool checkAndDownload=true)
        {
            option.StartVersion = version;
            
            if (checkAndDownload)
                CheckAndDownload(option.StartVersion);
            
            return CreateProcess(option);
        }

        public async Task<Process> CreateProcessAsync(string versionName, MLaunchOption option, 
            bool checkAndDownload=true)
        {
            var version = await GetVersionAsync(versionName).ConfigureAwait(false);
            return await CreateProcessAsync(version, option, checkAndDownload).ConfigureAwait(false);
        }

        public async Task<Process> CreateProcessAsync(MVersion version, MLaunchOption option, 
            bool checkAndDownload=true)
        {
            option.StartVersion = version;
            
            if (checkAndDownload)
                await CheckAndDownloadAsync(option.StartVersion).ConfigureAwait(false);
            
            return await CreateProcessAsync(option).ConfigureAwait(false);
        }
        
        public Process CreateProcess(MLaunchOption option)
        {
            checkLaunchOption(option);
            var launch = new MLaunch(option);
            return launch.GetProcess();
        }
        
        public async Task<Process> CreateProcessAsync(MLaunchOption option)
        {
            checkLaunchOption(option);
            var launch = new MLaunch(option);
            return await Task.Run(launch.GetProcess).ConfigureAwait(false);
        }

        public Process Launch(string versionName, MLaunchOption option)
        {
            Process process = CreateProcess(versionName, option);
            process.Start();
            return process;
        }

        public async Task<Process> LaunchAsync(string versionName, MLaunchOption option)
        {
            Process process = await CreateProcessAsync(versionName, option)
                .ConfigureAwait(false);
            process.Start();
            return process;
        }

        private void checkLaunchOption(MLaunchOption option)
        {
            if (option.Path == null)
                option.Path = MinecraftPath;
            if (option.StartVersion != null)
            {
                if (!string.IsNullOrEmpty(option.JavaPath))
                    option.StartVersion.JavaBinaryPath = option.JavaPath;
                else if (!string.IsNullOrEmpty(option.JavaVersion))
                    option.StartVersion.JavaVersion = option.JavaVersion;
                else if (string.IsNullOrEmpty(option.StartVersion.JavaBinaryPath))
                    option.StartVersion.JavaBinaryPath = 
                        GetJavaPath(option.StartVersion) ?? GetDefaultJavaPath();
            }
        }

        public string? GetJavaPath(MVersion version)
        {
            var javaPathResolver = new MinecraftJavaPathResolver(MinecraftPath);

            var javaPath = "";
            if (!string.IsNullOrEmpty(version.JavaVersion))
                javaPath = javaPathResolver.GetJavaBinaryPath(version.JavaVersion, MRule.OSName);
            
            return javaPath;
        }

        public string? GetDefaultJavaPath()
            => GetDefaultJavaPath(new MinecraftJavaPathResolver(MinecraftPath));
        
        public string? GetDefaultJavaPath(MinecraftJavaPathResolver javaPathResolver)
        {
            var javaVersions = javaPathResolver.GetInstalledJavaVersions();
            string? javaPath = null;
            
            if (string.IsNullOrEmpty(javaPath) &&
                javaVersions.Contains(MinecraftJavaPathResolver.JreLegacyVersionName))
                javaPath = javaPathResolver.GetJavaBinaryPath(MinecraftJavaPathResolver.JreLegacyVersionName, MRule.OSName);
            
            if (string.IsNullOrEmpty(javaPath) &&
                javaVersions.Contains(MinecraftJavaPathResolver.CmlLegacyVersionName))
                javaPath = javaPathResolver.GetJavaBinaryPath(MinecraftJavaPathResolver.CmlLegacyVersionName, MRule.OSName);

            if (string.IsNullOrEmpty(javaPath) && 
                javaVersions.Length > 0)
                javaPath = javaPathResolver.GetJavaBinaryPath(javaVersions[0], MRule.OSName);

            return javaPath;
        }
    }
}
