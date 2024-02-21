using System;
using System.Threading.Tasks;

namespace CmlLib.Core.Downloader;

public class DownloadFile : IEquatable<DownloadFile>
{
    public DownloadFile(string path, string url)
    {
        Path = path;
        Url = url;
    }

    public MFile Type { get; set; }
    public string? Name { get; set; }
    public string Path { get; }
    public string Url { get; private set; }
    public long Size { get; set; }

    public Func<Task>[]? AfterDownload { get; set; }

    public bool Equals(DownloadFile? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Path == other.Path;
    }

    public override int GetHashCode()
    {
        return Path.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || (obj is DownloadFile other && Equals(other));
    }
}
