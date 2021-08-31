using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Downloader;
using CmlLib.Core.Installer;
using CmlLib.Core.Installer.FabricMC;
using CmlLib.Core.Installer.LiteLoader;

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

            // install
            var fabric = fabricVersions.GetVersionMetadata(fabricVersionName);
            await fabric.SaveAsync(path);

            // update version list
            await launcher.GetAllVersionsAsync();

            var process = await launcher.CreateProcessAsync(fabricVersionName, new MLaunchOption());
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
            var liteLoaderVersionLoader = new LiteLoaderVersionLoader();
            var liteLoaderVersions = await liteLoaderVersionLoader.GetVersionMetadatasAsync();

            // print all LiteLoader versions
            foreach (var item in liteLoaderVersions)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine("Select LiteLoader version name (ex: LiteLoader1.12.2) : ");
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

            // install LiteLoader
            var liteLoader =
                (LiteLoaderVersionMetadata)liteLoaderVersions.GetVersionMetadata(selectLiteLoaderVersion);
            var startVersionName = liteLoader.Install(path, await versions.GetVersionAsync(selectGameVersion));

            // update version list
            await launcher.GetAllVersionsAsync();

            // start
            var process = await launcher.CreateProcessAsync(startVersionName, new MLaunchOption
            {
                Session = MSession.GetOfflineSession("liteloadertester"),
                MaximumRamMb = 2048
            });

            process.Start();
        }
        
        // only vanilla
        public async Task TestLiteLoaderVanilla()
        {
            var path = new MinecraftPath();
            var launcher = new CMLauncher(path);
            launcher.FileChanged += Downloader_ChangeFile;
            launcher.ProgressChanged += Downloader_ChangeProgress;

            var versions = await launcher.GetAllVersionsAsync();

            // initialize LiteLoader installer
            var liteLoaderVersionLoader = new LiteLoaderVersionLoader();
            var liteLoaderVersions = await liteLoaderVersionLoader.GetVersionMetadatasAsync();

            // print all LiteLoader versions
            foreach (var item in liteLoaderVersions)
            {
                var liteLoaderVersion = item as LiteLoaderVersionMetadata;
                if (liteLoaderVersion == null)
                    continue;
                
                Console.WriteLine(item.Name);

                var vanillaVersionName = liteLoaderVersion.VanillaVersionName;
                Console.WriteLine(vanillaVersionName);
                var vanillaVersion = await versions.GetVersionAsync(vanillaVersionName);

                var liteLoaderVersionName = liteLoaderVersion.Install(path, vanillaVersion);
                versions = await launcher.GetAllVersionsAsync(); // update version lists

                var process = await launcher.CreateProcessAsync(liteLoaderVersionName, new MLaunchOption());
                process.Start();
                Console.ReadLine();
            }
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