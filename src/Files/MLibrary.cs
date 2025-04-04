using CmlLib.Core.Files;
using CmlLib.Core.Rules;

namespace CmlLib.Core.Version;

public record MLibrary
{
    public MLibrary(string name) => 
        Name = name;

    public MFileMetadata? Artifact { get; init; }
    public IReadOnlyDictionary<string, MFileMetadata>? Classifiers { get; init; }
    public IReadOnlyDictionary<string, string>? Natives { get; init; }
    public IReadOnlyCollection<LauncherRule> Rules { get; init; } = [];
    public IReadOnlyCollection<string> ExtractExcludes { get; init; } = [];
    public string Name { get; }
    public bool IsServerRequired { get; init; } = true;
    public bool IsClientRequired { get; init; } = true;

    public bool CheckIsRequired(string side)
    {
        if (side == JsonVersionParserOptions.ClientSide)
            return IsClientRequired;
        else if (side == JsonVersionParserOptions.ServerSide)
            return IsServerRequired;
        else
            return true;
    }

    public string? GetClassifierId(LauncherOSRule os)
    {
        if (string.IsNullOrEmpty(os.Name) || string.IsNullOrEmpty(os.Arch))
            throw new ArgumentException("Invalid LauncherOSRule: empty Name or Arch");

        if (Natives == null || !Natives.TryGetValue(os.Name, out var native) || string.IsNullOrEmpty(native))
            return null;

        return native.Replace("${arch}", os.Arch);
    }

    public MFileMetadata? GetNativeLibrary(LauncherOSRule os)
    {
        if (Classifiers == null)
            return null;

        var classifierId = GetClassifierId(os);
        MFileMetadata? classifier;
        if (!string.IsNullOrEmpty(classifierId) && Classifiers.TryGetValue(classifierId, out classifier))
            return classifier;
        else if (!string.IsNullOrEmpty(os.Name) && Classifiers.TryGetValue(os.Name, out classifier))
            return classifier;
        else
            return null;
    }

    public string GetLibraryPath()
    {
        var path = Artifact?.Path;
        if (!string.IsNullOrEmpty(path))
            return path;
        
        return PackageName.Parse(Name).GetPath(null, Path.DirectorySeparatorChar);
    }

    public string? GetNativeLibraryPath(LauncherOSRule os)
    {
        var classifier = GetNativeLibrary(os);
        if (!string.IsNullOrEmpty(classifier?.Path))
            return classifier.Path;

        var classifierId = GetClassifierId(os);
        if (string.IsNullOrEmpty(classifierId))
            return null;
        return PackageName.Parse(Name).GetPath(classifierId, Path.DirectorySeparatorChar);
    }
}
