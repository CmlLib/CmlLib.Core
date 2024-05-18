using System.Text.Json;
using CmlLib.Core.ProcessBuilder;
using CmlLib.Core.Internals;

namespace CmlLib.Core.Version;

public static class JsonArgumentParser
{
    public static IReadOnlyCollection<MArgument> Parse(JsonElement element)
    {
        var list = new List<MArgument>();
        foreach (var item in element.EnumerateArray())
        {
            var arg = ParseArgumentElement(item);
            if (arg != null)
                list.Add(arg);
        }
        return list.ToArray();
    }

    public static MArgument? ParseArgumentElement(JsonElement item)
    {
        if (item.ValueKind == JsonValueKind.Object)
        {
            return parseArgumentObject(item);
        }
        else
        {
            var value = item.GetString();
            if (string.IsNullOrEmpty(value))
                return null;

            return new MArgument
            {
                Values = new string[] { value }
            };
        }
    }

    private static MArgument? parseArgumentObject(JsonElement item)
    {
        var arg = new MArgument();

        var rules = item.GetPropertyOrNull("rules");
        if (rules == null || rules.Value.ValueKind != JsonValueKind.Array)
            rules = item.GetPropertyOrNull("compatibilityRules");
        if (rules.HasValue)
            arg.Rules = JsonRulesParser.Parse(rules.Value);

        var value = item.GetPropertyOrNull("value") ?? 
                    item.GetPropertyOrNull("values");
        if (value != null)
        {
            if (value.Value.ValueKind == JsonValueKind.Array)
            {
                arg.Values = value.Value.EnumerateArray()
                    .Where(item => item.ValueKind == JsonValueKind.String)
                    .Select(item => item.GetString())
                    .Where(item => !string.IsNullOrEmpty(item))
                    .ToArray()!;
            }
            else if (value.Value.ValueKind == JsonValueKind.String)
            {
                var valueString = value.Value.GetString();
                if (!string.IsNullOrEmpty(valueString))
                    arg.Values = new string[] { valueString };
            }
        }

        if (arg.Values == null || arg.Values.Count == 0)
            return null;
        return arg;
    }
}