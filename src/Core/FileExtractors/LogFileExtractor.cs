using CmlLib.Core.Tasks;
using CmlLib.Core.Version;

namespace CmlLib.Core.FileExtractors;

public class LogFileExtractor : IFileExtractor
{
    public ValueTask<IEnumerable<LinkedTask>> Extract(MinecraftPath path, IVersion version)
    {
        var result = extract(path, version);
        return new ValueTask<IEnumerable<LinkedTask>>(result);
    }

    private IEnumerable<LinkedTask> extract(MinecraftPath path, IVersion version)
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
            Hash = version.Logging?.LogFile?.GetSha1()
        };
        var task = new FileCheckTask(file);
        task.OnFalse = new DownloadTask(file);
        yield return task;
    }
}
