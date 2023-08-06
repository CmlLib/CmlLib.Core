using System.Threading.Tasks.Dataflow;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;

namespace CmlLibCoreSample;

public class TPL
{
    public async Task Test()
    {
        var linkOptions = new DataflowLinkOptions
        {
            PropagateCompletion = true
        };

        var buffer = new BufferBlock<TaskFile>();
        var executor = new TransformManyBlock<TaskFile, TaskFile>(
            f => 
            {    
                if (f.Size < 0) 
                    Console.WriteLine($"@@@ ERROR {f.Name}, {f.Size}");
                else
                    Console.WriteLine($"{f.Name}, {f.Size}");
                Thread.Sleep(100);

                var list = new List<TaskFile>();
                for (int i = 0; i < f.Size; i++)
                {
                    list.Add(new TaskFile(f.Name)
                    {
                        Size = f.Size - 1
                    });
                }
                return list;
            },
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 8
            });
        var done = new ActionBlock<TaskFile>(f => Console.WriteLine($"DONE {f.Name} {f.Size}"));

        buffer.LinkTo(executor, linkOptions);
        executor.LinkTo(done, linkOptions, f => f.Size == 0);
        executor.LinkTo(buffer, linkOptions);

        var versionBroadcaster = new BroadcastBlock<string>(null);
        for (var i = 0; i < 4; i++)
        {
            int copy = i;
            var extractor = new TransformManyBlock<string, TaskFile>(
                v => extract(v, copy));
            versionBroadcaster.LinkTo(extractor, linkOptions);
            extractor.LinkTo(buffer, linkOptions);
        }

        await versionBroadcaster.SendAsync("1.20.1");
        Console.ReadLine();
        Console.WriteLine("DONE");
    }

    private async IAsyncEnumerable<TaskFile> extract(string version, int i)
    {
        Console.WriteLine($"extractor {i}");
        for (int j = 0; j < 4; j++)
        {
            await Task.Delay(100);
            Console.WriteLine("extracted " + (i*10+j));
            yield return new TaskFile((i*10 + j).ToString())
            {
                Size = j
            };
        }
    }
}