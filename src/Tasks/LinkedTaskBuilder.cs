using System.Diagnostics;

namespace CmlLib.Core.Tasks;

public class LinkedTaskBuilder
{
    private readonly ITaskFactory _taskFactory;

    public TaskFile TaskFile { get; private set; }
    public LinkedTask? Task { get; private set; }
    private LinkedTask? Tail { get; set; }
    public bool IsReadOnly { get; private set; }

    private LinkedTaskBuilder(
        TaskFile taskFile, 
        ITaskFactory taskFactory,
        LinkedTask? head, 
        LinkedTask? tail, 
        bool isReadOnly)
    {
        TaskFile = taskFile;
        _taskFactory = taskFactory;
        Task = head;
        Tail = tail;
        IsReadOnly = isReadOnly;
    }

    public static LinkedTaskBuilder Create(TaskFile taskFile, ITaskFactory taskFactory) => 
        new LinkedTaskBuilder(taskFile, taskFactory, null, null, false);

    public static LinkedTaskBuilder CreateReadOnly(TaskFile taskFile, ITaskFactory taskFactory, LinkedTask? head) =>
        new LinkedTaskBuilder(taskFile, taskFactory, head, null, true);

    public LinkedTaskBuilder Then(LinkedTask task)
    {
        if (IsReadOnly)
            return this;

        if (Task == null)
        {
            Task = task;
            Tail = task;
        }
        else
        {
            Debug.Assert(Tail != null);

            Tail.InsertNextTask(task);
            Tail = task;
        }

        return this;
    }

    public LinkedTaskBuilder ThenIf(bool condition)
    {
        if (condition)
            return this;
        else
            return CreateReadOnly(TaskFile, _taskFactory, Task);
    }

    public LinkedTaskBuilder CheckFile(LinkedTask onSuccess, LinkedTask onFail)
    {
        return Then(_taskFactory.CheckFile(TaskFile, onSuccess, onFail));
    }

    public LinkedTaskBuilder CheckFile(
        Func<LinkedTaskBuilder, LinkedTaskBuilder> onSuccess, 
        Func<LinkedTaskBuilder, LinkedTaskBuilder> onFail)
    {
        return CheckFile(
            onSuccess.Invoke(Create(TaskFile, _taskFactory)).BuildTask() ?? throw new InvalidOperationException(),
            onFail.Invoke(Create(TaskFile, _taskFactory)).BuildTask() ?? throw new InvalidOperationException());
    }

    public LinkedTaskBuilder Download()
    {
        return Then(_taskFactory.Download(TaskFile));
    }

    public LinkedTaskBuilder ReportDone()
    {
        return Then(_taskFactory.ReportDone(TaskFile));
    }

    public LinkedTaskBuilder CheckAndDownload()
    {
        return Then(_taskFactory.CheckAndDownload(TaskFile));
    }

    public LinkedTaskHead BuildHead()
    {
        if (Task == null)
            throw new InvalidOperationException();
        return new LinkedTaskHead(Task, TaskFile);
    }

    public LinkedTask BuildTask()
    {
        if (Task == null)
            throw new InvalidOperationException();
        return Task;
    }
}