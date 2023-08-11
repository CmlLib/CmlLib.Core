namespace CmlLib.Core.Tasks;

public abstract class LinkedTask
{
    public LinkedTask(TaskFile file)
    {
        if (string.IsNullOrEmpty(file.Name))
            throw new ArgumentException("file.Name was empty");
        this.Name = file.Name;
    }

    public LinkedTask(string name)
    {
        this.Name = name;
    }

    public string Name { get; set; }
    public LinkedTask? NextTask { get; private set; }

    public async ValueTask<LinkedTask?> Execute()
    {
        var nextTask = await OnExecuted();
        if (nextTask != null && nextTask.Name != this.Name)
            throw new InvalidOperationException("Name should be same");
        return nextTask;
    }

    protected abstract ValueTask<LinkedTask?> OnExecuted();

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