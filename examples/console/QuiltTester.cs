using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.ModLoaders.FabricMC;
using CmlLib.Core.ModLoaders.QuiltMC;
using CmlLib.Core.ProcessBuilder;

namespace CmlLibCoreSample;

public class QuiltTester
{
    private readonly MinecraftPath _minecraftPath;

    public QuiltTester(MinecraftPath minecraftPath) => _minecraftPath = minecraftPath;

    public async Task Test()
    {
        var quiltInstaller = new QuiltInstaller(new HttpClient());
        var versions = await quiltInstaller.GetSupportedVersionNames();

        foreach (var version in versions) 
        {
            Console.WriteLine("Install " + version);
            await quiltInstaller.Install(version, _minecraftPath);
        }
    }

    public async Task LaunchLatest()
    {
        await Launch(versionSelector: versions => versions.First());
    }

    public async Task LaunchOldest()
    {
        await Launch(versionSelector: versions => versions.Last());
    }

    private async Task Launch(Func<IEnumerable<string>, string> versionSelector)
    {
        var launcher = new MinecraftLauncher(_minecraftPath);
        var quiltInstaller = new QuiltInstaller(new HttpClient());

        var versionTags = await quiltInstaller.GetSupportedVersionNames();
        var versionName = await quiltInstaller.Install(versionSelector(versionTags), _minecraftPath);

        var process = await launcher.InstallAndBuildProcessAsync(versionName, new MLaunchOption
        {
            Session = MSession.CreateOfflineSession("hello"),
            MaximumRamMb = 4096
        });

        process.Start();
    }
}