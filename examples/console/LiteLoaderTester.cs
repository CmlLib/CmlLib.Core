using CmlLib.Core;
using CmlLib.Core.ModLoaders.LiteLoader;

namespace CmlLibCoreSample;

public class LiteLoaderTester
{
    private readonly MinecraftLauncher _launcher;

    public LiteLoaderTester(MinecraftLauncher launcher)
    {
        _launcher = launcher;
    }

    public async Task Test()
    {
        var liteLoaderInstaller = new LiteLoaderInstaller(new HttpClient());
        var loaders = await liteLoaderInstaller.GetAllLiteLoaders();

        foreach (var loader in loaders)
        {
            Console.WriteLine(loader.Version);
            var baseVersion = await _launcher.GetVersionAsync(loader.BaseVersion!);
            await liteLoaderInstaller.Install(loader, baseVersion, _launcher.MinecraftPath);
        }
    }
}