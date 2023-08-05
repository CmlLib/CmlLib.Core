using CmlLib.Utils;

namespace CmlLib.Core.Tasks;

public class FileCheckTask : ResultTask
{
    public FileCheckTask(TaskFile file)
    {
        if (string.IsNullOrEmpty(file.Path))
            throw new ArgumentException("file.Path was empty");
        this.Path = file.Path;

        if (string.IsNullOrEmpty(file.Hash))
            throw new ArgumentException("file.Hash return empty");
        this.Hash = file.Hash;
    }

    public FileCheckTask(string path, string hash)
    {
        this.Path = path;
        this.Hash = hash;
    }

    public string Path { get; }
    public string Hash { get; }

    protected override ValueTask<bool> ExecuteWithResult()
    {
        var result = IOUtil.CheckFileValidation(Path, Hash, true);
        return new ValueTask<bool>(result);
    }
}