using CmlLib.Utils;

namespace CmlLib.Core.Tasks;

public class FileCheckTask : ResultTask
{
    public FileCheckTask(TaskFile file) : base(file)
    {
        if (string.IsNullOrEmpty(file.Path))
            throw new ArgumentException("file.Path was empty");
        this.Path = file.Path;

        if (string.IsNullOrEmpty(file.Hash))
            throw new ArgumentException("file.Hash return empty");
        this.Hash = file.Hash;
    }

    public FileCheckTask(string name, string path, string hash) : base(name)
    {
        this.Path = path;
        this.Hash = hash;
    }

    public string Path { get; }
    public string Hash { get; }

    protected override ValueTask<bool> OnExecutedWithResult()
    {
        var result = checkFile();
        return new ValueTask<bool>(result);
    }

    private bool checkFile()
    {
        if (!File.Exists(Path))
            return false;
        
        var fileHash = IOUtil.ComputeFileSHA1(Path);
        return fileHash == Hash;
    }
}