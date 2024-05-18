using System.Text.RegularExpressions;

namespace CmlLib.Core.ProcessBuilder;

public static class Mapper
{
    private static readonly Regex argBracket = new Regex(@"\$?\{(.*?)}");

    public static string InterpolateVariables(string str, IReadOnlyDictionary<string, string?> dicts)
    {
        return argBracket.Replace(str, match =>
        {
            if (match.Groups.Count < 2)
                return match.Value;

            var key = match.Groups[1].Value;
            if (dicts.TryGetValue(key, out string? value))
            {
                return value ?? "";
            }
            else
            {
                return match.Value;
            }
        });
    }
}
