using System.Diagnostics;
using System.Threading.Tasks.Dataflow;
using CmlLib.Core.Tasks;

namespace CmlLib.Core.Installers;

public class ParallelGameInstaller : GameInstallerBase
{
    public static ParallelGameInstaller CreateAsCoreCount(HttpClient httpClient)
    {
        // 1 <= maxChecker <= 4
        var maxChecker = Environment.ProcessorCount;
        maxChecker = Math.Max(1, maxChecker);
        maxChecker = Math.Min(4, maxChecker);

        // 4 <= maxDownloader <= 6
        var maxDownloader = Environment.ProcessorCount;
        maxDownloader = Math.Max(4, maxDownloader);
        maxDownloader = Math.Max(6, maxDownloader);

        return new ParallelGameInstaller(maxChecker, maxDownloader, httpClient);
    }

    public ParallelGameInstaller(
        int maxChecker,
        int maxDownloader,
        HttpClient httpClient) : base(httpClient)
    {
        if (maxChecker <= 0)
            throw new ArgumentException(nameof(maxChecker));
        if (maxDownloader <= 0)
            throw new ArgumentException(nameof(maxDownloader));

        MaxChecker = maxChecker;
        MaxDownloader = maxDownloader;
    }

    public int MaxChecker { get; }
    public int MaxDownloader { get; }

    ThreadLocal<ByteProgress>? progressStorage;
    int totalFiles = 0;
    int progressedFiles = 0;
    long totalBytes = 0;

    protected override async ValueTask Install(
        IReadOnlyList<GameFile> gameFiles,
        CancellationToken cancellationToken)
    {
        totalFiles = gameFiles.Count;
        progressedFiles = 0;
        totalBytes = gameFiles.Select(f => f.Size).Sum();
        progressStorage = new ThreadLocal<ByteProgress>(
            () => new ByteProgress(), true);

        var (firstBlock, lastBlock) = buildBlock(cancellationToken);
        for (int i = 0; i < totalFiles; i++)
        {
            var file = gameFiles[i];
            FireFileProgress(totalFiles, progressedFiles, file.Name, InstallerEventType.Queued);
            await firstBlock.SendAsync(file, cancellationToken);
        }
        firstBlock.Complete();

        while (!lastBlock.Completion.IsCompleted)
        {
            aggregateAndReportByteProgress();
            await Task.WhenAny(Task.Delay(500), lastBlock.Completion);
        }
        aggregateAndReportByteProgress();

        progressStorage.Dispose();
        progressStorage = null;
    }

    private (ITargetBlock<GameFile>, IDataflowBlock) buildBlock(CancellationToken cancellationToken)
    {
        Debug.Assert(progressStorage != null);

        var checkBlock = new TransformBlock<GameFile, (GameFile, bool)>(gameFile =>
        {
            return (gameFile, NeedUpdate(gameFile));
        },
        new ExecutionDataflowBlockOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = MaxChecker
        });

        var buffer = new BufferBlock<(GameFile, bool)>();

        var downloadBlock = new ActionBlock<(GameFile GameFile, bool NeedUpdate)>(async result =>
        {
            var lastProgress = new ByteProgress
            {
                TotalBytes = result.GameFile.Size,
                ProgressedBytes = 0
            };

            var progressIntercepter = new SyncProgress<ByteProgress>(p =>
            {
                progressStorage.Value = new ByteProgress
                {
                    TotalBytes = progressStorage.Value.TotalBytes + (p.TotalBytes - lastProgress.TotalBytes),
                    ProgressedBytes = progressStorage.Value.ProgressedBytes + (p.ProgressedBytes - lastProgress.ProgressedBytes)
                };
                lastProgress = p;
            });

            if (result.NeedUpdate)
            {
                await Download(result.GameFile, progressIntercepter, cancellationToken);
                await result.GameFile.ExecuteUpdateTask(cancellationToken);
            }
            else
            {
                progressIntercepter.Report(new ByteProgress
                {
                    TotalBytes = result.GameFile.Size,
                    ProgressedBytes = result.GameFile.Size
                });
            }

            Interlocked.Increment(ref progressedFiles);
            FireFileProgress(totalFiles, progressedFiles, result.GameFile.Name, InstallerEventType.Done);
        },
        new ExecutionDataflowBlockOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = MaxDownloader
        });

        var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
        checkBlock.LinkTo(buffer, linkOptions);
        buffer.LinkTo(downloadBlock, linkOptions);

        return (checkBlock, downloadBlock);
    }

    private void aggregateAndReportByteProgress()
    {
        Debug.Assert(progressStorage != null);

        long aggregatedTotalBytes = totalBytes;
        long aggregatedProgressedBytes = 0;
        foreach (var progress in progressStorage.Values)
        {
            aggregatedTotalBytes += progress.TotalBytes;
            aggregatedProgressedBytes += progress.ProgressedBytes;
        }

        FireByteProgress(aggregatedTotalBytes, aggregatedProgressedBytes);
    }
}