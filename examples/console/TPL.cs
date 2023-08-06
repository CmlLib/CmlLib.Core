using System.Threading.Tasks.Dataflow;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;

namespace CmlLibCoreSample;

public class MockTask : LinkedTask
{
    public int Count { get; set; }

    public MockTask(string name) : base(name)
    {
    }

    protected override ValueTask<LinkedTask?> OnExecuted()
    {
        Console.WriteLine($"{Name}: {Count}");
        if (Count == 0)
            return new ValueTask<LinkedTask?>();
        else
        {
            var nextTask = new MockTask(Name);
            nextTask.Count = Count - 1;
            return new ValueTask<LinkedTask?>(nextTask);
        }
    }
}

public class TPL
{
    public async Task Test()
    {
        var linkOptions = new DataflowLinkOptions
        {
            PropagateCompletion = true
        };

        var buffer = new BufferBlock<MockTask>();
        var executor = new TransformBlock<MockTask, MockTask?>(
            async f =>
            {
                var next = await f.Execute();
                return next as MockTask;
            },
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 8
            });

        buffer.LinkTo(executor, linkOptions);
        executor.LinkTo(buffer!, linkOptions, f => f != null);
        executor.LinkTo(DataflowBlock.NullTarget<LinkedTask?>(), linkOptions);

        var versionBroadcaster = new BroadcastBlock<string>(null);
        for (var i = 0; i < 4; i++)
        {
            int copy = i;
            var extractor = new TransformManyBlock<string, MockTask>(
                v => extract(v, copy));
            versionBroadcaster.LinkTo(extractor, linkOptions);
            extractor.LinkTo(buffer, linkOptions);
        }

        await versionBroadcaster.SendAsync("1.20.1");
        Console.ReadLine();
        Console.WriteLine("DONE");
    }

    private async IAsyncEnumerable<MockTask> extract(string version, int i)
    {
        Console.WriteLine($"extractor {i}");
        for (int j = 0; j < 4; j++)
        {
            await Task.Delay(100);
            Console.WriteLine("extracted " + (i*10+j));
            yield return new MockTask((i*10 + j).ToString())
            {
                Count = j
            };
        }
    }
}