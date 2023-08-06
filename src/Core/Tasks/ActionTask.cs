namespace CmlLib.Core.Tasks;

public class ActionTask : LinkedTask
{
    private readonly Func<ValueTask<LinkedTask?>> _action;

    public ActionTask(string name, Func<ValueTask<LinkedTask?>> action) : base(name) => 
        _action = action;

    protected override async ValueTask<LinkedTask?> OnExecuted()
    {
        return await _action.Invoke();
    }
}