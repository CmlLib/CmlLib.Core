namespace CmlLib.Core.Tasks;

public struct LinkedTaskHead
{
    public LinkedTaskHead(LinkedTask? first, TaskFile file) =>
        (First, File) = (first, file);

    public string Name => File.Name;
    public LinkedTask? First { get; }
    public TaskFile File { get; }

    public ValueTask<LinkedTask?> Execute(
        IProgress<ByteProgressEventArgs>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (First == null)
            return new ValueTask<LinkedTask?>();

        var context = new TaskExecutionContext(progress, cancellationToken);
        return First.Execute(progress, cancellationToken);
    }
}