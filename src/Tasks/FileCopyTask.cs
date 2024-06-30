using CmlLib.Core.Internals;
using CmlLib.Core.Files;

namespace CmlLib.Core.Tasks;

public class FileCopyTask : IUpdateTask
{
    public FileCopyTask(string dest) =>
        DestinationPath = dest;

    public string DestinationPath { get; }

    public ValueTask Execute(GameFile file, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(file.Path) || !File.Exists(file.Path))
            throw new InvalidOperationException("The source file does not exists");

        var orgFile = new FileInfo(file.Path);
        cancellationToken.ThrowIfCancellationRequested();

        var desFile = new FileInfo(DestinationPath);
        if (!desFile.Exists || orgFile.Length != desFile.Length)
        {
            IOUtil.CreateParentDirectory(DestinationPath);
            orgFile.CopyTo(desFile.FullName, true);
        }

        return new ValueTask();
    }
}