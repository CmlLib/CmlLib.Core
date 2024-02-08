using CmlLib.Core.Rules;
using CmlLib.Core.Tasks;
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
        var id = version.JarId;
        var url = version.Client?.Url;
        
        if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(id))
            yield break;

        var clientPath = path.GetVersionJarPath(id);
        yield return new GameFile(id)
        {
            Path = clientPath,
            Url = url,
            Hash = version.Client?.GetSha1(),
            Size = version.Client?.Size ?? 0
        };
    }
}
