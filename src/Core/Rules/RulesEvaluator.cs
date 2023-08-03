namespace CmlLib.Core.Rules;

public class RulesEvaluator : IRulesEvaluator
{
    private readonly RulesEvaluatorContext _context;

    public RulesEvaluator()
    {
    }

    public RulesEvaluator(RulesEvaluatorContext context) =>
        _context = context;

    public bool Match(IEnumerable<LauncherRule> rules)
    {
        return rules.Any(rule => match(rule));
    }

    private bool match(LauncherRule rule)
    {
        var isAllow = rule.Action == "allow";
        var isOSMatched = rule.OS != null && rule.OS.Match(_context.OS);

        if (isAllow)
            return isOSMatched;
        else
            return !isOSMatched;
    }
}