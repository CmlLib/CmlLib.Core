using CmlLib.Core.Internals;
using CmlLib.Core.Tasks;
using System.Diagnostics;

namespace CmlLib.Core.Installers;

public class BasicGameInstaller : IGameInstaller
{
    private readonly HttpClient _httpClient;

    public BasicGameInstaller(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async ValueTask Install(
        IEnumerable<GameFile> gameFiles, 
        IProgress<InstallerProgressChangedEventArgs>? fileProgress, 
        IProgress<ByteProgress>? byteProgress, 
        CancellationToken cancellationToken)
    {
        int totalTasks = gameFiles.Count();
        int progressedTasks = 0;
        long totalBytes = gameFiles.Select(f => f.Size).Sum();
        long progressedBytes = 0;

        foreach (var gameFile in gameFiles) 
        {
            fileProgress?.Report(new InstallerProgressChangedEventArgs
            {
                TotalTasks = totalTasks,
                ProgressedTasks = progressedTasks,
                Name = gameFile.Name
            });

            if (needUpdate(gameFile))
            {
                long lastTotal = gameFile.Size;
                long lastProgressed = 0;

                var progressIntercepter = new SyncProgress<ByteProgress>(p =>
                {
                    totalBytes += p.TotalBytes - lastTotal;
                    progressedBytes += p.ProgressedBytes - lastProgressed;
                    lastTotal = p.TotalBytes;
                    lastProgressed = p.ProgressedBytes;

                    byteProgress?.Report(new ByteProgress
                    {
                        TotalBytes = totalBytes,
                        ProgressedBytes = progressedBytes
                    });
                });

                await download(gameFile, progressIntercepter, cancellationToken);
                await gameFile.ExecuteUpdateTask(cancellationToken);
            }

            progressedTasks++;
        }

        fileProgress?.Report(new InstallerProgressChangedEventArgs
        {
            TotalTasks = totalTasks,
            ProgressedTasks = totalTasks,
            Name = gameFiles.LastOrDefault()?.Name
        });
    }

    private bool needUpdate(GameFile file)
    {
        if (string.IsNullOrEmpty(file.Path))
            return false;

        if (!File.Exists(file.Path))
            return true;

        if (string.IsNullOrEmpty(file.Hash))
            return false;

        var realHash = IOUtil.ComputeFileSHA1(file.Path);
        return realHash != file.Hash;
    }

    private async Task download(
        GameFile file,
        IProgress<ByteProgress>? progress,
        CancellationToken cancellationToken)
    {
        Debug.Assert(!string.IsNullOrEmpty(file.Path));
        if (string.IsNullOrEmpty(file.Url))
            return;

        for (int i = 3; i > 0; i--)
        {
            try
            {
                await HttpClientDownloadHelper.DownloadFileAsync(
                    _httpClient,
                    file.Url,
                    file.Size,
                    file.Path,
                    progress,
                    cancellationToken);
                break;
            }
            catch (Exception)
            {
                if (i == 1)
                    throw;
                await Task.Delay(3000);
            }
        }
    }
}