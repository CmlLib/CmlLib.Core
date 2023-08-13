using CmlLib.Core.Tasks;

namespace CmlLib.Core.Executors;

public class AsyncTaskExecutor : IDisposable
{
    private readonly SemaphoreSlim _semaphore;

    public AsyncTaskExecutor(int maxParallelism)
    {
        _semaphore = new SemaphoreSlim(maxParallelism);
    }

    public ValueTask<LinkedTask?> QueueTask(
        LinkedTask task,
        IProgress<ByteProgressEventArgs> progress,
        CancellationToken cancellationToken)
    {
        var context = new TaskExecutionContext(progress, cancellationToken);
        return createQueuedTask(task, progress, cancellationToken);
    }

    private async ValueTask<LinkedTask?> createQueuedTask(
        LinkedTask task, 
        IProgress<ByteProgressEventArgs>? progress,
        CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync();

        LinkedTask? nextTask = null;
        try
        {
            nextTask = await task.Execute(progress, cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }
        return nextTask;
    }


    public void Dispose()
    {
        ((IDisposable)_semaphore).Dispose();
    }
}