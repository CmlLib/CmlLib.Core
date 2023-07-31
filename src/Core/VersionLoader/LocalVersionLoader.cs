using CmlLib.Core.VersionMetadata;

namespace CmlLib.Core.VersionLoader;

public class LocalVersionLoader : IVersionLoader
{
    public LocalVersionLoader(MinecraftPath path)
    {
        minecraftPath = path;
    }

    private readonly MinecraftPath minecraftPath;

    public ValueTask<MVersionCollection> GetVersionMetadatasAsync()
    {
        var versions = getFromLocal(minecraftPath);
        var collection = new MVersionCollection(versions, null, null);
        return new ValueTask<MVersionCollection>(collection);
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
