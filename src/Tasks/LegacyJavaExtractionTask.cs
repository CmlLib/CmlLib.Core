﻿using CmlLib.Core.Internals;

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
            throw new ArgumentException();

        var zipPath = Path.Combine(Path.GetTempPath(), "jre.zip");
        SevenZipWrapper.DecompressFileLZMA(file.Path, zipPath);
        SharpZipWrapper.Unzip(zipPath, ExtractTo, null, cancellationToken);

        return new ValueTask();
    }
}