using CmlLib.Core;
using CmlLib.Core.Installers;
using CmlLib.Core.ProcessBuilder;

namespace CmlLibCoreSample;

public class LauncherTester
{
    static readonly string[] TargetVersions = new[]
        {
            "rd-132211",

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
            "1.20.4",
            "3D Shareware v1.34",

            "1.7.10-Forge10.13.4.1558-1.7.10",
            "1.7.10-Forge10.13.4.1614-1.7.10",
            "1.8.9-forge1.8.9-11.15.1.1722",
            "1.8.9-OptiFine_HD_U_M5",
            "1.12.2-forge1.12.2-14.23.4.2705",
            "1.12.2-forge1.12.2-14.23.5.2847",
            "1.12.2-forge-14.23.5.2854",
            "1.12.2-forge-14.23.5.2860",
            "1.12.2-OptiFine_HD_U_G5",
            "1.16.1-forge-32.0.47",
            "1.16.5-forge-36.2.0",
            "1.16.5-forge-36.2.34",
            "1.16.5-OptiFine_HD_U_G8",
            "1.17.1-forge-37.0.48",
            "1.17.1-forge-37.0.75",
            "1.17.1-OptiFine_HD_U_G9",
            "1.18.1-forge-39.0.5",
            "1.18.1-forge-39.1.2",
            "1.18.1-OptiFine_HD_U_H4",
            "1.18.2-forge-40.0.41",
            "1.19.4-forge-45.1.0",
            "1.20.1-forge-47.1.0",
            "1.20.2-forge-48.0.40",
            "1.20.4-forge-49.0.31",

            "fabric-loader-0.13.3-1.18.2",
            "fabric-loader-0.15.7-1.14",
            "fabric-loader-0.15.7-1.14.1",
            "fabric-loader-0.15.7-1.14.2",
            "fabric-loader-0.15.7-1.14.3",
            "fabric-loader-0.15.7-1.14.4",
            "fabric-loader-0.15.7-1.15",
            "fabric-loader-0.15.7-1.15.1",
            "fabric-loader-0.15.7-1.15.2",
            "fabric-loader-0.15.7-1.16",
            "fabric-loader-0.15.7-1.16.1",
            "fabric-loader-0.15.7-1.16.2",
            "fabric-loader-0.15.7-1.16.3",
            "fabric-loader-0.15.7-1.16.4",
            "fabric-loader-0.15.7-1.16.5",
            "fabric-loader-0.15.7-1.17",
            "fabric-loader-0.15.7-1.17.1",
            "fabric-loader-0.15.7-1.18",
            "fabric-loader-0.15.7-1.18.1",
            "fabric-loader-0.15.7-1.18.2",
            "fabric-loader-0.15.7-1.19",
            "fabric-loader-0.15.7-1.19.1",
            "fabric-loader-0.15.7-1.19.2",
            "fabric-loader-0.15.7-1.19.3",
            "fabric-loader-0.15.7-1.19.4",
            "fabric-loader-0.15.7-1.20",
            "fabric-loader-0.15.7-1.20.1",
            "fabric-loader-0.15.7-1.20.2",
            "fabric-loader-0.15.7-1.20.3",
            "fabric-loader-0.15.7-1.20.4",

            "neoforge-20.4.196",

            "1.9.4-LiteLoader1.9.4",
            "1.9-SNAPSHOT-LiteLoader1.9",
            "1.8.9-SNAPSHOT-LiteLoader1.8.9",
            "1.8-LiteLoader1.8",
            "1.7.2_05-LiteLoader1.7.2",
            "1.7.10_04-LiteLoader1.7.10",
            "1.6.4_01-LiteLoader1.6.4",
            "1.6.2_04-LiteLoader1.6.2",
            "1.5.2_01-LiteLoader1.5.2",
            "1.12.2-SNAPSHOT-LiteLoader1.12.2",
            "1.12.1-SNAPSHOT-LiteLoader1.12.1",
            "1.12-SNAPSHOT-LiteLoader1.12",
            "1.11.2-SNAPSHOT-LiteLoader1.11.2",
            "1.11-SNAPSHOT-LiteLoader1.11",
            "1.10.2-LiteLoader1.10.2",
            "1.10-SNAPSHOT-LiteLoader1.10"
        };

    private readonly MinecraftLauncher _launcher;

    public LauncherTester(string id, MinecraftLauncher launcher)
    {
        Id = id;
        _launcher = launcher;
    }

    public string Id { get; }
    public int AliveTimeSec = 30;
    public string OutputDirectory = "./outputs";

    private StreamWriter? currentOutputStream;
    private string? currentOutputPath;

    public async Task Start()
    {
        prepareOutputs();
        foreach (var version in TargetVersions)
        {
            if (checkTested(version))
            {
                Console.WriteLine($"Skip {version}, already tested");
            }
            else
            {
                Console.WriteLine($"Start {version}");
                await startTest(version);
            }
        }
    }

    private void prepareOutputs()
    {
        var output = Path.Combine(OutputDirectory, Id);
        Console.WriteLine($"Output: {output}");
        Directory.CreateDirectory(output);
    }

    private bool checkTested(string targetVersion)
    {
        return File.Exists(getOutputPath(targetVersion));
    }

    private string getOutputPath(string targetVersion)
    {
        return Path.Combine(OutputDirectory, Id, targetVersion + ".log");
    }

    private string getCrashedOutputPath(string targetVersion)
    {
        return Path.Combine(OutputDirectory, Id, targetVersion + ".crash.log");
    }

    private async Task startTest(string version)
    {
        prepareOutput(version);

        InstallerProgressChangedEventArgs? lastFileProgress = null;
        ByteProgress lastByteProgress = new();

        var launcherTask = _launcher.InstallAndBuildProcessAsync(
            version, 
            new MLaunchOption
            {
                //JavaPath = "java"
            }, 
            new SyncProgress<InstallerProgressChangedEventArgs>(f => lastFileProgress = f),
            new SyncProgress<ByteProgress>(f => lastByteProgress = f));

        Console.WriteLine();
        while (!launcherTask.IsCompleted)
        {
            await Task.Delay(3000);
            var percent = lastByteProgress.ProgressedBytes / (double)lastByteProgress.TotalBytes * 100;
            //Console.SetCursorPosition(0, Math.Max(0, Console.CursorTop - 1));
            Console.WriteLine($"Progress: {version} {lastFileProgress?.ProgressedTasks} / {lastFileProgress?.TotalTasks}, {percent}%");
        }

        var process = await launcherTask;
        Console.WriteLine(process.StartInfo.FileName);
        Console.WriteLine(process.StartInfo.Arguments);
        writeOutput(process.StartInfo.FileName);
        writeOutput(process.StartInfo.Arguments);

        Console.WriteLine($"Launch: {version}");
        var processWrapper = new ProcessWrapper(process);
        processWrapper.OutputReceived += (sender, msg) =>
        {
            Console.WriteLine(msg);
            writeOutput(msg);
        };
        processWrapper.StartWithEvents();
        var processTask = processWrapper.WaitForExitTaskAsync();
        await Task.WhenAny(processTask, Task.Delay(AliveTimeSec * 1000));

        if (process.HasExited)
        {
            Console.WriteLine($"Crashed: {version}");
            completeCrashedOutput(version);
        }
        else
        {
            process.Kill();
            writeOutput($"!!! Success: has been alive for {AliveTimeSec}, kill it");
            Console.WriteLine($"Success: {version}, has been alive for {AliveTimeSec} seconds");
            completeSuccessfulOutput(version);
        }
    }

    private void prepareOutput(string version)
    {
        currentOutputPath = Path.GetTempFileName();
        Directory.CreateDirectory(Path.GetDirectoryName(currentOutputPath)!);
        currentOutputStream = new StreamWriter(File.Create(currentOutputPath)); 

        writeOutput($"!!! Test: {Id}/{version}, {DateTime.Now}");
    }

    private void writeOutput(string msg)
    {
        if (currentOutputStream == null)
            return;
        currentOutputStream.WriteLine(msg);
    }

    private void completeSuccessfulOutput(string version)
    {
        if (string.IsNullOrEmpty(currentOutputPath))
            return;
        if (currentOutputStream == null)
            return;

        currentOutputStream.Flush();
        currentOutputStream.Dispose();
        Directory.CreateDirectory(Path.GetDirectoryName(getOutputPath(version))!);
        File.Move(currentOutputPath, getOutputPath(version));
    }

    private void completeCrashedOutput(string version)
    {
        if (string.IsNullOrEmpty(currentOutputPath))
            return;
        if (currentOutputStream == null)
            return;

        currentOutputStream.Flush();
        currentOutputStream.Dispose();
        Directory.CreateDirectory(Path.GetDirectoryName(getCrashedOutputPath(version))!);
        File.Move(currentOutputPath, getCrashedOutputPath(version), true);
    }
}