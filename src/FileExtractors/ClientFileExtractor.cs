using CmlLib.Core.Rules;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;

namespace CmlLib.Core.FileExtractors;

public class ClientFileExtractor : IFileExtractor
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

    private IEnumerable<LinkedTaskHead> extract(ITaskFactory taskFactory, MinecraftPath path, IVersion version)
    {
        var id = version.JarId;
        var url = version.Client?.Url;
        
        if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(id))
            yield break;

        var clientPath = path.GetVersionJarPath(id);
        var file = new TaskFile(id)
        {
            Path = clientPath,
            Url = url,
            Hash = version.Client?.GetSha1(),
            Size = version.Client?.Size ?? 0
        };

        yield return new LinkedTaskHead(taskFactory.CheckAndDownload(file), file);
    }
}
