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

            if (NeedUpdate(gameFile))
            {
                long lastTotal = gameFile.Size;
                long lastProgressed = 0;
                var progressIntercepter = new SyncProgress<ByteProgress>(p =>
                {
                    totalBytes += p.TotalBytes - lastTotal;
                    progressedBytes += p.ProgressedBytes - lastProgressed;
                    lastTotal = p.TotalBytes;
                    lastProgressed = p.ProgressedBytes;

                    FireByteProgress(totalBytes, progressedBytes);
                });

                await Download(gameFile, progressIntercepter, cancellationToken);
                await gameFile.ExecuteUpdateTask(cancellationToken);
            }

            FireFileProgress(gameFiles.Count, i + 1, gameFile.Name, InstallerEventType.Done);
        }
    }
}