namespace CmlLib.Core.Tasks;

public class LZMADecompressTask : LinkedTask
{
    public LZMADecompressTask(string name, string lzmaPath, string decompressTo) : base(name)
    {

    }

    protected override ValueTask<LinkedTask?> OnExecuted()
    {
        throw new NotImplementedException();
    }
}