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

    public async Task GetAllVersions()
    {
        var liteLoaderInstaller = new LiteLoaderInstaller(new HttpClient());
        var loaders = await liteLoaderInstaller.GetAllLiteLoaders();

        foreach (var loader in loaders)
        {
            Console.WriteLine($"GameVersion: {loader.BaseVersion}, LoaderVersion: {loader.Version}");
        }
    }

    public async Task Install()
    {
        var path = new MinecraftPath();
        var launcher = new MinecraftLauncher(path);
        var version = "1.7.10";

        var liteLoaderInstaller = new LiteLoaderInstaller(new HttpClient());
        var loaders = await liteLoaderInstaller.GetAllLiteLoaders();
        var loaderToInstall = loaders.First(loader => loader.BaseVersion == version);

        var installedVersion = await liteLoaderInstaller.Install(
            loaderToInstall, 
            await launcher.GetVersionAsync(version), 
            path);
    }
}