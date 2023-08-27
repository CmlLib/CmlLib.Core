namespace CmlLib.Core.Tasks;

public class ProgressTask : LinkedTask
{
    public static ProgressTask CreateDoneTask(TaskFile file)
    {
        var task = new ProgressTask(file, new ByteProgress
        {
            TotalBytes = file.Size,
            ProgressedBytes = file.Size
        });
        task.Size = file.Size;
        return task;
    }

    public ProgressTask(TaskFile file, ByteProgress progress) : base(file) =>
        Progress = progress;

    public ProgressTask(string name, ByteProgress progress) : base(name) =>
        Progress = progress;

    public ByteProgress Progress { get; }

    protected override ValueTask<LinkedTask?> OnExecuted(
        IProgress<ByteProgress>? progress, 
        CancellationToken cancellationToken)
    {
        progress?.Report(Progress);
        return new ValueTask<LinkedTask?>(NextTask);
    }
}
