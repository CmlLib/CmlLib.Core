using CmlLib.Core.Tasks;
using CmlLib.Core.Version;

namespace CmlLib.Core.FileExtractors;

public class ClientFileExtractor : IFileExtractor
{
    public ValueTask<IEnumerable<LinkedTask>> Extract(MinecraftPath path, IVersion version)
    {
        var result = extract(path, version);
        return new ValueTask<IEnumerable<LinkedTask>>(result);
    }

    private IEnumerable<LinkedTask> extract(MinecraftPath path, IVersion version)
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
        checkTask.OnFalse = new DownloadTask(file);
        yield return checkTask;
    }
}
