using ICSharpCode.SharpZipLib.Zip;

namespace CmlLib.Core.Internals;

internal static class SharpZipWrapper
{
    public static void Unzip(
        string zipPath, 
        string extractTo, 
        IReadOnlyCollection<string> excludes,
        CancellationToken cancellationToken = default)
    {
        using var fs = File.OpenRead(zipPath);
        using var s = new ZipInputStream(fs);

        ZipEntry e;
        while ((e = s.GetNextEntry()) != null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (excludes.Any(e.Name.StartsWith)) 
                continue;

            var fullPath = Path.Combine(extractTo, e.Name);
            if (e.IsFile)
            {
                IOUtil.CreateParentDirectory(fullPath);
                var fileName = Path.GetFileName(fullPath);

                try
                {
                    using var output = File.Create(fullPath);
                    s.CopyTo(output);
                }
                catch (IOException)
                {
                    // just skip
                }
                catch (UnauthorizedAccessException)
                {
                    // just skip 
                }
            }
            else
            {
                Directory.CreateDirectory(fullPath);
            }
        }
    }
}
