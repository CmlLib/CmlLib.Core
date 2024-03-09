using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Downloader;
using System;
using System.Threading.Tasks;
using CmlLib.Core.VersionLoader;

namespace CmlLibCoreSample
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine(CmlLib._Test.tstr);
            var p = new Program();

            // Login
            MSession session = MSession.CreateOfflineSession("cmllib_tester");

            // log login session information
            Console.WriteLine("Success to login : {0} / {1} / {2}", session.Username, session.UUID, session.AccessToken);

            // Launch
            p.Start(session);
            //p.StartAsync(session).GetAwaiter().GetResult();
        }

        void Start(MSession session)
        {
            // Initializing Launcher

            // Set minecraft home directory
            // MinecraftPath.GetOSDefaultPath() return default minecraft BasePath of current OS.
            // https://github.com/AlphaBs/CmlLib.Core/blob/master/CmlLib/Core/MinecraftPath.cs

            // You can set this path to what you want like this :
            //var path = "./testdir";
            var path = MinecraftPath.GetOSDefaultPath();
            var game = new MinecraftPath(path);

            // Create CMLauncher instance
            var launcher = new CMLauncher(game);

            // if you want to download with parallel downloader, add below code :
            System.Net.ServicePointManager.DefaultConnectionLimit = 256;

            // for offline mode
            //launcher.VersionLoader = new LocalVersionLoader(launcher.MinecraftPath);
            //launcher.FileDownloader = null;
            
            launcher.ProgressChanged += Downloader_ChangeProgress;
            launcher.FileChanged += Downloader_ChangeFile;

            Console.WriteLine($"Initialized in {launcher.MinecraftPath.BasePath}");

            // Get all installed profiles and load all profiles from mojang server
            var versions = launcher.GetAllVersions(); 

            foreach (var item in versions) // Display all profiles 
            {
                // You can filter snapshots and old versions to add if statement : 
                // if (item.MType == MProfileType.Custom || item.MType == MProfileType.Release)
                Console.WriteLine(item.Type + " " + item.Name);
            }

            var launchOption = new MLaunchOption
            {
                Session = session,
                
                //MaximumRamMb = 2048,
                //ScreenWidth = 1600,
                //ScreenHeight = 900,
                //ServerIp = "mc.hypixel.net",
                //MinimumRamMb = 102,
                //FullScreen = true,

                // More options:
                // https://github.com/AlphaBs/CmlLib.Core/wiki/MLaunchOption
            };

            // download essential files (ex: vanilla libraries) and create game process.

            // var process = await launcher.CreateProcessAsync("1.15.2", launchOption); // vanilla
            // var process = await launcher.CreateProcessAsync("1.12.2-forge1.12.2-14.23.5.2838", launchOption); // forge
            // var process = await launcher.CreateProcessAsync("1.12.2-LiteLoader1.12.2"); // liteloader
            // var process = await launcher.CreateProcessAsync("fabric-loader-0.11.3-1.16.5") // fabric-loader

            Console.WriteLine("input version (example: 1.12.2) : ");
            var process = launcher.CreateProcess(Console.ReadLine(), launchOption);

            //var process = launcher.CreateProcess("1.16.2", "33.0.5", launchOption);
            Console.WriteLine(process.StartInfo.FileName);
            Console.WriteLine(process.StartInfo.Arguments);

            // Below codes are print game logs in Console.
            var processUtil = new CmlLib.Utils.ProcessUtil(process);
            processUtil.OutputReceived += (s, e) => Console.WriteLine(e);
            processUtil.StartWithEvents();
            process.WaitForExit();

            // or just start it without print logs
            // process.Start();

            Console.ReadLine();
        }

        async Task StartAsync(MSession session) // async version
        {
            var path = new MinecraftPath();
            var launcher = new CMLauncher(path);

            System.Net.ServicePointManager.DefaultConnectionLimit = 256;

            var versions = await launcher.GetAllVersionsAsync();
            foreach (var item in versions)
            {
                Console.WriteLine(item.Type + " " + item.Name);
            }

            launcher.FileChanged += Downloader_ChangeFile;
            launcher.ProgressChanged += Downloader_ChangeProgress;
            
            Console.WriteLine("input version (example: 1.12.2) : ");
            var versionName = Console.ReadLine();
            var process = await launcher.CreateProcessAsync(versionName, new MLaunchOption
            {
                Session = session,
                MaximumRamMb = 1024
            });

            Console.WriteLine(process.StartInfo.Arguments);
            process.Start();
        }

        #region QuickStart

        // this code is from README.md

        async Task QuickStart()
        {
            //var path = new MinecraftPath("game_directory_path");
            var path = new MinecraftPath(); // use default directory

            var launcher = new CMLauncher(path);
            launcher.FileChanged += (e) =>
            {
                Console.WriteLine("[{0}] {1} - {2}/{3}", e.FileKind.ToString(), e.FileName, e.ProgressedFileCount, e.TotalFileCount);
            };
            launcher.ProgressChanged += (s, e) =>
            {
                Console.WriteLine("{0}%", e.ProgressPercentage);
            };

            var versions = await launcher.GetAllVersionsAsync();
            foreach (var item in versions)
            {
                Console.WriteLine(item.Name);
            }

            var launchOption = new MLaunchOption
            {
                MaximumRamMb = 1024,
                Session = MSession.CreateOfflineSession("hello"), // Login Session. ex) Session = MSession.GetOfflineSession("hello")

                //ScreenWidth = 1600,
                //ScreenHeigth = 900,
                //ServerIp = "mc.hypixel.net"
            };

            // launch vanila
            var process = await launcher.CreateProcessAsync("1.15.2", launchOption);

            process.Start();
        }

        #endregion

        // Event Handling

        // The code below has some tricks to display logs prettier.
        // You can also use a simpler event handler

        #region Pretty event handler

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

            Console.WriteLine("[{0}] ({2}/{3}) {1}   ", e.FileKind.ToString(), e.FileName, e.ProgressedFileCount, e.TotalFileCount);

            endTop = Console.CursorTop;
        }

        #endregion

        #region Simple event handler
        //private void Downloader_ChangeProgress(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        //{
        //    Console.WriteLine("{0}%", e.ProgressPercentage);
        //}

        //private void Downloader_ChangeFile(DownloadFileChangedEventArgs e)
        //{
        //    Console.WriteLine("[{0}] {1} - {2}/{3}", e.FileKind.ToString(), e.FileName, e.ProgressedFileCount, e.TotalFileCount);
        //}
        #endregion
    }
}
