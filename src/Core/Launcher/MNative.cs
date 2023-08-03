using CmlLib.Core.Rules;
using CmlLib.Core.Version;
using CmlLib.Utils;

namespace CmlLib.Core;

public class MNative
{
    public MNative(MinecraftPath gamePath, IVersion version)
    {
        this.version = version;
        this.gamePath = gamePath;
    }

    private readonly IVersion version;
    private readonly MinecraftPath gamePath;
    private readonly IRulesEvaluator rulesEvaluator;

    public string ExtractNatives()
    {
        var extractPath = gamePath.GetNativePath(version.Id);
        Directory.CreateDirectory(extractPath);

        var nativeLibraries = getNativeLibraryPaths();
        foreach (var libPath in nativeLibraries)
        {
            if (File.Exists(libPath))
                new SharpZip(libPath).Unzip(extractPath);
        }

        return extractPath;
    }

    private IEnumerable<string> getNativeLibraryPaths()
    {
        return version
            .ConcatInheritedCollection(v => v.Libraries)
            .Where(lib => lib.CheckIsRequired("SIDE"))
            .Where(lib => lib.Rules == null || rulesEvaluator.Match(lib.Rules))
            .Select(lib => lib.GetNativeLibraryPath(os))
            .Where(libPath => !string.IsNullOrEmpty(libPath))
            .Select(libPath => Path.Combine(gamePath.Library, libPath));
    }

    public void CleanNatives()
    {

        try
        {
            string path = gamePath.GetNativePath(version.Id);
            DirectoryInfo di = new DirectoryInfo(path);

            if (!di.Exists)
                return;

            foreach (var item in di.GetFiles())
            {
                item.Delete();
            }
        }
        catch
        {
            // ignore exception
            // will be overwriten to new file
        }
    }
}
