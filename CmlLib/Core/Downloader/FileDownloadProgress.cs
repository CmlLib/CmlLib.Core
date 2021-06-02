namespace CmlLib.Core.Downloader
{
    public class FileDownloadProgress
    {
        public FileDownloadProgress(DownloadFile file, long total, long progressed, long received, int percent)
        {
            this.File = file;
            this.TotalBytes = total;
            this.ProgressedBytes = progressed;
            this.ReceivedBytes = received;
            this.ProgressPercentage = percent;
        }

        public DownloadFile File { get; private set; }
        public long TotalBytes { get; private set; }
        public long ProgressedBytes { get; private set; }
        public long ReceivedBytes { get; private set; }
        public int ProgressPercentage { get; private set; }
    }
}
