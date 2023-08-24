namespace CmlLib.Core.Tasks;

public class ActionTask : LinkedTask
{
    private readonly Func<ActionTask, ValueTask<LinkedTask?>> _action;

    public ActionTask(string name, Func<ActionTask, ValueTask<LinkedTask?>> action)
     : base(name) => 
        _action = action;

    protected override async ValueTask<LinkedTask?> OnExecuted(
        IProgress<ByteProgress>? progress,
        CancellationToken cancellationToken)
    {
        return await _action.Invoke(this);
    }
}