using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Downloader;
using CmlLib.Core.Version;
using System;
using System.IO;

namespace CmlLibCoreSample
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine(CmlLib._Test.tstr);
            var p = new Program();

            // Login
            MSession session; // login session

            // There are two login methods, one is using mojang email and password, and the other is using only username
            // Choose one which you want.
            session = p.PremiumLogin(); // Login by mojang email and password
            //session = p.OfflineLogin(); // Login by username

            // log login session information
            Console.WriteLine("Success to login : {0} / {1} / {2}", session.Username, session.UUID, session.AccessToken);

            // Launch
            p.Start(session);
            //p.StartWithAdvancedOptions(session);
        }

        MSession PremiumLogin()
        {
            var login = new MLogin();

            // TryAutoLogin() reads the login cache file and check validation.
            // If the cached session is invalid, it refreshes the session automatically.
            // Refreshing the session doesn't always succeed, so you have to handle this.
            Console.WriteLine("Attempting to automatically log in.");
            var response = login.TryAutoLogin();

            if (!response.IsSuccess) // if cached session is invalid and failed to refresh token
            {
                Console.WriteLine("Auto login failed: {0}", response.Result.ToString());

                Console.WriteLine("Input your Mojang email: ");
                var email = Console.ReadLine();
                Console.WriteLine("Input your Mojang password: ");
                var pw = Console.ReadLine();

                response = login.Authenticate(email, pw);

                if (!response.IsSuccess)
                {
                    // session.Message contains a detailed error message. It can be null or an empty string.
                    Console.WriteLine("failed to login. {0} : {1}", response.Result.ToString(), response.ErrorMessage);
                    Console.ReadLine();
                    Environment.Exit(0);
                    return null;
                }
            }

            return response.Session;
        }

        MSession OfflineLogin()
        {
            // Create fake session by username
            return MSession.GetOfflineSession("tester123");
        }

        void Start(MSession session)
        {
            // Initializing Launcher

            // Set minecraft home directory
            // MinecraftPath.GetOSDefaultPath() return default minecraft BasePath of current OS.
            // https://github.com/AlphaBs/CmlLib.Core/blob/master/CmlLib/Core/MinecraftPath.cs

            // You can set this path to what you want like this :
            // var path = Environment.GetEnvironmentVariable("APPDATA") + "\\.mylauncher";
            var path = MinecraftPath.GetOSDefaultPath();
            var game = new MinecraftPath(path);

            // Create CMLauncher instance
            var launcher = new CMLauncher(game);
            launcher.ProgressChanged += Downloader_ChangeProgress;
            launcher.FileChanged += Downloader_ChangeFile;
            launcher.LogOutput += (s, e) => Console.WriteLine(e); // forge installer log

            Console.WriteLine($"Initialized in {launcher.MinecraftPath.BasePath}");

            var versions = launcher.GetAllVersions(); // Get all installed profiles and load all profiles from mojang server
            foreach (var item in versions) // Display all profiles 
            {
                // You can filter snapshots and old versions to add if statement : 
                // if (item.MType == MProfileType.Custom || item.MType == MProfileType.Release)
                Console.WriteLine(item.Type + " " + item.Name);
            }

            var launchOption = new MLaunchOption
            {
                MaximumRamMb = 1024,
                Session = session,

                // More options:
                // https://github.com/AlphaBs/CmlLib.Core/wiki/MLaunchOption
            };

            // (A) checks forge installation and install forge if it was not installed.
            // (B) just launch any versions without installing forge, but it can still launch forge already installed.
            // Both methods automatically download essential files (ex: vanilla libraries) and create game process.

            // (A) download forge and launch
            // var process = launcher.CreateProcess("1.12.2", "14.23.5.2768", launchOption);

            // (B) launch vanilla version
            // var process = launcher.CreateProcess("1.15.2", launchOption);

            // If you have already installed forge, you can launch it directly like this.
            // var process = launcher.CreateProcess("1.12.2-forge1.12.2-14.23.5.2838", launchOption);

            // launch by user input
            Console.WriteLine("input version (example: 1.12.2) : ");
            var process = launcher.CreateProcess(Console.ReadLine(), launchOption);

            //var process = launcher.CreateProcess("1.16.2", "33.0.5", launchOption);
            Console.WriteLine(process.StartInfo.Arguments);

            // Below codes are print game logs in Console.
            var processUtil = new CmlLib.Utils.ProcessUtil(process);
            processUtil.OutputReceived += (s, e) => Console.WriteLine(e);
            processUtil.StartWithEvents();
            process.WaitForExit();

            // or just start it without print logs
            // process.Start();

            Console.ReadLine();

            return;
        }

        #region Advance Launch

        void StartWithAdvancedOptions(MSession session)
        {
            // game directory
            var defaultPath = MinecraftPath.GetOSDefaultPath();
            var path = Path.Combine(Environment.CurrentDirectory, "game dir");

            // create minecraft path instance
            var minecraft = new MinecraftPath(path)
            {
                Assets = Path.Combine(defaultPath, "assets") // this speed up asset downloads
            };

            // get all version metadatas
            // you can also use MVersionLoader.GetVersionMetadatasFromLocal and GetVersionMetadatasFromWeb
            var versionMetadatas = new MVersionLoader().GetVersionMetadatas(minecraft);
            foreach (var item in versionMetadatas)
            {
                Console.WriteLine("Name : {0}", item.Name);
                Console.WriteLine("Type : {0}", item.Type);
                Console.WriteLine("Path : {0}", item.Path);
                Console.WriteLine("IsLocalVersion : {0}", item.IsLocalVersion);
                Console.WriteLine("============================================");
            }
            Console.WriteLine("");
            Console.WriteLine("LatestRelease : {0}", versionMetadatas.LatestReleaseVersion?.Name);
            Console.WriteLine("LatestSnapshot : {0}", versionMetadatas.LatestSnapshotVersion?.Name);

            Console.WriteLine("Input Version Name (ex: 1.15.2) : ");
            var versionName = Console.ReadLine();

            // get MVersion from MVersionMetadata
            var version = versionMetadatas.GetVersion(versionName);
            if (version == null)
            {
                Console.WriteLine("{0} is not exist", versionName);
                return;
            }

            Console.WriteLine("\n\nVersion Information : ");
            Console.WriteLine("Id : {0}", version.Id);
            Console.WriteLine("Type : {0}", version.TypeStr);
            Console.WriteLine("ReleaseTime : {0}", version.ReleaseTime);
            Console.WriteLine("AssetId : {0}", version.AssetId);
            Console.WriteLine("JAR : {0}", version.Jar);
            Console.WriteLine("Libraries : {0}", version.Libraries.Length);

            if (version.IsInherited)
                Console.WriteLine("Inherited Profile from {0}", version.ParentVersionId);

            // Download mode
            Console.WriteLine("\nSelect download mode : ");
            Console.WriteLine("(1) Sequence Download");
            Console.WriteLine("(2) Parallel Download");
            var downloadModeInput = Console.ReadLine();

            MDownloader downloader;
            if (downloadModeInput == "1")
                downloader = new MDownloader(minecraft, version); // Sequence Download
            else if (downloadModeInput == "2")
                downloader = new MAsyncDownloader(minecraft, version); // Parallel Download (note: Parallel Download is not stable yet)
            else
            {
                Console.WriteLine("Input 1 or 2");
                Console.ReadLine();
                return;
            }

            downloader.ChangeFile += Downloader_ChangeFile;
            downloader.ChangeProgress += Downloader_ChangeProgress;

            // Start download
            downloader.DownloadAll();

            Console.WriteLine("Download Completed.\n");

            // Set java
            Console.WriteLine("Input java path (empty input will download java) : ");
            var javaInput = Console.ReadLine();

            if (javaInput == "")
            {
                var java = new MJava();
                java.ProgressChanged += Downloader_ChangeProgress;
                javaInput = java.CheckJava();
            }

            // LaunchOption
            var option = new MLaunchOption()
            {
                JavaPath = javaInput,
                Session = session,
                StartVersion = version,
                Path = minecraft,

                MaximumRamMb = 4096,
                ScreenWidth = 1600,
                ScreenHeight = 900,
            };

            // Launch
            var launch = new MLaunch(option);
            var process = launch.GetProcess();

            Console.WriteLine(process.StartInfo.Arguments);
            process.Start();
            Console.WriteLine("Started");
            Console.ReadLine();
        }

        #endregion

        #region QuickStart

        // this code is from README.md

        void QuickStart()
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

            foreach (var item in launcher.GetAllVersions())
            {
                Console.WriteLine(item.Name);
            }

            var launchOption = new MLaunchOption
            {
                MaximumRamMb = 1024,
                Session = MSession.GetOfflineSession("hello"), // Login Session. ex) Session = MSession.GetOfflineSession("hello")

                //ScreenWidth = 1600,
                //ScreenHeigth = 900,
                //ServerIp = "mc.hypixel.net"
            };

            // launch vanila
            var process = launcher.CreateProcess("1.15.2", launchOption);

            process.Start();
        }

        #endregion

        // Event Handling

        // The code below has some tricks to display logs prettier.
        // You can use a simpler event handler

        #region Pretty event handler

        int nextline = -1;

        private void Downloader_ChangeProgress(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (nextline < 0)
                return;

            Console.SetCursorPosition(0, nextline);

            // e.ProgressPercentage: 0~100
            Console.WriteLine("{0}%", e.ProgressPercentage);
        }

        private void Downloader_ChangeFile(DownloadFileChangedEventArgs e)
        {
            // More information about DownloadFileChangedEventArgs
            // https://github.com/AlphaBs/CmlLib.Core/wiki/Handling-Events#downloadfilechangedeventargs

            Console.WriteLine("[{0}] {1} - {2}/{3}           ", e.FileKind.ToString(), e.FileName, e.ProgressedFileCount, e.TotalFileCount);
            if (e.FileKind == MFile.Resource && string.IsNullOrEmpty(e.FileName))
                Console.SetCursorPosition(0, Console.CursorTop - 1);
            nextline = Console.CursorTop;
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
