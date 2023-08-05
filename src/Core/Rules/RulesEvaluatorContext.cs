using System.Runtime.InteropServices;

namespace CmlLib.Core.Rules;

public class RulesEvaluatorContext
{
    public RulesEvaluatorContext(LauncherOSRule os)
    {
        OS = os;
    }

    public LauncherOSRule OS { get; set; }
    public string[]? Features { get; set; }
}