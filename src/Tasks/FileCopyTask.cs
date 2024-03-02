using CmlLib.Core.Internals;
using CmlLib.Core.Files;

namespace CmlLib.Core.Tasks;

public class FileCopyTask : IUpdateTask
{
    public FileCopyTask(IEnumerable<string> destPaths) =>
        DestinationPaths = destPaths.ToArray();

    public string[] DestinationPaths { get; }

    public ValueTask Execute(GameFile file, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(file.Path) || !File.Exists(file.Path))
            throw new InvalidOperationException("The source file does not exists");

        var orgFile = new FileInfo(file.Path);
        foreach (var destination in DestinationPaths)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var desFile = new FileInfo(destination);
            if (!desFile.Exists || orgFile.Length != desFile.Length)
            {
                IOUtil.CreateParentDirectory(destination);
                orgFile.CopyTo(desFile.FullName, true);
            }
        }

        return new ValueTask();
    }
}