namespace CmlLib.Core.Downloader
{
    public class DownloadFileByteProgress
    {
        public DownloadFileByteProgress(DownloadFile file, long total, long progressed)
        {
            this.File = file;
            this.TotalBytes = total;
            this.ProgressedBytes = progressed;
        }

        public DownloadFile File { get; private set; }
        public long TotalBytes { get; private set; }
        public long ProgressedBytes { get; private set; }
    }
}
