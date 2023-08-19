using System.Threading.Tasks.Dataflow;
using BenchmarkDotNet.Attributes;
using CmlLib.Core.Tasks;

namespace CmlLib.Core.Benchmarks;

public abstract class ExecutorBenchmarkBase
{
    public static ByteProgress LastEvent;

    public int MaxParallelism { get; set; } = 6;
    public string Name { get; set; } = "D";
    public int Count { get; set; } = 1024*2; // 256
    public int Size { get; set; } = 1024*16; // 1024*128
    public bool Verbose { get; set; } = false;

    protected int TotalTasks = 0;
    protected int ProceedTasks = 0;
    protected LinkedTask[] Tasks { get; private set; } = null!;
    public event EventHandler<ByteProgress>? ByteProgress;

    public async Task IterationSetup()
    {
        Setup();

        var minecraftPath = new MinecraftPath();
        var version = new DummyVersion();
        var taskExtractor = new DummyDownloaderExtractor(Name, Count, Size);
        var result = await taskExtractor.Extract(minecraftPath, version);

        var list = new List<LinkedTask>();
        foreach (var head in result)
        {
            if (head.First == null)
                continue;
            
            OnTaskAdded(head);
            Interlocked.Increment(ref TotalTasks);
            list.Add(head.First);
        }
        Tasks = list.ToArray();

        ByteProgress += (s, e) => LastEvent = e;
        if (Verbose)
            ByteProgress += (s, e) => Console.WriteLine($"{e.ProgressedBytes} / {e.TotalBytes}");
    }

    protected abstract void Setup();

    protected virtual void OnTaskAdded(LinkedTaskHead head)
    {
        
    }


    public async Task Benchmark()
    {
        var block = CreateExecutorBlock();
        Task? lastSendTask = null;
        foreach (var item in Tasks)
        {
            lastSendTask = block.SendAsync(item);
        }
        if (lastSendTask == null)
            return;
        await lastSendTask;

        var executeTask = block.Completion;
        while (!executeTask.IsCompleted)
        {
            OnReportEvent();
            await Task.Delay(100);
        }
        OnReportEvent();
        await executeTask;
    }

    protected virtual BufferBlock<LinkedTask> CreateExecutorBlock()
    {

        var buffer = new BufferBlock<LinkedTask>();
        var executor = new TransformBlock<LinkedTask, LinkedTask?>(
        async t => 
        {
            var result = await Transform(t);
            if (result == null)
            {
                Interlocked.Increment(ref ProceedTasks);
                if (TotalTasks == ProceedTasks)
                    buffer.Complete();
            }
            return result;
        }, 
        new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = MaxParallelism
        });
        buffer.LinkTo(executor);
        executor.LinkTo(buffer!, t => t != null);
        executor.LinkTo(DataflowBlock.NullTarget<LinkedTask?>());
        return buffer;
    }

    protected virtual ValueTask<LinkedTask?> Transform(LinkedTask task)
    {
        return new ValueTask<LinkedTask?>(task.NextTask);
    }
    protected abstract void OnReportEvent();

    protected void FireProgress(ByteProgress progress)
    {
        ByteProgress?.Invoke(this, progress);
    }
}