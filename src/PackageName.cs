namespace CmlLib.Core;

// maven naming convention
// https://maven.apache.org/guides/mini/guide-naming-conventions.html

public class PackageName
{
    public static PackageName Parse(string name)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        var part = name.Split(':');
        if (part.Length < 3)
            throw new ArgumentException("invalid name");

        return new PackageName(part);
    }

    private PackageName(string[] names)
    {
        this.names = names;
    }

    private readonly string[] names;
    public string this[int index] => names[index];

    /// <summary>
    /// groupId
    /// </summary>
    public string Package => names[0];

    /// <summary>
    /// artifactId
    /// </summary>
    public string Name => names[1];

    /// <summary>
    /// version
    /// </summary>
    public string Version => names[2];

    public string GetPath(string? nativeId, char separator)
    {
        return GetPath(nativeId, "jar", separator);
    }

    public string GetPath(string? nativeId, string extension, char separator)
    {
        var filename = GetFilename(nativeId, extension);
        var directory = GetDirectory(separator);
        return $"{directory}{separator}{filename}";
    }

    public string GetFilename(string? nativeId, string extension)
    {
        var filename = string.Join("-", names.Skip(1));
        if (!string.IsNullOrEmpty(nativeId))
            filename += "-" + nativeId;
        filename += "." + extension;
        return filename;
    }

    public string GetDirectory(char separator)
    {
        var dir = Package.Replace('.', separator);
        return $"{dir}{separator}{Name}{separator}{Version}";
    }

    public string GetIdentifier()
    {
        return string.Join(":", names.Take(2).Concat(names.Skip(3)));
    }

    public override string ToString()
    {
        return string.Join(":", names);
    }

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }
}
