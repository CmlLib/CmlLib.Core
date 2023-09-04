using CmlLib.Core.VersionMetadata;

namespace CmlLib.Core.VersionLoader;

public class LocalJsonVersionLoader : IVersionLoader
{
    public LocalJsonVersionLoader(MinecraftPath path)
    {
        minecraftPath = path;
    }

    private readonly MinecraftPath minecraftPath;

    public ValueTask<VersionMetadataCollection> GetVersionMetadatasAsync()
    {
        var versions = getFromLocal(minecraftPath);
        var collection = new VersionMetadataCollection(versions, null, null);
        return new ValueTask<VersionMetadataCollection>(collection);
    }

    private IEnumerable<IVersionMetadata> getFromLocal(MinecraftPath path)
    {
        var versionDirectory = new DirectoryInfo(path.Versions);
        if (!versionDirectory.Exists)
            yield break;
        
        var dirs = versionDirectory.GetDirectories();
        foreach (var dir in dirs)
        {
            var filepath = Path.Combine(dir.FullName, dir.Name + ".json");
            if (!File.Exists(filepath)) continue;

            var model = new JsonVersionMetadataModel
            {
                Name = dir.Name,
                Type = "local",
            };
            yield return new LocalVersionMetadata(model, filepath);
        }
    }
}
