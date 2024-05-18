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
}