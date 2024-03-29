using CmlLib.Core.Rules;
using CmlLib.Core.Files;
using CmlLib.Core.Version;

namespace CmlLib.Core.FileExtractors;

public class ClientFileExtractor : IFileExtractor
{
    public ValueTask<IEnumerable<GameFile>> Extract(
        MinecraftPath path, 
        IVersion version,
        RulesEvaluatorContext rulesContext,
        CancellationToken cancellationToken)
    {
        var result = extract(path, version);
        return new ValueTask<IEnumerable<GameFile>>(result);
    }

    private IEnumerable<GameFile> extract(MinecraftPath path, IVersion version)
    {
        var id = version.MainJarId;
        var client = version.GetInheritedProperty(v => v.Client);
        
        if (string.IsNullOrEmpty(client?.Url) || string.IsNullOrEmpty(id))
            yield break;

        var clientPath = path.GetVersionJarPath(id);
        yield return new GameFile(id)
        {
            Path = clientPath,
            Url = client.Url,
            Hash = client.GetSha1(),
            Size = client.Size
        };
    }
}
