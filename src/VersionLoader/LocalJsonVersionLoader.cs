using CmlLib.Core.VersionMetadata;

namespace CmlLib.Core.VersionLoader;

public class LocalJsonVersionLoader : IVersionLoader
{
    public LocalJsonVersionLoader(MinecraftPath path)
    {
        minecraftPath = path;
    }

    private readonly MinecraftPath minecraftPath;

    public ValueTask<VersionMetadataCollection> GetVersionMetadatasAsync(CancellationToken cancellationToken = default)
    {
        var versions = GetVersionNameAndPaths()
            .Select(nameAndPath => toMetadata(nameAndPath.Item1, nameAndPath.Item2));
        var collection = new VersionMetadataCollection(versions, null, null);
        return new ValueTask<VersionMetadataCollection>(collection);
    }

    private IVersionMetadata toMetadata(string name, string path)
    {
        var model = new JsonVersionMetadataModel
        {
            Id = name,
            Type = "local",
        };
        return new LocalVersionMetadata(model, path);
    }

    public IEnumerable<(string, string)> GetVersionNameAndPaths()
    {
        var versionDirectory = new DirectoryInfo(minecraftPath.Versions);
        if (!versionDirectory.Exists)
            yield break;
        
        var dirs = versionDirectory.GetDirectories();
        foreach (var dir in dirs)
        {
            var filepath = Path.Combine(dir.FullName, dir.Name + ".json");
            if (!File.Exists(filepath)) continue;
            yield return (dir.Name, filepath);
        }
    }
}
