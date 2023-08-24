using System.Text;
using System.Text.RegularExpressions;

namespace CmlLib.Core;

public static class Mapper
{
    private static readonly Regex argBracket = new Regex(@"\$?\{(.*?)}");

    public static string[] Map(string[] arg, Dictionary<string, string?> dicts, string prepath)
    {
        var checkPath = !string.IsNullOrEmpty(prepath);

        var args = new List<string>(arg.Length);
        foreach (string item in arg)
        {
            var a = Interpolation(item, dicts);
            if (checkPath)
                a = ToFullPath(a, prepath);
            args.Add(HandleEmptyArg(a, out _));
        }

        return args.ToArray();
    }

    public static string[] MapInterpolation(string[] arg, Dictionary<string, string?> dicts)
    {
        var args = new List<string>(arg.Length);
        foreach (string item in arg)
        {
            var a = Interpolation(item, dicts);
            if (!string.IsNullOrEmpty(a))
                args.Add(HandleEmptyArg(a, out _));
        }

        return args.ToArray();
    }

    public static string[] MapPathString(string[] arg, string prepath)
    {
        var args = new List<string>(arg.Length);
        foreach (string item in arg)
        {
            var a = ToFullPath(item, prepath);
            args.Add(HandleEmptyArg(a, out _));
        }

        return args.ToArray();
    }

    public static string Interpolation(string str, Dictionary<string, string?> dicts)
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

            return match.Value;
        });
    }

    public static string ToFullPath(string str, string prepath)
    {
        if (str.StartsWith("[") && str.EndsWith("]") && !string.IsNullOrEmpty(prepath))
        {
            var innerStr = str.TrimStart('[').TrimEnd(']').Split('@');
            var pathName = innerStr[0];
            var extension = "jar";

            if (innerStr.Length > 1)
                extension = innerStr[1];

            return Path.Combine(prepath,
                PackageName.Parse(pathName).GetPath(null, extension));
        }
        else if (str.StartsWith("\'") && str.EndsWith("\'"))
            return str.Trim('\'');
        else
            return str;
    }

    static string replaceByPos(string input, string replace, int startIndex, int length)
    {
        var sb = new StringBuilder(input);
        return replaceByPos(sb, replace, startIndex, length);
    }

    static string replaceByPos(StringBuilder sb, string replace, int startIndex, int length)
    {
        sb.Remove(startIndex, length);
        sb.Insert(startIndex, replace);
        return sb.ToString();
    }

    // key=value 1 => key="value 1"
    // key="va  l" => key="va  l"
    // va lue => "va lue"
    // "va lue" => "va lue"
    public static string HandleEmptyArg(string input, out string key)
    {
        var seperatorIndex = input.IndexOf('=');
        if (seperatorIndex != -1)
        {
            key = input.Substring(0, seperatorIndex);
            var value = input.Substring(seperatorIndex + 1);

            if ((key.Contains(" ") && !checkEmptyHandled(key)) || string.IsNullOrWhiteSpace(key))
                return key + "=\"" + value + "\"";
            else
                return input;
        }
        else if ((input.Contains(" ") && !checkEmptyHandled(input)) || string.IsNullOrWhiteSpace(input))
        {
            key = "";
            return "\"" + input + "\"";
        }
        else
        {
            key = "";
            return input;
        }
    }

    static bool checkEmptyHandled(string str)
    {
        return str.StartsWith("\"") || str.EndsWith("\"");
    }
}
