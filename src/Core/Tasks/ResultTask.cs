namespace CmlLib.Core.Tasks;

public abstract class ResultTask : LinkedTask
{
    public LinkedTask? OnTrue { get; set; }
    public LinkedTask? OnFalse { get; set; }

    public override async ValueTask<LinkedTask?> Execute()
    {
        var result = await ExecuteWithResult();
        var nextTask = result ? OnTrue : OnFalse;

        if (nextTask != null)
            return InsertNextTask(nextTask);
        else
            return NextTask;
    }

    protected abstract ValueTask<bool> ExecuteWithResult(); 
}