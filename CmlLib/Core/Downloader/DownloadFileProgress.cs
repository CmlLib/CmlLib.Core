namespace CmlLib.Core.Downloader;

public class DownloadFileProgress
{
    public DownloadFileProgress(DownloadFile file, long total, long progressed, long received, int percent)
    {
        File = file;
        TotalBytes = total;
        ProgressedBytes = progressed;
        ReceivedBytes = received;
        ProgressPercentage = percent;
    }

    public DownloadFile File { get; private set; }
    public long TotalBytes { get; private set; }
    public long ProgressedBytes { get; private set; }
    public long ReceivedBytes { get; private set; }
    public int ProgressPercentage { get; private set; }
}
