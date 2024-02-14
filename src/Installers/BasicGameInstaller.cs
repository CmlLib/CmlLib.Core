using CmlLib.Core.Tasks;

namespace CmlLib.Core.Installers;

public class BasicGameInstaller : GameInstallerBase
{
    public BasicGameInstaller(HttpClient httpClient) : base(httpClient)
    {
        
    }

    protected override async ValueTask Install(
        IReadOnlyList<GameFile> gameFiles,
        CancellationToken cancellationToken)
    {
        long totalBytes = gameFiles.Select(f => f.Size).Sum();
        long progressedBytes = 0;

        for (int i = 0; i < gameFiles.Count; i++)
        {
            var gameFile = gameFiles[i];
            FireFileProgress(gameFiles.Count, i, gameFile.Name, InstallerEventType.Queued);

            var progress = new ByteProgressDelta(initialSize: gameFile.Size, delta =>
            {
                totalBytes += delta.TotalBytes;
                progressedBytes += delta.ProgressedBytes;
                FireByteProgress(totalBytes, progressedBytes);
            });

            if (NeedUpdate(gameFile))
            {
                await Download(gameFile, progress, cancellationToken);
                await gameFile.ExecuteUpdateTask(cancellationToken);
            }
            else
            {
                progress.ReportDone();
            }

            FireFileProgress(gameFiles.Count, i + 1, gameFile.Name, InstallerEventType.Done);
        }
    }
}