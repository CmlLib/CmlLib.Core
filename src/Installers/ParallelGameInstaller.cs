using System.Diagnostics;
using System.Threading.Tasks.Dataflow;
using CmlLib.Core.Files;

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

    protected override async ValueTask Install(
        IEnumerable<GameFile> gameFiles,
        CancellationToken cancellationToken)
    {
        progressStorage = new ThreadLocal<ByteProgress>(
            () => new ByteProgress(), true);
        totalFiles = 0;
        progressedFiles = 0;

        var (firstBlock, lastBlock) = buildBlock(cancellationToken);

        var queue = new HashSet<GameFile>(GameFilePathComparer.Default);
        foreach (var gameFile in gameFiles)
        {
            if (string.IsNullOrEmpty(gameFile.Url) || string.IsNullOrEmpty(gameFile.Path))
                continue;

            if (!queue.Add(gameFile))
                continue;

            if (IsExcludedPath(gameFile.Path))
                continue;

            addProgressToStorage(gameFile.Size, 0);
            Interlocked.Increment(ref totalFiles);

            FireFileProgress(totalFiles, progressedFiles, gameFile.Name, InstallerEventType.Queued);
            await firstBlock.SendAsync(gameFile, cancellationToken);
        }
        firstBlock.Complete();

        while (!lastBlock.Completion.IsCompleted)
        {
            aggregateAndReportByteProgress();
            await Task.WhenAny(Task.Delay(500), lastBlock.Completion);
        }

        await lastBlock.Completion; // throw exception if exists
        aggregateAndReportByteProgress(); // report 100%

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
            MaxDegreeOfParallelism = MaxChecker,
            EnsureOrdered = false
        });
        var downloadBlock = new ActionBlock<(GameFile GameFile, bool NeedUpdate)>(async result =>
        {
            var progress = new ByteProgressDelta(initialSize: result.GameFile.Size, delta =>
                addProgressToStorage(delta.TotalBytes, delta.ProgressedBytes));

            if (result.NeedUpdate)
            {
                await Download(result.GameFile, progress, cancellationToken);
                await result.GameFile.ExecuteUpdateTask(cancellationToken);
            }
            else
            {
                progress.ReportDone();
            }

            Interlocked.Increment(ref progressedFiles);
            FireFileProgress(totalFiles, progressedFiles, result.GameFile.Name, InstallerEventType.Done);
        },
        new ExecutionDataflowBlockOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = MaxDownloader,
            BoundedCapacity = 1000,
            EnsureOrdered = false
        });

        var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
        checkBlock.LinkTo(downloadBlock, linkOptions);

        return (checkBlock, downloadBlock);
    }

    private void addProgressToStorage(long totalBytes, long progressedBytes)
    {
        var storedProgress = progressStorage!.Value;
        progressStorage.Value = new ByteProgress
        {
            TotalBytes = storedProgress.TotalBytes + totalBytes,
            ProgressedBytes = storedProgress.ProgressedBytes + progressedBytes
        };
    }

    private void aggregateAndReportByteProgress()
    {
        Debug.Assert(progressStorage != null);

        long aggregatedTotalBytes = 0;
        long aggregatedProgressedBytes = 0;
        foreach (var progress in progressStorage.Values)
        {
            aggregatedTotalBytes += progress.TotalBytes;
            aggregatedProgressedBytes += progress.ProgressedBytes;
        }

        FireByteProgress(aggregatedTotalBytes, aggregatedProgressedBytes);
    }
}