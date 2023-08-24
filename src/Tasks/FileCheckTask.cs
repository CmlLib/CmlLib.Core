using CmlLib.Core.Internals;

namespace CmlLib.Core.Tasks;

public class FileCheckTask : ResultTask
{
    public FileCheckTask(TaskFile file) : base(file)
    {
        if (string.IsNullOrEmpty(file.Path))
            throw new ArgumentException("file.Path was empty");
        this.Path = file.Path;
        this.Hash = file.Hash;
    }

    public FileCheckTask(string name, string path, string? hash) : base(name)
    {
        this.Path = path;
        this.Hash = hash;
    }

    public string Path { get; }
    public string? Hash { get; }

    protected override ValueTask<bool> OnExecutedWithResult(
        IProgress<ByteProgress>? progress,
        CancellationToken cancellationToken)
    {
        var result = checkFile();
        return new ValueTask<bool>(result);
    }

    private bool checkFile()
    {
        if (File.Exists(Path))
        {
            if (string.IsNullOrEmpty(Hash))
                return true;
            else
            {
                var realHash = IOUtil.ComputeFileSHA1(Path);
                return realHash == Hash;
            }
        }
        else
            return false;
    }
}