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

        var nativeLibraries = version
            .ConcatInheritedCollection(v => v.Libraries)
            .Where(lib => lib.IsClientRequired)
            .Where(lib => lib.Rules == null || rulesEvaluator.Match(lib.Rules, rulesContext));

        foreach (var nativeLibrary in nativeLibraries)
        {
            var libPath = nativeLibrary.GetNativeLibraryPath(rulesContext.OS);
            if (string.IsNullOrEmpty(libPath))
                continue;    
            
            SharpZipWrapper.Unzip(
                Path.Combine(path.Library, libPath), 
                extractPath, 
                nativeLibrary.ExtractExcludes, 
                default);
        }

        return extractPath;
    }

    public void Clean(MinecraftPath path, IVersion version)
    {
        try
        {
            var nativePath = path.GetNativePath(version.Id);
            DirectoryInfo di = new DirectoryInfo(nativePath);
            if (!di.Exists)
                return;
            di.Delete(true);
        }
        catch
        {
            // ignore exception
            // will be overwriten to new file
        }
    }
}