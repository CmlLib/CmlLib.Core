namespace CmlLib.Core.Rules;

public class RulesEvaluatorContext
{
    public RulesEvaluatorContext(LauncherOSRule os)
    {
        OS = os;
    }

    public LauncherOSRule OS { get; set; }
    public IEnumerable<string> Features { get; set; } = Enumerable.Empty<string>();
}