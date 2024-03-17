using System.Diagnostics;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Installers;
using CmlLib.Core.ProcessBuilder;

namespace CmlLibCoreSample;

class Program
{
    public static async Task Main()
    {
        var p = new Program();
        await p.Start();
        return;

        var tester = new LauncherTester("a");
        await tester.Start(new [] 
        {
            "1.0",
            "1.2.5",
            "1.3.2",
            "1.4.7",
            "1.5.2",
            "1.6.4",
            "1.7.2",
            "1.7.4",
            "1.7.10",
            "1.8.9",
            "1.9.4",
            "1.10.2",
            "1.11.2",
            "1.12.2",
            "1.13.2",
            "1.14.4",
            "1.15.2",
            "1.16.5",
            "1.17.1",
            "1.18.2",
            "1.19.4",
            "1.20.4"
        });
        return;
    }

    private async Task Start()
    {
        var sw = new Stopwatch();

        // initialize launcher
        var parameters = MinecraftLauncherParameters.CreateDefault();
        var launcher = new MinecraftLauncher(parameters);
        
        // add event handler
        //launcher.FileProgressChanged += Launcher_FileProgressChanged;
        //launcher.ByteProgressChanged += Launcher_ByteProgressChanged;

        // list versions
        var versions = await launcher.GetAllVersionsAsync();
        foreach (var v in versions)
        {
            //if (v.GetVersionType() == MVersionType.Release)
            {
                Console.WriteLine($"{v.Name}, {v.Type}, {v.ReleaseTime}");
                //await v.SaveVersionAsync(launcher.MinecraftPath);
            }
        }

        // select version
        Console.WriteLine("Select the version to launch: ");
        Console.Write("> ");
        var startVersion = Console.ReadLine();
        //var startVersion = "1.20.1";
        if (string.IsNullOrEmpty(startVersion))
            return;

        // install
        sw.Start();
        await launcher.InstallAsync(
            startVersion, 
            new SyncProgress<InstallerProgressChangedEventArgs>(e => Launcher_FileProgressChanged(null, e)), 
            new SyncProgress<ByteProgress>(e => Launcher_ByteProgressChanged(null, e)));
        sw.Stop();

        // build process
        var process = await launcher.BuildProcessAsync(startVersion, new MLaunchOption
        {
            Session = MSession.CreateOfflineSession("username"),
            JavaPath = "java"
        });

        // print debug information and start process
        Console.WriteLine();
        Console.WriteLine("Elapsed time to install: " + sw.Elapsed);
        Console.WriteLine("Java:");
        Console.WriteLine(process.StartInfo.FileName);
        Console.WriteLine("Arguments:");
        Console.WriteLine(process.StartInfo.Arguments);
        return;
        var processWrapper = new ProcessWrapper(process);
        processWrapper.OutputReceived += (s, e) => Console.WriteLine(e);
        processWrapper.StartWithEvents();
        var exitCode = await processWrapper.WaitForExitTaskAsync();
        Console.WriteLine($"Exited with code {exitCode}");
        Console.ReadLine();
    }

    private readonly object consoleLock = new object();
    private string bottomText = "...";
    private int previousProceed;
    private int lastCursorLeft = 0;
    private int lastUpdate = 0;
    private long lastProgressed = 0;

    // print installation progress to console
    private void Launcher_FileProgressChanged(object? sender, InstallerProgressChangedEventArgs e)
    {
        lock (consoleLock)
        {
            if (previousProceed > e.ProgressedTasks)
                return;

            var msg = $"[{e.ProgressedTasks} / {e.TotalTasks}][{e.EventType}] {e.Name}";
            Console.WriteLine(msg.PadRight(lastCursorLeft));
            printBottomProgress();

            previousProceed = e.ProgressedTasks;
        }
    }

    private void Launcher_ByteProgressChanged(object? sender, ByteProgress e)
    {
        lock (consoleLock)
        {
            var now = Environment.TickCount;
            if (Math.Abs(now - lastUpdate) >= 1000)
            {
                var percent = (e.ProgressedBytes / (double)e.TotalBytes) * 100;
                var total = e.TotalBytes.ToString().PadRight(12);
                var progressed = e.ProgressedBytes.ToString().PadLeft(12);

                var speed = (e.ProgressedBytes - lastProgressed) / (double)(now - lastUpdate);
                bottomText = $"==> {percent:F2}%, ({progressed} / {total}) bytes, {speed:F2} KB/s";
                lastProgressed = e.ProgressedBytes;
                lastUpdate = now;
            }
            printBottomProgress();
        }
    }

    private void printBottomProgress()
    {
        bottomText.PadRight(lastCursorLeft);
        Console.Write(bottomText);
        lastCursorLeft = Console.CursorLeft;
        Console.CursorLeft = 0;
    }
}

