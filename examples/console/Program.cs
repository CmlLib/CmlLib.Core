using System.Diagnostics;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.FileExtractors;
using CmlLib.Core.Installers;
using CmlLib.Core.ProcessBuilder;
using CmlLib.Core.VersionLoader;

namespace CmlLibCoreSample;

class Program
{
    public static async Task Main()
    {
        var p = new Program();
        await p.Start();
    }

    private async Task Start()
    {
        var sw = new Stopwatch();

        // initialize launcher
        var parameters = LauncherParameters.CreateDefault();
        //parameters.GameInstaller = new TPLGameInstaller(1);
        //parameters.VersionLoader = new VersionLoaderCollection
        //{
        //    new LocalVersionLoader(parameters.MinecraftPath!)
        //};
        var launcher = new MinecraftLauncher(parameters);
        
        // add event handler
        launcher.FileProgressChanged += Launcher_FileProgressChanged;
        launcher.ByteProgressChanged += Launcher_ByteProgressChanged;

        // list versions
        var versions = await launcher.GetAllVersionsAsync();
        foreach (var v in versions)
        {
            Console.WriteLine($"{v.Name}, {v.Type}, {v.ReleaseTime}");
        }

        // select version
        Console.WriteLine("Select the version to launch: ");
        Console.Write("> ");
        //var startVersion = Console.ReadLine();
        var startVersion = "1.20.1";
        if (string.IsNullOrEmpty(startVersion))
            return;

        // install
        sw.Start();
        await launcher.InstallAsync(startVersion);
        sw.Stop();

        // build process
        var process = await launcher.BuildProcessAsync(startVersion, new MLaunchOption
        {
            Session = MSession.GetOfflineSession("username"),
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
            if (previousProceed > e.ProceedTasks)
                return;

            var msg = $"[{e.ProceedTasks} / {e.TotalTasks}][{e.EventType}] {e.Name}";
            Console.WriteLine(msg.PadRight(lastCursorLeft));
            printBottomProgress();

            previousProceed = e.ProceedTasks;
        }
    }

    private void Launcher_ByteProgressChanged(object? sender, ByteProgress e)
    {
        lock (consoleLock)
        {
            var percent = (e.ProgressedBytes / (double)e.TotalBytes) * 100;
            var total = e.TotalBytes.ToString().PadRight(12);
            var progressed = e.ProgressedBytes.ToString().PadLeft(12);

            var now = Environment.TickCount;
            var speed = (e.ProgressedBytes - lastProgressed) / (double)(now - lastUpdate);
            bottomText = $"==> {percent:F2}%, ({progressed} / {total}) bytes, {speed:F2} KB/s";
            lastProgressed = e.ProgressedBytes;
            lastUpdate = now;
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

