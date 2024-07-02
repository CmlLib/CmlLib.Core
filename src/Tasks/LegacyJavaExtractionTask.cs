using CmlLib.Core.Internals;
using CmlLib.Core.Files;

namespace CmlLib.Core.Tasks;

public class LegacyJavaExtractionTask : IUpdateTask
{
    public LegacyJavaExtractionTask(string extractTo)
    {
        ExtractTo = extractTo;
    }

    public string ExtractTo { get; }

    public ValueTask Execute(GameFile file, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(file.Path))
            throw new InvalidOperationException("file.Path was null");

        // jre.lzma (file.Path) -> jre.zip -> /extracTo
        var zipPath = Path.Combine(Path.GetTempPath(), "jre.zip");
        SevenZipWrapper.DecompressFileLZMA(file.Path, zipPath);
        SharpZipWrapper.Unzip(zipPath, ExtractTo, [], cancellationToken);

        return new ValueTask();
    }
}