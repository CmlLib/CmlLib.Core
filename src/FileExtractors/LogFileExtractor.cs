using CmlLib.Core.Rules;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;

namespace CmlLib.Core.FileExtractors;

public class LogFileExtractor : IFileExtractor
{
    private readonly HttpClient _httpClient;

    public LogFileExtractor(HttpClient httpClient)
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
        var task = new FileCheckTask(file);
        task.OnTrue = ProgressTask.CreateDoneTask(file);
        task.OnFalse = new DownloadTask(file, _httpClient);
        yield return new LinkedTaskHead(task, file);
    }
}
