namespace CmlLib.Core.Tasks;

public struct TaskFile
{
    public TaskFile(string name) => 
        Name = name;

    public string Name { get; }
    public string? Path { get; set; } = null;
    public string? Hash { get; set; } = null;
    public string? Url { get; set; } = null;
    public long Size { get; set; } = 0;
}