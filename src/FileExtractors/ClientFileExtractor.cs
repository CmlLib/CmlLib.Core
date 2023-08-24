using CmlLib.Core.Rules;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;

namespace CmlLib.Core.FileExtractors;

public class ClientFileExtractor : IFileExtractor
{
    private readonly HttpClient _httpClient;

    public ClientFileExtractor(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public ValueTask<IEnumerable<LinkedTaskHead>> Extract(
        MinecraftPath path, 
        IVersion version,
        RulesEvaluatorContext rulesContext,
        CancellationToken cancellationToken)
    {
        var result = extract(path, version);
        return new ValueTask<IEnumerable<LinkedTaskHead>>(result);
    }

    private IEnumerable<LinkedTaskHead> extract(MinecraftPath path, IVersion version)
    {
        var id = version.Jar;
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

        var checkTask = new FileCheckTask(file);
        checkTask.OnFalse = new DownloadTask(file, _httpClient);

        yield return new LinkedTaskHead(checkTask, file);
    }
}
