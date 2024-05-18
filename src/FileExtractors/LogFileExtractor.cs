using CmlLib.Core.Rules;
using CmlLib.Core.Files;
using CmlLib.Core.Version;

namespace CmlLib.Core.FileExtractors;

public class LogFileExtractor : IFileExtractor
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

    private IEnumerable<GameFile> extract(
        MinecraftPath path, 
        IVersion version)
    {
        if (version.Logging == null)
            yield break;
        
        var url = version.Logging?.LogFile?.Url;
        if (string.IsNullOrEmpty(url))
            yield break;
        
        var id = version.Logging?.LogFile?.Id ?? version.Id;
        yield return new GameFile(id)
        {
            Path = path.GetLogConfigFilePath(id),
            Url = url,
            Hash = version.Logging?.LogFile?.GetSha1(),
            Size = version.Logging?.LogFile?.Size ?? 0
        };
    }
}
