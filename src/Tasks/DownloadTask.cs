using CmlLib.Core.Installers;

namespace CmlLib.Core.Tasks;

public class DownloadTask : LinkedTask
{
    public DownloadTask(TaskFile file, HttpClient httpClient) : base(file)
    {
        if (string.IsNullOrEmpty(file.Path))
            throw new ArgumentException("file.Path was empty");
        if (string.IsNullOrEmpty(file.Url))
            throw new ArgumentException("file.Url was empty");

        HttpClient = httpClient;
        this.Path = file.Path;
        this.Url = file.Url;
        this.Size = file.Size;
    }

    protected HttpClient HttpClient;
    public string Path { get; }
    public string Url { get; }

    protected async override ValueTask<LinkedTask?> OnExecuted(
        IProgress<ByteProgress>? progress,
        CancellationToken cancellationToken)
    {
        await DownloadFile(progress, cancellationToken);
        return NextTask;
    }

    protected async virtual ValueTask DownloadFile(
        IProgress<ByteProgress>? progress,
        CancellationToken cancellationToken)
    {
        var interceptedProgress = new SyncProgress<ByteProgress>(e =>
        {
            this.Size = e.TotalBytes;
            progress?.Report(e);
        });

        for (int i = 3; i > 0; i--)
        {
            try
            {
                await HttpClientDownloadHelper.DownloadFileAsync(
                    HttpClient,
                    Url,
                    Size,
                    Path,
                    interceptedProgress,
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