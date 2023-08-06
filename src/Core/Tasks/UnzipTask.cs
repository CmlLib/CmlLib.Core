namespace CmlLib.Core.Tasks;

public class UnzipTask : LinkedTask
{
    public UnzipTask(string name, string zipPath, string unzipTo) : base(name)
    {
        
    }

    protected override ValueTask<LinkedTask?> OnExecuted()
    {
        throw new NotImplementedException();
    }
}