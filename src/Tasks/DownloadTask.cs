using CmlLib.Core.Downloader;

namespace CmlLib.Core.Tasks;

public class DownloadTask : LinkedTask
{
    public DownloadTask(TaskFile file) : base(file)
    {
        if (string.IsNullOrEmpty(file.Path))
            throw new ArgumentException("file.Path was empty");
        if (string.IsNullOrEmpty(file.Url))
            throw new ArgumentException("file.Url was empty");

        this.Path = file.Path;
        this.Url = file.Url;
    }

    public DownloadTask(string name, string path, string url) : base(name)
    {
        this.Path = path;
        this.Url = url;
    }

    public string Path { get; }
    public string Url { get; }

    protected override ValueTask<LinkedTask?> OnExecuted()
    {
        return new ValueTask<LinkedTask?>(NextTask);
    }
}