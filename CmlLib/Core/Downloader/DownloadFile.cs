using System;
using System.Threading.Tasks;

namespace CmlLib.Core.Downloader
{
    public class DownloadFile : IEquatable<DownloadFile>
    {
        public DownloadFile(string path, string url)
        {
            this.Path = path;
            this.Url = url;
        }
        
        public MFile Type { get; set; }
        public string? Name { get; set; }
        public string Path { get; private set; }
        public string Url { get; private set; }
        public long Size { get; set; }

        public Func<Task>[]? AfterDownload { get; set; }

        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }

        public bool Equals(DownloadFile? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Path == other.Path;
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is DownloadFile other && Equals(other);
        }
    }
}
