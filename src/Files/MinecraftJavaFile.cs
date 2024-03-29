namespace CmlLib.Core.Files;

public class MinecraftJavaFile
{
    public MinecraftJavaFile(string name) => Name = name;

    public string Name { get; }
    public string? Type { get; set; }
    public bool Executable { get; set; }
    public string? Sha1 { get; set; }
    public long Size { get; set; }
    public string? Url { get; set; }
}
