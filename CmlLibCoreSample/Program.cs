using CmlLib.Core;
using System;

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
        }

        MSession PremiumLogin()
        {
            MSession session;
            var login = new MLogin();

            // TryAutoLogin() read login cache file and check validation.
            // if cached session is invalid, it refresh session automatically.
            // but refreshing session doesn't always succeed, so you have to handle this.
            Console.WriteLine("Try Auto login");
            session = login.TryAutoLogin();

            if (session.Result != MLoginResult.Success) // cached session is invalid and failed to refresh token
            {
                Console.WriteLine("Auto login failed : {0}", session.Result.ToString());

                Console.WriteLine("Input mojang email : ");
                var email = Console.ReadLine();
                Console.WriteLine("Input mojang password : ");
                var pw = Console.ReadLine();

                session = login.Authenticate(email, pw);

                if (session.Result != MLoginResult.Success)
                {
                    // session.Message contains detailed error message. it can be null or empty string.
                    Console.WriteLine("failed to login. {0} : {1}", session.Result.ToString(), session.Message);
                    Console.ReadLine();
                    Environment.Exit(0);
                    return null;
                }
            }

            return session;
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
            // Minecraft.GetOSDefaultPath() return default minecraft path of current OS.
            // https://github.com/AlphaBs/CmlLib.Core/blob/master/CmlLib/Core/Minecraft.cs

            // You can set this path to what you want like this :
            // var path = Environment.GetEnvironmentVariable("APPDATA") + "\\.mylauncher";
            var path = Minecraft.GetOSDefaultPath();

            // Create CMLauncher instance
            var launcher = new CmlLib.CMLauncher(path);
            launcher.ProgressChanged += Downloader_ChangeProgress;
            launcher.FileChanged += Downloader_ChangeFile;

            Console.WriteLine($"Initialized in {launcher.Minecraft.path}");

            launcher.UpdateProfiles(); // Get all installed profiles and load all profiles from mojang server
            foreach (var item in launcher.Profiles) // Display all profiles 
            {
                Console.WriteLine(item.Type + " " + item.Name);
            }

            var launchOption = new MLaunchOption
            {
                MaximumRamMb = 1024,
                Session = session,

                // More options:
                // https://github.com/AlphaBs/CmlLib.Core/wiki/MLaunchOption
            };

            // launcher.CreateProcess method downloads profile and create Process instance.

            // launch forge
            // var process = launcher.CreateProcess("1.12.2", "14.23.5.2768", launchOption);

            // launch vanila
            // var process = launcher.CreateProcess("1.15.2", launchOption);

            // launch by user input
            Console.WriteLine("input version : ");
            var process = launcher.CreateProcess(Console.ReadLine(), launchOption);

            Console.WriteLine(process.StartInfo.Arguments);
            process.Start();

            Console.WriteLine("Started");
            Console.ReadLine();

            return;
        }

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

            if (e.FileKind == MFile.Resource)
            {
                Console.WriteLine("[Resource] {0} - {1} / {2}", e.FileName, e.ProgressedFileCount, e.TotalFileCount);

                if (e.ProgressedFileCount < e.TotalFileCount)
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
            }
            else
            {
                Console.WriteLine("[{0}] {1} - {2}/{3}", e.FileKind.ToString(), e.FileName, e.ProgressedFileCount, e.TotalFileCount);
                nextline = Console.CursorTop;
            }
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
