using System.Diagnostics;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Installers;
using CmlLib.Core.ProcessBuilder;
using CmlLib.Core.Rules;
using ShellProgressBar;

namespace CmlLibCoreSample;

class Program
{
    public static async Task Main()
    {
        Console.WriteLine($"{LauncherOSRule.Current.Name}, {LauncherOSRule.Current.Arch}, {LauncherOSRule.Current.Version}");

        var p = new Program();
        await p.Start();
        return;

        // var path = new MinecraftPath("D:\\Games\\minecraft");
        // var launcher = new MinecraftLauncher(path);
        //
        // var quiltTester = new QuiltTester(path);
        // await quiltTester.LaunchOldest();
        // return;
        //
        // var llTester = new LiteLoaderTester(launcher);
        // await llTester.Test();
        // return;
        //
        // var tester = new LauncherTester("b", launcher);
        // await tester.Start();
    }

    private async Task Start()
    {
        var sw = new Stopwatch();

        // initialize launcher
        var parameters = MinecraftLauncherParameters.CreateDefault();
        var launcher = new MinecraftLauncher(parameters);

        // list versions
        var versions = await launcher.GetAllVersionsAsync();
        foreach (var v in versions)
        {
            Console.WriteLine($"{v.Name}, {v.Type}, {v.ReleaseTime}");
        }

        // select version
        Console.WriteLine("Select the version to launch: ");
        Console.Write("> ");
        var startVersion = Console.ReadLine();
        if (string.IsNullOrEmpty(startVersion))
            return;

        // install
        sw.Start();
        using var progressbar = new ProgressBar(10000, startVersion, new ProgressBarOptions
        {
            ProgressBarOnBottom = true,
            ProgressCharacter = '=',
        });
        await launcher.InstallAsync(
            startVersion,
            new SyncProgress<InstallerProgressChangedEventArgs>(e => 
                progressbar.WriteLine($"[{e.ProgressedTasks} / {e.TotalTasks}][{e.EventType}] {e.Name}")),
            new SyncProgress<ByteProgress>(e =>
            {
                var p = (int)(e.ToRatio() * 10000);
                progressbar.Tick(p);
            }));
        sw.Stop();

        // build process
        var process = await launcher.BuildProcessAsync(startVersion, new MLaunchOption
        {
            Session = MSession.CreateOfflineSession("username")
        });

        // print debug information and start process
        Console.WriteLine();
        Console.WriteLine("Elapsed time to install: " + sw.Elapsed);
        Console.WriteLine("Java:");
        Console.WriteLine(process.StartInfo.FileName);
        Console.WriteLine("Arguments:");
        Console.WriteLine(process.StartInfo.Arguments);

        var processWrapper = new ProcessWrapper(process);
        processWrapper.OutputReceived += (s, e) => Console.WriteLine(e);
        processWrapper.StartWithEvents();
        var exitCode = await processWrapper.WaitForExitTaskAsync();
        Console.WriteLine($"Exited with code {exitCode}");
        Console.ReadLine();
    }
}

