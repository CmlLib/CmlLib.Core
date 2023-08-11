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