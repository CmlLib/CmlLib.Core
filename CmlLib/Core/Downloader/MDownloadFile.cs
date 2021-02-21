using System;

namespace CmlLib.Core.Downloader
{
    public class DownloadFile : IEquatable<DownloadFile>
    {
        public DownloadFile(MFile type, string name, string path, string url)
        {
            Type = type;
            Name = name;
            Path = path;
            Url = url;
        }

        public MFile Type { get; private set; }
        public string Name { get; private set; }
        public string Path { get; private set; }
        public string Url { get; private set; }

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
