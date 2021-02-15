using System;

namespace CmlLib.Core.Downloader
{
    public class DownloadFile : IEquatable<DownloadFile>
    {
        public MFile Type { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Url { get; set; }

        public Action[] AfterDownload { get; set; }

        bool IEquatable<DownloadFile>.Equals(DownloadFile other)
        {
            if (other == null)
                return false;

            return Path == other.Path;
        }

        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }
    }
}
