namespace CmlLib.Core.Tasks;

public struct LinkedTaskHead
{
    public LinkedTaskHead(LinkedTask? first, TaskFile file) =>
        (First, File) = (first, file);

    public string Name => File.Name;
    public LinkedTask? First { get; }
    public TaskFile File { get; }
}