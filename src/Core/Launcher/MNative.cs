using CmlLib.Core.Rules;
using CmlLib.Core.Version;
using CmlLib.Utils;

namespace CmlLib.Core;

public class MNative
{
    public MNative(
        MinecraftPath gamePath, 
        IVersion version,
        IRulesEvaluator rulesEvaluator,
        RulesEvaluatorContext context)
    {
        this.version = version;
        this.gamePath = gamePath;
        this.rulesEvaluator = rulesEvaluator;
        this.context = context;
    }

    private readonly IVersion version;
    private readonly MinecraftPath gamePath;
    private readonly IRulesEvaluator rulesEvaluator;
    private readonly RulesEvaluatorContext context;

    public string ExtractNatives()
    {
        var extractPath = gamePath.GetNativePath(version.Id);
        Directory.CreateDirectory(extractPath);

        var nativeLibraries = getNativeLibraryPaths();
        foreach (var libPath in nativeLibraries)
        {
            if (File.Exists(libPath))
                SharpZipWrapper.Unzip(libPath, extractPath, null);
        }

        return extractPath;
    }

    private IEnumerable<string> getNativeLibraryPaths()
    {
        return version
            .ConcatInheritedCollection(v => v.Libraries)
            .Where(lib => lib.CheckIsRequired("SIDE"))
            .Where(lib => lib.Rules == null || rulesEvaluator.Match(lib.Rules, context))
            .Select(lib => lib.GetNativeLibraryPath(context.OS))
            .Where(libPath => !string.IsNullOrEmpty(libPath))
            .Select(libPath => Path.Combine(gamePath.Library, libPath!));
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
