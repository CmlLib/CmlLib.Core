using System.Text.Json;
using CmlLib.Core.Files;
using CmlLib.Core.Internals;

namespace CmlLib.Core.Version;

public static class JsonLibraryParser
{
    public static MLibrary? Parse(JsonElement element)
    {
        var name = element.GetPropertyValue("name");
        if (string.IsNullOrEmpty(name))
            return null;

        var library = new MLibrary(name);
        
        // rules
        if (element.TryGetProperty("rules", out var rulesProp))
            library.Rules = JsonRulesParser.Parse(rulesProp);

        // forge serverreq, clientreq
        library.IsServerRequired = element
            .GetPropertyOrNull("serverreq")?
            .GetBoolean() ?? 
            true; // default value is true
        
        library.IsClientRequired = element
            .GetPropertyOrNull("clientreq")?
            .GetBoolean() ?? 
            true; // default value is true
            
        // artifact
        var artifact = element.GetPropertyOrNull("artifact") ?? 
                       element.GetPropertyOrNull("downloads")?.GetPropertyOrNull("artifact") ??
                       element;
        library.Artifact = artifact.Deserialize<MFileMetadata>();

        // classifiers
        var classifiers = element.GetPropertyOrNull("classifies") ?? 
                          element.GetPropertyOrNull("downloads")?.GetPropertyOrNull("classifiers");
        if (classifiers.HasValue)
            library.Classifiers = classifiers.Value.Deserialize<Dictionary<string, MFileMetadata>>();

        // natives
        var natives = element.GetPropertyOrNull("natives");
        if (natives.HasValue)
        {
            library.Natives = natives.Value.Deserialize<Dictionary<string, string>>();
        }

        return library;
    }
}
