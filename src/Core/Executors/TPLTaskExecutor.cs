using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;
using CmlLib.Core.FileExtractors;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;

namespace CmlLib.Core.Executors;

public class TPLTaskExecutor
{
    private enum TaskStatus
    {
        Processing,
        Done
    }

    private readonly ConcurrentDictionary<string, TaskStatus> _runningTasks = new();
    private int proceed = 0;

    private readonly DataflowLinkOptions _linkOptions = new()
    {
        PropagateCompletion = true
    };

    public async ValueTask Install(
        IEnumerable<IFileExtractor> extractors,
        MinecraftPath path,
        IVersion version)
    {
        var executor = createTaskExecutorBlock();
        var installer = completeBlock(executor, path, extractors);

        await installer.SendAsync(version);
    }

    private BufferBlock<LinkedTask> createTaskExecutorBlock()
    {
        var buffer = new BufferBlock<LinkedTask>();
        var executor = new TransformBlock<LinkedTask, LinkedTask?>(async task =>
        {
            if (_runningTasks.TryAdd(task.Name, TaskStatus.Processing))
                fireEvent(task.Name, TaskStatus.Processing);
            var nextTask = await task.Execute();

            if (nextTask == null || nextTask.Name != task.Name)
            {
                Interlocked.Increment(ref proceed);
                _runningTasks.TryUpdate(task.Name, TaskStatus.Done, TaskStatus.Processing);
                fireEvent(task.Name, TaskStatus.Done);

                if (proceed == _runningTasks.Count) // TODO: check also extractor is all done
                {
                    Console.WriteLine("ALL DONE");
                }
            }

            return nextTask;
        }, new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = 8
        });

        buffer.LinkTo(executor, _linkOptions);
        executor.LinkTo(buffer!, _linkOptions, t => t != null);

        return buffer;
    }

    private ITargetBlock<IVersion> completeBlock(
        ITargetBlock<LinkedTask> executor,
        MinecraftPath path,
        IEnumerable<IFileExtractor> extractors)
    {
        var broadcaster = new BroadcastBlock<IVersion>(null);
        foreach (var extractor in extractors)
        {
            var block = new TransformManyBlock<IVersion, LinkedTask>(async v =>
            {
                return await extractor.Extract(path, v);
            });
            broadcaster.LinkTo(block, _linkOptions);
            block.LinkTo(executor, _linkOptions);
        }

        return broadcaster;
    }

    private void fireEvent(string name, TaskStatus status)
    {
        var totalTasks = _runningTasks.Count;
        var statusMsg = status == TaskStatus.Processing ? "START" : "END";
        Console.WriteLine($"[{proceed}/{totalTasks}][{statusMsg}] {name}");
    }
}