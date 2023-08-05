namespace CmlLib.Core.Tasks;

public struct TaskFile
{
    public string? Name { get; set; }
    public string? Path { get; set; }
    public string? Hash { get; set; }
    public string? Url { get; set; }
    public long Size { get; set; }
}