using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.ProcessBuilder;

namespace CmlLibCoreSample;

public class QuickStart
{
    public async Task Basic()
    {
        var launcher = new MinecraftLauncher();
        var process = await launcher.InstallAndBuildProcessAsync("1.20.4", new MLaunchOption());
        process.Start();
    }

    public async Task Fancy()
    {
        var path = new MinecraftPath("./my_game_dir");
        var launcher = new MinecraftLauncher(path);
        launcher.FileProgressChanged += (sender, args) =>
        {
            Console.WriteLine($"Name: {args.Name}");
            Console.WriteLine($"Type: {args.EventType}");
            Console.WriteLine($"Total: {args.TotalTasks}");
            Console.WriteLine($"Progressed: {args.ProgressedTasks}");
        };
        launcher.ByteProgressChanged += (sender, args) =>
        {
            Console.WriteLine($"{args.ProgressedBytes} bytes / {args.TotalBytes} bytes");
        };

        await launcher.InstallAsync("1.20.4");
        var process = await launcher.BuildProcessAsync("1.20.4", new MLaunchOption
        {
            Session = MSession.CreateOfflineSession("Gamer123"),
            MaximumRamMb = 4096
        });
        process.Start();
    }

    public async Task GetAllVersions()
    {
        var launcher = new MinecraftLauncher();
        var versions = await launcher.GetAllVersionsAsync();
        foreach (var version in versions)
        {
            Console.WriteLine($"{version.Type} {version.Name}");
        }
    }
}