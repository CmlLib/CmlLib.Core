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

        // 4 <= maxDownloader <= 8
        var maxDownloader = Environment.ProcessorCount;
        maxDownloader = Math.Max(4, maxDownloader);
        maxDownloader = Math.Max(8, maxDownloader);

        return new ParallelGameInstaller(maxChecker, maxDownloader, 2048,  httpClient);
    }

    public ParallelGameInstaller(
        int maxChecker,
        int maxDownloader,
        int boundedCapacity,
        HttpClient httpClient) : base(httpClient)
    {
        if (maxChecker <= 0)
            throw new ArgumentException(nameof(maxChecker));
        if (maxDownloader <= 0)
            throw new ArgumentException(nameof(maxDownloader));

        MaxChecker = maxChecker;
        MaxDownloader = maxDownloader;
        BoundedCapacity = boundedCapacity;
    }

    public int MaxChecker { get; }
    public int MaxDownloader { get; }
    public int BoundedCapacity { get; }

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

        using var pipelineCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var (firstBlock, lastBlock) = buildBlock(pipelineCancellation.Token);
        var completion = Task.WhenAll(
            firstBlock.Completion,
            cancelPipelineOnFailure(lastBlock, pipelineCancellation));

        try
        {
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
                if (!await firstBlock.SendAsync(gameFile, pipelineCancellation.Token))
                    break;
            }
            firstBlock.Complete();

            while (!completion.IsCompleted)
            {
                aggregateAndReportByteProgress();
                await Task.WhenAny(Task.Delay(500), completion);
            }

            await completion;
            aggregateAndReportByteProgress();
        }
        catch (Exception exception)
        {
            pipelineCancellation.Cancel();
            try
            {
                await completion;
            }
            catch when (exception is not OperationCanceledException || cancellationToken.IsCancellationRequested)
            {
                // 정리 중 발생한 예외를 회수하되, 입력·진행률 오류나 호출자의 취소를 덮어쓰지 않는다.
            }

            throw;
        }
        finally
        {
            // 모든 작업이 종료된 후에만 진행률 저장소를 해제한다.
            progressStorage.Dispose();
            progressStorage = null;
        }
    }

    private async Task cancelPipelineOnFailure(
        IDataflowBlock lastBlock,
        CancellationTokenSource pipelineCancellation)
    {
        try
        {
            await lastBlock.Completion;
        }
        catch
        {
            // 다운로드 실패 시 큐 입력과 검사 블록도 중단해 제한된 큐에서 대기하지 않게 한다.
            pipelineCancellation.Cancel();
            throw;
        }
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
            BoundedCapacity = BoundedCapacity,
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
                await result.GameFile.ExecuteUpdateTask(cancellationToken);
                progress.ReportDone();
            }

            Interlocked.Increment(ref progressedFiles);
            FireFileProgress(totalFiles, progressedFiles, result.GameFile.Name, InstallerEventType.Done);
        },
        new ExecutionDataflowBlockOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = MaxDownloader,
            BoundedCapacity = BoundedCapacity,
            EnsureOrdered = false
        });

        var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
        checkBlock.LinkTo(downloadBlock, linkOptions);

        return (checkBlock, downloadBlock);
    }

    private void addProgressToStorage(long totalBytes, long progressedBytes)
    {
        progressStorage!.Value += new ByteProgress(totalBytes, progressedBytes);
    }

    private void aggregateAndReportByteProgress()
    {
        Debug.Assert(progressStorage != null);

        var aggregated = progressStorage.Values.Aggregate(
            new ByteProgress(), 
            (acc, progress) => acc + progress);
            
        FireByteProgress(aggregated);
    }
}
