using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using CmlLib.Core.FileExtractors;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;

namespace CmlLib.Core.Executors;

public class TPLTaskExecutor
{
    private enum TaskStatus
    {
        Queued,
        Processing,
        Done
    }

    private readonly DataflowLinkOptions _completeLinkOptions = new()
    {
        PropagateCompletion = true
    };
    private readonly ConcurrentDictionary<string, TaskStatus> _runningTasks = new();
    private int proceed = 0;

    public void PrintStatus()
    {
        var runningTasks = _runningTasks
            .Where(kv => kv.Value != TaskStatus.Done)
            .Select(kv => kv.Key);

        foreach (var task in runningTasks)
        {
            Console.WriteLine(task);
        }
    }

    public async ValueTask Install(
        IEnumerable<IFileExtractor> extractors,
        MinecraftPath path,
        IVersion version)
    {
        var extractBlock = createExtractBlock(version, path);
        var executeBlock = createExecuteBlock(extractBlock.Completion);
        extractBlock.LinkTo(executeBlock);

        await Task.WhenAll(extractors.Select(extractor => extractBlock.SendAsync(extractor)));
        extractBlock.Complete();
        await extractBlock.Completion;

        if (proceed == _runningTasks.Count)
            return;
        else
            await executeBlock.Completion;
    }

    private BufferBlock<LinkedTask> createExecuteBlock(Task extractTask)
    {
        var buffer = new BufferBlock<LinkedTask>();
        var executor = new TransformBlock<LinkedTask, LinkedTask?>(async task =>
        {
            LinkedTask? nextTask = null;
            Exception? exception = null;
            try
            {
                fireEvent(task.Name, TaskStatus.Processing);
                nextTask = await task.Execute();
            }
            catch (Exception ex)
            {
                exception = ex;
                Debug.WriteLine(ex.ToString());
            }

            if (nextTask == null)
            {
                Interlocked.Increment(ref proceed);
                _runningTasks.TryUpdate(task.Name, TaskStatus.Done, TaskStatus.Queued);
                fireEvent(task.Name, TaskStatus.Done);

                if (proceed == _runningTasks.Count && extractTask.IsCompleted)
                {
                    buffer.Complete();
                }
            }

            return nextTask;
        }, new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = 6
        });

        buffer.LinkTo(executor);
        executor.LinkTo(buffer!, t => t != null);
        executor.LinkTo(DataflowBlock.NullTarget<LinkedTask?>());

        return buffer;
    }

    private IPropagatorBlock<IFileExtractor, LinkedTask> createExtractBlock(
        IVersion version,
        MinecraftPath path)
    {
        IEnumerable<LinkedTask> fireTasksEvent(IEnumerable<LinkedTask> tasks)
        {
            foreach (var task in tasks)
            {
                if (_runningTasks.TryAdd(task.Name, TaskStatus.Queued))
                {
                    fireEvent(task.Name, TaskStatus.Queued);
                    yield return task;
                }
            }
        }

        var block = new TransformManyBlock<IFileExtractor, LinkedTask>(async extractor =>
        {
            var tasks = await extractor.Extract(path, version);
            var iterated = fireTasksEvent(tasks);
            return iterated;
        }, new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = 6
        });

        return block;
    }

    private void fireEvent(string name, TaskStatus status)
    {
        //if (status != TaskStatus.Done) return;
        //if (proceed % 100 != 0) return;
        var totalTasks = _runningTasks.Count;
        var now = DateTime.Now.ToString("hh:mm:ss.fff");
        Console.WriteLine($"[{now}][{proceed}/{totalTasks}][{status}] {name}");
    }
}