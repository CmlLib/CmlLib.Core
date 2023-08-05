using CmlLib.Core.Rules;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;

namespace CmlLib.Core.FileExtractors;

public class LibraryFileExtractor : IFileExtractor
{
    private readonly RulesEvaluatorContext _rulesContext;
    private readonly IRulesEvaluator _rulesEvaluator;

    public LibraryFileExtractor(IRulesEvaluator rulesEvaluator, RulesEvaluatorContext context)
    {
        this._rulesEvaluator = rulesEvaluator;
        this._rulesContext = context;
    }

    private string libServer = MojangServer.Library;
    public string LibraryServer
    {
        get => libServer;
        set
        {
            if (value.Last() == '/')
                libServer = value;
            else
                libServer = value + "/";
        }
    }

    public ValueTask<IEnumerable<LinkedTask>> Extract(MinecraftPath path, IVersion version)
    {
        var result = extract(path, version);
        return new ValueTask<IEnumerable<LinkedTask>>(result);
    }

    private IEnumerable<LinkedTask> extract(MinecraftPath path, IVersion version)
    {
        return version.Libraries
            .Where(lib => lib.CheckIsRequired("SIDE"))
            .Where(lib => lib.Rules == null || _rulesEvaluator.Match(lib.Rules, _rulesContext))
            .SelectMany(lib => createLibraryTasks(path, lib));
    }

    private IEnumerable<LinkedTask> createLibraryTasks(MinecraftPath path, MLibrary library)
    {
        var artifact = library.Artifact;
        if (artifact != null)
        {
            var libPath = library.GetLibraryPath();
            var file = new TaskFile
            {
                Name = library.Name,
                Path = Path.Combine(path.Library, libPath),
                Url = createDownloadUrl(artifact.Url, libPath),
                Hash = artifact.GetSha1()
            };

            var task = new FileCheckTask(file);
            task.OnFalse = new DownloadTask(file);
            yield return task;
        }

        var native = library.GetNativeLibrary(_rulesContext.OS);
        if (native != null)
        {
            var libPath = library.GetNativeLibraryPath(_rulesContext.OS);
            if (!string.IsNullOrEmpty(libPath))
            {
                var file = new TaskFile
                {
                    Name = library.Name,
                    Path = Path.Combine(path.Library, libPath),
                    Url = createDownloadUrl(native.Url, libPath),
                    Hash = native.GetSha1()
                };

                var task = new FileCheckTask(file);
                task.OnFalse = new DownloadTask(file);
                yield return task;
            }
        }
    }

    private string? createDownloadUrl(string? url, string path)
    {
        if (string.IsNullOrEmpty(url) && string.IsNullOrEmpty(path))
            return null;

        if (url == null)
            url = LibraryServer + path;
        else if (url == "")
            url = null;
        else if (url.Split('/').Last() == "")
            url += path.Replace("\\", "/");

        return url;
    }
}
