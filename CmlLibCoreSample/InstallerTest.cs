using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Downloader;
using CmlLib.Core.Installer;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader.FabricMC;

namespace CmlLibCoreSample
{
    public class InstallerTest
    {
        // FabricLoader
        public async Task TestFabric()
        {
            var path = new MinecraftPath();
            var launcher = new CMLauncher(path);
            launcher.FileChanged += Downloader_ChangeFile;
            launcher.ProgressChanged += Downloader_ChangeProgress;

            // initialize fabric version loader
            var fabricVersionLoader = new FabricVersionLoader();
            var fabricVersions = await fabricVersionLoader.GetVersionMetadatasAsync();

            // print fabric versions
            foreach (var v in fabricVersions)
            {
                Console.WriteLine(v.Name);
            }
            Console.WriteLine("select version: ");
            var fabricVersionName = Console.ReadLine();
            
            if (string.IsNullOrEmpty(fabricVersionName))
                return;

            var versions = await launcher.GetAllVersionsAsync();
            if (!versions.Contains(fabricVersionName)) // check version existence
            {
                var fabric = fabricVersions.GetVersionMetadata(fabricVersionName);
                fabric.Save(path);
                
                // update version list
                await launcher.GetAllVersionsAsync();
            }

            var process = await launcher.CreateProcessAsync(fabricVersionName, new MLaunchOption
            {
                Session = MSession.GetOfflineSession("hello"),
                MaximumRamMb = 2048
            });
            process.Start();
        }
        
        // LiteLoader
        public async Task TestLiteLoader()
        {
            var path = new MinecraftPath();
            var launcher = new CMLauncher(path);
            launcher.FileChanged += Downloader_ChangeFile;
            launcher.ProgressChanged += Downloader_ChangeProgress;

            // initialize LiteLoader installer
            var liteLoaderInstaller = new LiteLoaderInstaller(path);
            var liteLoaderVersions = await liteLoaderInstaller.GetAllLiteLoaderVersions();

            // print all LiteLoader versions
            foreach (var item in liteLoaderVersions)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("Select LiteLoader version name : ");
            var selectLiteLoaderVersion = Console.ReadLine();

            // print all game versions
            var versions = await launcher.GetAllVersionsAsync();
            foreach (var item in versions)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("Select minecraft version where to install : ");
            var selectGameVersion = Console.ReadLine();
            
            if (string.IsNullOrEmpty(selectLiteLoaderVersion) || string.IsNullOrEmpty(selectGameVersion))
                return;

            var startVersionName = LiteLoaderInstaller.GetVersionName(
                selectLiteLoaderVersion, selectGameVersion);
            
            if (!versions.Contains(startVersionName))
            {
                // install LiteLoader
                await liteLoaderInstaller.Install(
                    selectLiteLoaderVersion, versions.GetVersionMetadata(selectGameVersion));

                // update version list
                await launcher.GetAllVersionsAsync();
            }

            // start
            var process = await launcher.CreateProcessAsync(startVersionName, new MLaunchOption
            {
                Session = MSession.GetOfflineSession("liteloadertester"),
                MaximumRamMb = 2048
            });

            process.Start();
        }
        
        int endTop = -1;

        private void Downloader_ChangeProgress(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            Console.SetCursorPosition(0, endTop);

            // e.ProgressPercentage: 0~100
            Console.Write("{0}%       ", e.ProgressPercentage);

            Console.SetCursorPosition(0, endTop);
        }

        private void Downloader_ChangeFile(DownloadFileChangedEventArgs e)
        {
            // More information about DownloadFileChangedEventArgs
            // https://github.com/AlphaBs/CmlLib.Core/wiki/Handling-Events#downloadfilechangedeventargs

            Console.WriteLine("[{0}] ({2}/{3}) {1}   ", e.FileKind.ToString(), e.FileName, e.ProgressedFileCount,
                e.TotalFileCount);

            endTop = Console.CursorTop;
        }
    }
}