namespace CmlLib.Core.Rules;

public class RulesEvaluator : IRulesEvaluator
{
    public bool Match(IEnumerable<LauncherRule> rules, RulesEvaluatorContext context)
    {
        return rules.Any(rule => match(rule, context));
    }

    private bool match(LauncherRule rule, RulesEvaluatorContext context)
    {
        var isAllow = rule.Action == "allow";
        var isOSMatched = rule.OS != null && rule.OS.Match(context.OS);

        if (isAllow)
            return isOSMatched;
        else
            return !isOSMatched;
    }
}