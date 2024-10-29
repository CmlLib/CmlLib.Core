using CmlLib.Core;
using CmlLib.Core.ModLoaders.FabricMC;

namespace CmlLibCoreSample;

public class FabricTester
{
    private readonly MinecraftPath _minecraftPath;

    public FabricTester(MinecraftPath minecraftPath) =>
        _minecraftPath = minecraftPath;

    public async Task Test()
    {
        var fabricInstaller = new FabricInstaller(new HttpClient());
        var versions = await fabricInstaller.GetSupportedVersionNames();

        foreach (var version in versions) 
        {
            Console.WriteLine("Install " + version);
            await fabricInstaller.Install(version, _minecraftPath);
        }
    }

    public async Task GetMinecraftVersions()
    {
        var fabricInstaller = new FabricInstaller(new HttpClient());
        var versions = await fabricInstaller.GetSupportedVersionNames();

        foreach (var version in versions)
        {
            Console.WriteLine(version);
        }
    }

    public async Task GetFabricVersions()
    {
        var fabricInstaller = new FabricInstaller(new HttpClient());
        var versions = await fabricInstaller.GetLoaders();

        foreach (var version in versions)
        {
            Console.WriteLine(version.Version);
        }
    }

    public async Task Install()
    {
        var path = new MinecraftPath();
        var launcher = new MinecraftLauncher(path);

        var fabricInstaller = new FabricInstaller(new HttpClient());
        var versionName = await fabricInstaller.Install("1.20.4", "0.16.0", path);
    }
}