namespace CmlLib.Core.Files;

public class ModFile
{
    public ModFile(string filename, string url)
    {
        Name = filename;
        Path = System.IO.Path.Combine("mods", filename);
        Url = url;
    }

    public ModFile(string filename, string url, string hash) : this(filename, url)
    {
        Hash = hash;
    }

    public string? Name { get; set; }
    public string? Hash { get; set; }
    public string Path { get; set; }
    public string Url { get; set; }
}
