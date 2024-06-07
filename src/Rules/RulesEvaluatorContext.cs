namespace CmlLib.Core.Rules;

public record RulesEvaluatorContext
{
    public RulesEvaluatorContext(LauncherOSRule os)
    {
        OS = os;
    }

    public RulesEvaluatorContext(LauncherOSRule os, IEnumerable<string> features)
    {
        OS = os;
        Features = features;
    }

    public LauncherOSRule OS { get; init; }
    public IEnumerable<string> Features { get; init; } = Enumerable.Empty<string>();
}