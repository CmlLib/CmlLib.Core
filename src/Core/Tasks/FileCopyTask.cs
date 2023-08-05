namespace CmlLib.Core.Tasks;

public class FileCopyTask : LinkedTask
{
    public FileCopyTask(string sourcePath, IEnumerable<string> destPaths) =>
        (SourcePath, DestinationPaths) = (sourcePath, destPaths.ToArray());

    public string SourcePath { get; }
    public string[] DestinationPaths { get; }

    public override ValueTask<LinkedTask?> Execute()
    {
        if (!File.Exists(SourcePath))
            throw new InvalidOperationException("The source file does not exists");

        foreach (var destination in DestinationPaths)
        {
            File.Copy(SourcePath, destination);
        }

        return new ValueTask<LinkedTask?>(NextTask);
    }
}