namespace CmlLib.Core.Tasks;

public abstract class ResultTask : LinkedTask
{
    public ResultTask(string name) : base(name)
    {

    }

    public ResultTask(TaskFile file) : base(file)
    {

    }

    public LinkedTask? OnTrue { get; set; }
    public LinkedTask? OnFalse { get; set; }

    protected override async ValueTask<LinkedTask?> OnExecuted()
    {
        var result = await OnExecutedWithResult();
        var nextTask = result ? OnTrue : OnFalse;

        if (nextTask != null)
            return InsertNextTask(nextTask);
        else
            return NextTask;
    }

    protected abstract ValueTask<bool> OnExecutedWithResult(); 
}