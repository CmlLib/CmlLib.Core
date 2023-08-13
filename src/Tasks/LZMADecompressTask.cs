using CmlLib.Core.Internals;

namespace CmlLib.Core.Tasks;

public class LZMADecompressTask : LinkedTask
{
    public string LZMAPath { get; set; }
    public string ExtractPath { get; set; }

    public LZMADecompressTask(string name, string lzmaPath, string extractTo) : base(name)
    {
        LZMAPath = lzmaPath;
        ExtractPath = extractTo;
    }

    protected override ValueTask<LinkedTask?> OnExecuted(
        IProgress<ByteProgress>? progress,
        CancellationToken cancellationToken)
    {
        SevenZipWrapper.DecompressFileLZMA(LZMAPath, ExtractPath);
        return new ValueTask<LinkedTask?>();
    }
}