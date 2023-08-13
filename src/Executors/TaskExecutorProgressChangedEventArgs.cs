namespace CmlLib.Core.Executors;

public struct TaskExecutorProgressChangedEventArgs
{
    public TaskExecutorProgressChangedEventArgs(string name, TaskStatus status) =>
        (Name, EventType) = (name, status);

    public int TotalTasks { get; set; } = 0;
    public int ProceedTasks { get; set; } = 0;
    public TaskStatus EventType { get; }
    public string Name { get; }

    public void Print()
    {
        //if (status != TaskStatus.Done) return;
        //if (proceed % 100 != 0) return;
        var now = DateTime.Now.ToString("hh:mm:ss.fff");
        Console.WriteLine($"[{now}][{ProceedTasks}/{TotalTasks}][{EventType}] {Name}");
    }
}