namespace CmlLib.Core.Tasks;

public abstract class LinkedTask
{
    public string? Name { get; set; }
    public LinkedTask? NextTask { get; private set; }

    public abstract ValueTask<LinkedTask?> Execute();

    public LinkedTask InsertNextTask(LinkedTask task)
    {
        if (NextTask == null)
        {
            NextTask = task;
        }
        else
        {
            task.NextTask = this.NextTask;
            this.NextTask = task;
        }
        return NextTask;
    }

    public static LinkedTask? LinkTasks(params LinkedTask[] tasks)
    {
        return LinkTasks(tasks);
    }

    public static LinkedTask? LinkTasks(IEnumerable<LinkedTask> tasks)
    {
        var firstTask = tasks.FirstOrDefault();
        if (firstTask == null)
            return null;
        
        var nextTask = firstTask;
        foreach (var task in tasks.Skip(1))
        {
            nextTask.InsertNextTask(task);
            nextTask = task;
        }

        return firstTask;
    }
}