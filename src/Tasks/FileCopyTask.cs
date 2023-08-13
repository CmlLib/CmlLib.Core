using CmlLib.Core.Internals;

namespace CmlLib.Core.Tasks;

public class FileCopyTask : LinkedTask
{
    public FileCopyTask(string name, string sourcePath, IEnumerable<string> destPaths) : base(name) =>
        (SourcePath, DestinationPaths) = (sourcePath, destPaths.ToArray());

    public string SourcePath { get; }
    public string[] DestinationPaths { get; }

    protected override ValueTask<LinkedTask?> OnExecuted(
        IProgress<ByteProgressEventArgs>? progress,
        CancellationToken cancellationToken)
    {
        if (!File.Exists(SourcePath))
            throw new InvalidOperationException("The source file does not exists");

        var orgFile = new FileInfo(SourcePath);
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

        return new ValueTask<LinkedTask?>(NextTask);
    }
}