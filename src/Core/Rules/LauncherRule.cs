using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace CmlLib.Core.Rules;

public class LauncherRule
{
    [JsonPropertyName("action")]
    public string? Action { get; set; }

    [JsonPropertyName("features")]
    public Dictionary<string, bool>? Features { get; set; }

    [JsonPropertyName("os")]
    public LauncherOSRule? OS { get; set; }

    public bool MatchFeatures(Dictionary<string, bool> toMatch)
    {
        if (Features == null)
            return true;

        foreach (var kv in Features)
        {
            var exists = toMatch.ContainsKey(kv.Key);
            if (kv.Value) // feature: true
            {
                if (!exists)
                    return false;
                if (!toMatch[kv.Key])
                    return false;
            }
            else // feature: false
            {
                if (exists && toMatch[kv.Key])
                    return false;
            }
        }

        return true;
    }
}

public record LauncherOSRule
{
    public static readonly string Windows = "windows";
    public static readonly string OSX = "osx";
    public static readonly string Linux = "linux";

    public static LauncherOSRule CreateCurrent()
    {
        var os = new LauncherOSRule();
        if (Environment.Is64BitOperatingSystem)
            os.Arch = "64";
        else
            os.Arch = "32";
        os.Name = getOSName();
        return os;
    }

    private static string getOSName()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return OSX;
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return Windows;
        else
            return Linux;
    }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("arch")]
    public string? Arch { get; set; }

    public bool Match(LauncherOSRule toMatch)
    {
        return isPropMatch(Name, toMatch.Name) &&
               isPropMatch(Arch, toMatch.Arch);
    }

    private bool isPropMatch(string? rule, string? toMatch)
    {
        if (string.IsNullOrEmpty(rule))
            return true;
        return rule == toMatch;
    }
}