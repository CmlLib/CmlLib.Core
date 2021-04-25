using System.ComponentModel;

namespace CmlLib.Core.Downloader
{
    public class FileProgressChangedEventArgs : ProgressChangedEventArgs
    {
        public FileProgressChangedEventArgs(long total, long received, int percent) : base(percent, null)
        {
            this.TotalBytes = total;
            this.ReceivedBytes = received;
        }

        public long TotalBytes { get; private set; }
        public long ReceivedBytes { get; private set; }
    }
}
