using CmlLib.Core.Files;

namespace CmlLib.Core.Installers;

public class BasicGameInstaller : GameInstallerBase
{
    public BasicGameInstaller(HttpClient httpClient) : base(httpClient)
    {

    }

    protected override async ValueTask Install(
        IEnumerable<GameFile> gameFiles,
        CancellationToken cancellationToken)
    {
        long totalBytes = 0;
        long progressedBytes = 0;

        // queue files
        var queue = new HashSet<GameFile>(GameFilePathComparer.Default);
        foreach (var gameFile in gameFiles)
        {
            if (string.IsNullOrEmpty(gameFile.Url) || string.IsNullOrEmpty(gameFile.Path))
                continue;

            if (!queue.Add(gameFile))
                continue;
            
            if (IsExcludedPath(gameFile.Path))
                continue;

            totalBytes += gameFile.Size;
            FireFileProgress(queue.Count, 0, gameFile.Name, InstallerEventType.Queued);
        }

        // process files
        int progressed = 0;
        foreach (var gameFile in queue)
        {
            var progress = new ByteProgressDelta(initialSize: gameFile.Size, delta =>
            {
                totalBytes += delta.TotalBytes;
                progressedBytes += delta.ProgressedBytes;
                FireByteProgress(new ByteProgress(totalBytes, progressedBytes));
            });

            if (NeedUpdate(gameFile))
            {
                await Download(gameFile, progress, cancellationToken);
                await gameFile.ExecuteUpdateTask(cancellationToken);
            }
            else
            {
                await gameFile.ExecuteUpdateTask(cancellationToken);
                progress.ReportDone();
            }

            progressed++;
            FireFileProgress(queue.Count, progressed, gameFile.Name, InstallerEventType.Done);
        }
    }
}