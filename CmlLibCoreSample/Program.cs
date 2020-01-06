using System;
using System.IO;
using CmlLib.Launcher;
using CmlLib.Utils;

namespace CmlLibCoreSample
{
    class Program
    {
        static void Main()
        {
            var p = new Program();
            p.Start();
        }

        private static void Z_ProgressEvent(object sender, int e)
        {
            Console.WriteLine(e);
        }

        void Start()
        {
            // initialize

            var minecraftFolder = Minecraft.GetOSDefaultPath();
            Console.WriteLine("Initialize Minecraft in {0}", minecraftFolder);

            Minecraft.Initialize(minecraftFolder);

            // check java

            var j = new MJava();
            var java = Path.Combine(j.RuntimeDirectory, "bin", "java.exe");

            if (!j.CheckJavaw())
            {
                Console.WriteLine("Start downloading java");
                j.DownloadProgressChanged += J_DownloadProgressChanged;
                j.DownloadCompleted += J_DownloadCompleted;
                j.UnzipCompleted += J_UnzipCompleted;
                j.DownloadJava();
                Console.WriteLine();
            }

            Console.WriteLine("Set java path : {0}", java);

            // login

            bool premiumMode = false;
            MSession session;

            if (premiumMode)
            {
                Console.WriteLine("Try Auto login");

                var login = new MLogin();

                session = login.TryAutoLogin();
                if (session.Result != MLoginResult.Success)
                {
                    Console.WriteLine("Auto login failed : {0}", session.Result.ToString());

                    Console.WriteLine("Input mojang email : ");
                    var email = Console.ReadLine();
                    Console.WriteLine("Input mojang password : ");
                    var pw = Console.ReadLine();

                    session = login.Authenticate(email, pw);

                    if (session.Result != MLoginResult.Success)
                    {
                        Console.WriteLine("failed to login. {0} : {1}", session.Result.ToString(), session.Message);
                        Console.ReadLine();
                        return;
                    }
                }
            }
            else
                session = MSession.GetOfflineSession("tester123");

            Console.WriteLine("Success to login : {0} / {1} / {2}", session.Username, session.UUID, session.AccessToken);

            // get profile info

            var infos = MProfileInfo.GetProfiles();
            foreach (var item in infos)
            {
                Console.WriteLine("[{0}] {1} ({2})", item.MType.ToString(), item.Name, item.IsWeb ? "web" : "local");
            }

            // get profile

            Console.WriteLine("Input version name : ");
            var version = Console.ReadLine();

            var profile = MProfile.FindProfile(infos, version);
            if (profile == null)
            {
                Console.WriteLine("Can't find profile");
                Console.ReadLine();
                return;
            }

            // download

            Console.WriteLine("Start downloading {0}", version);

            var downloader = new MDownloader(profile);
            downloader.ChangeFile += Downloader_ChangeFile;
            downloader.ChangeProgress += Downloader_ChangeProgress;
            downloader.DownloadAll();

            // launch

            Console.WriteLine("Launch Game");

            var javapath = java;
            var xmx = 1024;

            Console.WriteLine("Java Path : {0}", javapath);
            Console.WriteLine("Xmx : {0}", xmx);

            var option = new MLaunchOption()
            {
                // must require
                StartProfile = profile,
                JavaPath = javapath, //SET YOUR JAVA PATH (if you want autoset, goto wiki)
                MaximumRamMb = xmx, // MB
                Session = session,

                // not require
                ServerIp = "", // connect server directly
                LauncherName = "", // display launcher name at main window
                CustomJavaParameter = "" // set your own java args
            };

            var launch = new MLaunch(option);
            var process = launch.GetProcess();
            Console.WriteLine(process.StartInfo.Arguments);
            process.Start();

            Console.WriteLine("Started");
            Console.ReadLine();
            return;
        }

        private void J_UnzipCompleted(object sender, EventArgs e)
        {
            Console.WriteLine("Java Unzip Completed");
        }

        private void J_DownloadCompleted(object sender, EventArgs e)
        {
            Console.WriteLine("Unzip java");
        }

        private void J_DownloadProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            Console.WriteLine("Java : {0}%", e.ProgressPercentage);
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }

        int nextline = -1;

        private void Downloader_ChangeProgress(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (nextline < 0)
                return;

            Console.SetCursorPosition(0, nextline);
            Console.WriteLine("{0}%", e.ProgressPercentage);
        }

        private void Downloader_ChangeFile(DownloadFileChangedEventArgs e)
        {
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
    }
}
