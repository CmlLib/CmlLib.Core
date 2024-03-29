using CmlLib.Core.Rules;
using CmlLib.Core.Version;
using CmlLib.Core.Internals;

namespace CmlLib.Core.Natives;

public class NativeLibraryExtractor : INativeLibraryExtractor
{
    public NativeLibraryExtractor(
        IRulesEvaluator rulesEvaluator)
    {
        this.rulesEvaluator = rulesEvaluator;
    }

    private readonly IRulesEvaluator rulesEvaluator;

    public string Extract(
        MinecraftPath path, 
        IVersion version,
        RulesEvaluatorContext rulesContext)
    {
        var extractPath = path.GetNativePath(version.Id);
        Directory.CreateDirectory(extractPath);

        var nativeLibraries = getNativeLibraryPaths(path, version, rulesContext);
        foreach (var libPath in nativeLibraries)
        {
            if (File.Exists(libPath))
                SharpZipWrapper.Unzip(libPath, extractPath, null);
        }

        return extractPath;
    }

    private IEnumerable<string> getNativeLibraryPaths(
        MinecraftPath path, 
        IVersion version,
        RulesEvaluatorContext rulesContext)
    {
        return version
            .ConcatInheritedCollection(v => v.Libraries)
            .Where(lib => lib.IsClientRequired)
            .Where(lib => lib.Rules == null || rulesEvaluator.Match(lib.Rules, rulesContext))
            .Select(lib => lib.GetNativeLibraryPath(rulesContext.OS))
            .Where(libPath => !string.IsNullOrEmpty(libPath))
            .Select(libPath => Path.Combine(path.Library, libPath!));
    }

    public void Clean(MinecraftPath path, IVersion version)
    {

        try
        {
            var nativePath = path.GetNativePath(version.Id);
            DirectoryInfo di = new DirectoryInfo(nativePath);

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