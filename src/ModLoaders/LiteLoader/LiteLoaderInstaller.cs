using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using CmlLib.Core.VersionMetadata;

namespace CmlLib.Core.ModLoaders.LiteLoader;

// 1.8.9 freezing
public class LiteLoaderInstaller
{
    private readonly HttpClient _httpClient;

    public LiteLoaderInstaller(MinecraftPath path, HttpClient httpClient)
    {
        _httpClient = httpClient;
        this.minecraftPath = path;
    }

    private readonly MinecraftPath minecraftPath;
    private VersionMetadataCollection? liteLoaderVersions;

    public static string GetVersionName(string loaderVersion, string baseVersion)
    {
        loaderVersion = loaderVersion.Replace("LiteLoader", "");
        return $"{loaderVersion}-LiteLoader{baseVersion}";
    }

    public async Task<LiteLoaderVersionMetadata[]> GetAllLiteLoaderVersions()
    {
        var llVersionLoader = new LiteLoaderVersionLoader(_httpClient);
        liteLoaderVersions = await llVersionLoader.GetVersionMetadatasAsync()
            .ConfigureAwait(false);

        return liteLoaderVersions
            .Select(x => x as LiteLoaderVersionMetadata)
            .Where(x => x != null)
            .ToArray()!;
    }

    // vanilla
    public async Task<string> InstallAsync(string liteLoaderVersion)
    {
        var localVersionLoader = new LocalJsonVersionLoader(minecraftPath);
        var localVersions = await localVersionLoader.GetVersionMetadatasAsync()
            .ConfigureAwait(false);
        
        liteLoaderVersions = await localVersionLoader.GetVersionMetadatasAsync()
            .ConfigureAwait(false);

        var liteLoader = liteLoaderVersions?.GetVersionMetadata(liteLoaderVersion) as LiteLoaderVersionMetadata;
        if (liteLoader == null)
            throw new KeyNotFoundException(liteLoaderVersion);

        var vanillaVersionName = liteLoader.VanillaVersionName;
        var vanillaVersion = await localVersions.GetVersionAsync(vanillaVersionName)
            .ConfigureAwait(false);

        if (vanillaVersion == null)
            throw new KeyNotFoundException(vanillaVersionName);

        return await liteLoader.InstallAsync(minecraftPath, vanillaVersion);
    }

    public async Task<string> InstallAsync(string liteLoaderVersion, IVersion target)
    {
        if (liteLoaderVersions == null)
            await GetAllLiteLoaderVersions().ConfigureAwait(false);

        var liteLoader = liteLoaderVersions?.GetVersionMetadata(liteLoaderVersion) as LiteLoaderVersionMetadata;
        if (liteLoader == null)
            throw new KeyNotFoundException(liteLoaderVersion);

        return await liteLoader.InstallAsync(minecraftPath, target);
    }
}