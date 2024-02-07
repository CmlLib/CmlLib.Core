using CmlLib.Core.Rules;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;

namespace CmlLib.Core.FileExtractors;

public class LibraryFileExtractor : IFileExtractor
{
    private readonly string _side;
    private readonly IRulesEvaluator _rulesEvaluator;

    public LibraryFileExtractor(string side, IRulesEvaluator rulesEvaluator)
    {
        _side = side;
        _rulesEvaluator = rulesEvaluator;
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

    public ValueTask<IEnumerable<LinkedTaskHead>> Extract(
        ITaskFactory taskFactory,
        MinecraftPath path, 
        IVersion version,
        RulesEvaluatorContext rulesContext,
        CancellationToken cancellationToken)
    {
        var result = version.Libraries
            .Where(lib => lib.CheckIsRequired(_side))
            .Where(lib => lib.Rules == null || _rulesEvaluator.Match(lib.Rules, rulesContext))
            .SelectMany(lib => createLibraryTasks(taskFactory, path, lib, rulesContext));
        return new ValueTask<IEnumerable<LinkedTaskHead>>(result);
    }

    private IEnumerable<LinkedTaskHead> createLibraryTasks(
        ITaskFactory taskFactory,
        MinecraftPath path, 
        MLibrary library,
        RulesEvaluatorContext rulesContext)
    {
        // java library (*.jar)
        var artifact = library.Artifact;
        if (artifact != null)
        {
            var libPath = library.GetLibraryPath();
            var file = new TaskFile(library.Name)
            {
                Path = Path.Combine(path.Library, libPath),
                Url = createDownloadUrl(artifact.Url, libPath),
                Hash = artifact.GetSha1(),
                Size = artifact.Size
            };

            yield return new LinkedTaskHead(taskFactory.CheckAndDownload(file), file);
        }

        // native library (*.dll, *.so)
        var native = library.GetNativeLibrary(rulesContext.OS);
        if (native != null)
        {
            var libPath = library.GetNativeLibraryPath(rulesContext.OS);
            if (!string.IsNullOrEmpty(libPath))
            {
                var file = new TaskFile(library.Name)
                {
                    Path = Path.Combine(path.Library, libPath),
                    Url = createDownloadUrl(native.Url, libPath),
                    Hash = native.GetSha1(),
                    Size = native.Size
                };
                yield return new LinkedTaskHead(taskFactory.CheckAndDownload(file), file);
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
