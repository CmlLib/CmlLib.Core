using System;
using System.Threading;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Downloader;

namespace CmlLibCoreSample
{
    public class Test
    {
        public async Task TestDownloadFile()
        {
            // Console.WriteLine("TestDownloadFile");
            // var wd = new CmlLib.Utils.WebDownload();
            // wd.FileDownloadProgressChanged += (sender, args) => Console.WriteLine(args.ProgressPercentage); 
            // await wd.DownloadFileAsync(new DownloadFile()
            // {
            //     Name = "test.bin",
            //     Size = 0,
            //     Url =  "https://github.com/AlphaBs/YoutubeMusicPlayer/releases/download/v2.0-b2/YMP_b2.zip",
            //     Path = "test.bin"
            // });
            // Console.WriteLine("done");
        }
        
        async Task TestAll(MSession session)
        {
            var path = MinecraftPath.GetOSDefaultPath();
            var game = new MinecraftPath(path);

            var launcher = new CMLauncher(game);

            System.Net.ServicePointManager.DefaultConnectionLimit = 256;
            launcher.FileDownloader = new AsyncParallelDownloader();

            launcher.ProgressChanged += Downloader_ChangeProgress;
            launcher.FileChanged += Downloader_ChangeFile;

            Console.WriteLine($"Initialized in {launcher.MinecraftPath.BasePath}");

            var launchOption = new MLaunchOption
            {
                MaximumRamMb = 1024,
                Session = session,
            };

            var versions = await launcher.GetAllVersionsAsync();
            foreach (var item in versions)
            {
                Console.WriteLine(item.Type + " " + item.Name);

                if (!item.IsLocalVersion)
                    continue;

                var process = launcher.CreateProcess(item.Name, launchOption);

                //var process = launcher.CreateProcess("1.16.2", "33.0.5", launchOption);
                Console.WriteLine(process.StartInfo.Arguments);

                // Below codes are print game logs in Console.
                var processUtil = new CmlLib.Utils.ProcessUtil(process);
                processUtil.OutputReceived += (s, e) => Console.WriteLine(e);
                processUtil.StartWithEvents();

                Thread.Sleep(1000 * 15);

                if (process.HasExited)
                {
                    Console.WriteLine("FAILED!!!!!!!!!");
                    Console.ReadLine();
                }

                process.Kill();
                process.WaitForExit();
            }

            return;
        }

        void TestStartSync(MSession session)
        {
            var path = new MinecraftPath();
            var launcher = new CMLauncher(path);

            launcher.FileChanged += Downloader_ChangeFile;
            launcher.ProgressChanged += Downloader_ChangeProgress;

            var versions = launcher.GetAllVersions();
            foreach (var item in versions)
            {
                Console.WriteLine(item.Name);
            }

            var process = launcher.CreateProcess("1.5.2", new MLaunchOption
            {
                Session = session
            });

            var processUtil = new CmlLib.Utils.ProcessUtil(process);
            processUtil.OutputReceived += (s, e) => Console.WriteLine(e);
            processUtil.StartWithEvents();
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