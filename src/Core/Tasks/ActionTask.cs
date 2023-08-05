namespace CmlLib.Core.Tasks;

public class ActionTask : LinkedTask
{
    private readonly Func<ValueTask<LinkedTask?>> _action;

    public ActionTask(Func<ValueTask<LinkedTask?>> action) => 
        _action = action;

    public override async ValueTask<LinkedTask?> Execute()
    {
        return await _action.Invoke();
    }
}