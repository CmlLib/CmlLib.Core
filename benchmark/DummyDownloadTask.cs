using CmlLib.Core.Tasks;

namespace CmlLib.Core.Benchmarks;

public class DummyDownloadTask : DownloadTask
{
    public static int Seed = 0;

    public DummyDownloadTask(TaskFile file, HttpClient httpClient) : base(file, httpClient)
    {
    }

    protected override async ValueTask DownloadFile(
        IProgress<ByteProgressEventArgs>? progress,
        CancellationToken cancellationToken)
    {
        for (int i = 0; i < Size; i += 1)
        {
            if (Size % 512 == 0)
            {
                progress?.Report(new ByteProgressEventArgs
                {
                    TotalBytes = Size,
                    ProgressedBytes = i
                });
            }
            Seed += i;
            //await Task.Delay(1);
        }
    }
}