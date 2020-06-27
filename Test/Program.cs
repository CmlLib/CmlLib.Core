using CmlLib.Core;
using System;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //var path = new Minecraft("your minecraft directory);
            var path = Minecraft.GetOSDefaultPath(); // mc directory

            var launcher = new CmlLib.CMLauncher(path);
            launcher.ProgressChanged += (s, e) =>
            {
                Console.WriteLine("{0}%", e.ProgressPercentage);
            };
            launcher.FileChanged += (e) =>
            {
                Console.WriteLine("[{0}] {1} - {2}/{3}", e.FileKind.ToString(), e.FileName, e.ProgressedFileCount, e.TotalFileCount);
            };

            launcher.UpdateProfiles();
            foreach (var item in launcher.Profiles)
            {
                Console.WriteLine(item.Name);
            }

            var launchOption = new MLaunchOption
            {
                MaximumRamMb = 1024,
                Session = MSession.GetOfflineSession("hello"), // Login Session. ex) Session = MSession.GetOfflineSession("hello")

                //LauncherName = "MyLauncher",
                //ScreenWidth = 1600,
                //ScreenHeigth = 900,
                //ServerIp = "mc.hypixel.net"
            };

            // launch vanila
            var process = launcher.CreateProcess("1.15.2", launchOption);

            process.Start();
        }
    }
}
