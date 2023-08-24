namespace CmlLib.Core.Tasks;

public class TaskExecutionContext
{
    public TaskExecutionContext(
        IProgress<ByteProgress>? progress, 
        CancellationToken cancellationToken) =>
        (ProgressChanged, CancellationToken) = (progress, cancellationToken);

    public CancellationToken CancellationToken { get; }
    public IProgress<ByteProgress>? ProgressChanged { get; }
}