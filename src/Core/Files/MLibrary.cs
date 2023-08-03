using CmlLib.Core.Files;
using CmlLib.Core.Rules;

namespace CmlLib.Core.Version;

public record MLibrary
{
    public MLibrary(string name) => 
        Name = name;

    public MFileMetadata? Artifact { get; set; }
    public Dictionary<string, MFileMetadata>? Classifiers { get; set; }
    public Dictionary<string, string>? Natives { get; set; }
    public LauncherRule[]? Rules { get; set; }
    public string Name { get; set; }
    public bool IsServerRequired { get; set; } = true;
    public bool IsClientRequired { get; set; } = true;

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
            throw new ArgumentException();

        var classifierId = Natives?[os.Name]?.Replace("${arch}", os.Arch);
        return classifierId;
    }

    public MFileMetadata? GetNativeLibrary(LauncherOSRule os)
    {
        var classifierId = GetClassifierId(os);
        if (string.IsNullOrEmpty(classifierId) || Classifiers == null)
            return null;

        if (Classifiers.TryGetValue(classifierId, out var classifier))
            return classifier;
        else
            return null;
    }

    public string GetLibraryPath()
    {
        var path = Artifact?.Path;
        if (!string.IsNullOrEmpty(path))
            return path;
        
        return PackageName.Parse(Name).GetPath(null);
    }

    public string? GetNativeLibraryPath(LauncherOSRule os)
    {
        var classifier = GetNativeLibrary(os);
        if (!string.IsNullOrEmpty(classifier?.Path))
            return classifier.Path;

        var classifierId = GetClassifierId(os);
        if (string.IsNullOrEmpty(classifierId))
            return null;
        return PackageName.Parse(Name).GetPath(classifierId);
    }
}
