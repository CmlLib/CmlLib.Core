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
    public long LinkedSize { get; protected set; }
    public long Size { get; protected set; }
    public LinkedTask? NextTask { get; private set; }

    public async ValueTask<LinkedTask?> Execute(
        IProgress<ByteProgress>? progress,
        CancellationToken cancellationToken)
    {
        var nextTask = await OnExecuted(progress, cancellationToken);
        if (nextTask != null)
        {
            if (nextTask.Name != this.Name)
                throw new InvalidOperationException("Name should be same");
            nextTask.LinkedSize = LinkedSize + Size;
        }
        
        return nextTask;
    }

    protected abstract ValueTask<LinkedTask?> OnExecuted(
        IProgress<ByteProgress>? progress, 
        CancellationToken cancellationToken);

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
        return internalLinkTasks(tasks);
    }

    public static LinkedTask? LinkTasks(IEnumerable<LinkedTask> tasks) =>
        internalLinkTasks(tasks);

    private static LinkedTask? internalLinkTasks(IEnumerable<LinkedTask> tasks)
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