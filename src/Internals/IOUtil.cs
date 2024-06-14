namespace CmlLib.Core.Internals;

internal static class IOUtil
{
    public static void CreateParentDirectory(string filePath)
    {
        var dirPath = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(dirPath))
            Directory.CreateDirectory(dirPath);
    }

    public static string NormalizePath(string path)
    {
        return Path.GetFullPath(path)
            .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
            .TrimEnd(Path.DirectorySeparatorChar);
    }

    public static string CombinePath(IEnumerable<string> paths, string pathSeparator)
    {
        return string.Join(pathSeparator, paths.Select(Path.GetFullPath));
    }

    public static bool CheckFileValidation(string path, string? compareHash)
    {
        if (!File.Exists(path))
            return false;
        
        var fileHash = IOUtil.ComputeFileSHA1(path);
        return fileHash == compareHash;
    }

    public static string ComputeFileSHA1(string path)
    {
#pragma warning disable CS0618
#pragma warning disable SYSLIB0021

        using var file = File.OpenRead(path);
        using var hasher = new System.Security.Cryptography.SHA1CryptoServiceProvider();

        var binaryHash = hasher.ComputeHash(file);
        return BitConverter.ToString(binaryHash).Replace("-", "").ToLowerInvariant();

#pragma warning restore SYSLIB0021
#pragma warning restore CS0618
    }
}
