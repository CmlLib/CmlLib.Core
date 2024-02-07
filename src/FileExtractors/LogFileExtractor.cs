using CmlLib.Core.Rules;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;

namespace CmlLib.Core.FileExtractors;

public class LogFileExtractor : IFileExtractor
{
    public ValueTask<IEnumerable<LinkedTaskHead>> Extract(
        ITaskFactory taskFactory,
        MinecraftPath path, 
        IVersion version,
        RulesEvaluatorContext rulesContext,
        CancellationToken cancellationToken)
    {
        var result = extract(taskFactory, path, version);
        return new ValueTask<IEnumerable<LinkedTaskHead>>(result);
    }

    private IEnumerable<LinkedTaskHead> extract(
        ITaskFactory taskFactory, 
        MinecraftPath path, 
        IVersion version)
    {
        if (version.Logging == null)
            yield break;
        
        var url = version.Logging?.LogFile?.Url;
        if (string.IsNullOrEmpty(url))
            yield break;
        
        var id = version.Logging?.LogFile?.Id ?? version.Id;

        var file = new TaskFile(id)
        {
            Path = path.GetLogConfigFilePath(id),
            Url = url,
            Hash = version.Logging?.LogFile?.GetSha1(),
            Size = version.Logging?.LogFile?.Size ?? 0
        };

        yield return new LinkedTaskHead(taskFactory.CheckAndDownload(file), file);
    }
}
