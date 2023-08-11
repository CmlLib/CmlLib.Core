using ICSharpCode.SharpZipLib.Zip;

namespace CmlLib.Core.Internals;

internal static class SharpZipWrapper
{
    public static void Unzip(string zipPath, string extractTo, IProgress<int>? progress)
    {
        using var fs = File.OpenRead(zipPath);
        using var s = new ZipInputStream(fs);

        long length = fs.Length;
        ZipEntry e;
        while ((e = s.GetNextEntry()) != null)
        {
            var fullPath = Path.Combine(extractTo, e.Name);
            IOUtil.CreateParentDirectory(fullPath);
            var fileName = Path.GetFileName(fullPath);

            using var output = File.Create(fullPath);
            s.CopyTo(output);

            int percent = (int)(s.Position / (double)length * 100);
            progress?.Report(percent);
        }
    }
}
