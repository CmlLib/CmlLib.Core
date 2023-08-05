namespace CmlLib.Core.Rules;

public interface IRulesEvaluator
{
    bool Match(IEnumerable<LauncherRule> rules, RulesEvaluatorContext context);
}