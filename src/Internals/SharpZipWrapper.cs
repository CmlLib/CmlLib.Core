using ICSharpCode.SharpZipLib.Zip;

namespace CmlLib.Core.Internals;

internal static class SharpZipWrapper
{
    public static void Unzip(
        string zipPath, 
        string extractTo, 
        IProgress<ByteProgress>? progress,
        CancellationToken cancellationToken = default)
    {
        using var fs = File.OpenRead(zipPath);
        using var s = new ZipInputStream(fs);

        long length = fs.Length;
        ZipEntry e;
        while ((e = s.GetNextEntry()) != null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var fullPath = Path.Combine(extractTo, e.Name);
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

            progress?.Report(new ByteProgress(length, s.Position));
        }
    }
}
