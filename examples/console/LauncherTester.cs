using CmlLib.Core;
using CmlLib.Core.Installers;
using CmlLib.Core.ProcessBuilder;

namespace CmlLibCoreSample;

public class LauncherTester
{
    private readonly MinecraftLauncher _launcher;

    public LauncherTester(string id)
    {
        Id = id;
        var path = new MinecraftPath();
        _launcher = new MinecraftLauncher(path);
    }

    public string Id { get; }
    public int AliveTimeSec = 30;
    public string OutputDirectory = "./outputs";

    private StreamWriter? currentOutputStream;
    private string? currentOutputPath;

    public async Task Start(IEnumerable<string> targetVersions)
    {
        prepareOutputs();
        foreach (var version in targetVersions)
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
        return Path.Combine(OutputDirectory, Id, targetVersion);
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
                JavaPath = "java"
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
        Console.WriteLine(process.StartInfo.Arguments);
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
        }
        else
        {
            process.Kill();
            writeOutput($"!!! Success: has been alive for {AliveTimeSec}, kill it");
            Console.WriteLine($"Success: {version}, has been alive for {AliveTimeSec} seconds");
        }

        completeOutput(version);
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

    private void completeOutput(string version)
    {
        if (string.IsNullOrEmpty(currentOutputPath))
            return;
        if (currentOutputStream == null)
            return;

        currentOutputStream.Flush();
        currentOutputStream.Dispose();
        Directory.CreateDirectory(Path.GetDirectoryName(getOutputPath(version))!);
        File.Move(currentOutputPath, getOutputPath(version) + ".txt");
    }
}