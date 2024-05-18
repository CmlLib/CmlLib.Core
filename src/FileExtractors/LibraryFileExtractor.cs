using CmlLib.Core.Internals;
using CmlLib.Core.Rules;
using CmlLib.Core.Files;
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

    public string LibraryServer { get; set; } = MojangServer.Library;

    public ValueTask<IEnumerable<GameFile>> Extract(
        MinecraftPath path,
        IVersion version,
        RulesEvaluatorContext rulesContext,
        CancellationToken cancellationToken)
    {
        var result = version.Libraries
            .Where(lib => lib.CheckIsRequired(_side))
            .Where(lib => lib.Rules == null || _rulesEvaluator.Match(lib.Rules, rulesContext))
            .SelectMany(lib => Extractor.ExtractTasks(LibraryServer, path, lib, rulesContext));
        return new ValueTask<IEnumerable<GameFile>>(result);
    }

    public static class Extractor
    {
        public static IEnumerable<GameFile> ExtractTasks(
            string libraryServer,
            MinecraftPath path,
            MLibrary library,
            RulesEvaluatorContext rulesContext)
        {
            if (!libraryServer.EndsWith("/"))
                libraryServer += '/';

            // java library (*.jar)
            var artifact = library.Artifact;
            if (artifact != null)
            {
                var libPath = library.GetLibraryPath();
                yield return new GameFile(library.Name)
                {
                    Path = IOUtil.NormalizePath(Path.Combine(path.Library, libPath)),
                    Url = createDownloadUrl(libraryServer, artifact.Url, libPath),
                    Hash = artifact.GetSha1(),
                    Size = artifact.Size
                };
            }

            // native library (*.dll, *.so)
            var native = library.GetNativeLibrary(rulesContext.OS);
            if (native != null)
            {
                var libPath = library.GetNativeLibraryPath(rulesContext.OS);
                if (!string.IsNullOrEmpty(libPath))
                {
                    yield return new GameFile(library.Name)
                    {
                        Path = IOUtil.NormalizePath(Path.Combine(path.Library, libPath)),
                        Url = createDownloadUrl(libraryServer, native.Url, libPath),
                        Hash = native.GetSha1(),
                        Size = native.Size
                    };
                }
            }
        }

        private static string? createDownloadUrl(string server, string? url, string path)
        {
            if (string.IsNullOrEmpty(url) && string.IsNullOrEmpty(path))
                return null;

            if (url == null)
                url = server + path.Replace("\\", "/");
            else if (url == "")
                url = null;
            else if (url.Split('/').Last() == "")
                url += path.Replace("\\", "/");

            return url;
        }
    }
}
