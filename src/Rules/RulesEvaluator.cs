using System.Text.RegularExpressions;

namespace CmlLib.Core.Rules;

public class RulesEvaluator : IRulesEvaluator
{
    public bool Match(IEnumerable<LauncherRule> rules, RulesEvaluatorContext context)
    {
        var finalResult = false;
        var isAny = false;
        foreach (var rule in rules)
        {
            isAny = true;
            var isAllow = rule.Action == "allow";
            var isOSMatch = matchOS(rule, context);
            var isFeatureMatch = matchFeature(rule, context);
            
            if (isOSMatch && isFeatureMatch)
                finalResult = isAllow;
        }
        return isAny ? finalResult : true;
    }

    private bool matchOS(LauncherRule rule, RulesEvaluatorContext context)
    {
        bool isNameMatched = true;
        if (!string.IsNullOrEmpty(rule.OS?.Name))
            isNameMatched = rule.OS?.Name == context.OS?.Name;

        bool isArchMatched = true;
        if (!string.IsNullOrEmpty(rule.OS?.Arch))
            isArchMatched = rule.OS?.Arch == getArchForRule(context.OS?.Arch);

        bool isVersionMatched = true;
        if (string.IsNullOrEmpty(context.OS?.Version))
            isVersionMatched = true;
        else if (!string.IsNullOrEmpty(rule.OS?.Version))
            isVersionMatched = Regex.IsMatch(
                context.OS.Version, 
                rule.OS.Version, 
                RegexOptions.None, 
                TimeSpan.FromMilliseconds(100));

        return isNameMatched && isArchMatched && isVersionMatched;
    }

    private string? getArchForRule(string? arch)
    {
        if (arch == LauncherOSRule.X86)
            return "x86";
        else if (arch == LauncherOSRule.X64)
            return "x64";
        else
            return arch;
    }

    private bool matchFeature(LauncherRule rule, RulesEvaluatorContext context)
    {
        if (rule.Features == null)
            return true;

        foreach (var kv in rule.Features)
        {
            var isFeatured = context.Features?.Contains(kv.Key) ?? false;
            if (isFeatured != kv.Value)
                return false;
        }

        return true;
    }
}