using CmlLib.Utils;

namespace CmlLib.Core.Tasks;

public class UnzipTask : LinkedTask
{
    public string ZipPath { get; set; }
    public string ExtractTo { get; set; }

    public UnzipTask(string name, string zipPath, string unzipTo) : base(name)
    {
        ZipPath = zipPath;
        ExtractTo = unzipTo;
    }

    protected override ValueTask<LinkedTask?> OnExecuted()
    {
        SharpZipWrapper.Unzip(ZipPath, ExtractTo, null);
        return new ValueTask<LinkedTask?>(NextTask);
    }
}