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
        IProgress<ByteProgress>? interceptedProgress = progress;
        if (this.Size <= 0)
        {
            interceptedProgress = new SyncProgress<ByteProgress>(e =>
            {
                this.Size = e.TotalBytes;
                progress?.Report(e);
            });
        }

        await HttpClientDownloadHelper.DownloadFileAsync(
            HttpClient,
            Url,
            Size,
            Path,
            interceptedProgress,
            cancellationToken);
    }
}