namespace CmlLib.Core.Tasks;

public class TaskExecutionContext
{
    public TaskExecutionContext(
        IProgress<ByteProgressEventArgs>? progress, 
        CancellationToken cancellationToken)
    {
        (ProgressChanged, CancellationToken) = (progress, cancellationToken);
    }

    public CancellationToken CancellationToken { get; }
    public IProgress<ByteProgressEventArgs>? ProgressChanged { get; }
}