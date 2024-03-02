using CmlLib.Core.Files;

namespace CmlLib.Core.Tasks;

public class CompositeUpdateTask : IUpdateTask
{
    public static CompositeUpdateTask Create(params IUpdateTask[] tasks)
    {
        return new CompositeUpdateTask(tasks);
    }

    private readonly IEnumerable<IUpdateTask> _tasks;

    public CompositeUpdateTask(IEnumerable<IUpdateTask> tasks)
    {
        _tasks = tasks;
    }

    public async ValueTask Execute(GameFile file, CancellationToken cancellationToken)
    {
        foreach (var task in _tasks)
        {
            await task.Execute(file, cancellationToken);
        }
    }
}