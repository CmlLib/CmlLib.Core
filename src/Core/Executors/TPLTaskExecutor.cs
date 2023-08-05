using System.Threading.Tasks.Dataflow;
using CmlLib.Core.FileExtractors;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;

namespace CmlLib.Core.Executors;

public class TPLTaskExecutor
{
    private readonly DataflowLinkOptions _linkOptions = new ()
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
        installer.Complete();
        await installer.Completion;
    }

    private BufferBlock<LinkedTask> createTaskExecutorBlock()
    {
        var buffer = new BufferBlock<LinkedTask>();
        var executor = new TransformBlock<LinkedTask, LinkedTask?>(async task =>
        {
            Console.WriteLine(task.GetType().Name);
            if (task is FileCheckTask fct)
            {
                Console.WriteLine(fct.Path);
                Console.WriteLine(fct.Hash);
            }
            else if (task is DownloadTask dt)
            {
                Console.WriteLine(dt.Path);
                Console.WriteLine(dt.Url);
            }
            return await task.Execute();
        }, new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = 8
        });

        buffer.LinkTo(executor, _linkOptions);
        executor.LinkTo(buffer!, _linkOptions, next => next != null);

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
}