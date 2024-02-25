using CmlLib.Core.Rules;

namespace CmlLib.Core.ProcessBuilder;

public class MArgument
{
    public MArgument()
    {

    }

    public MArgument(string arg)
    {
        Values = new string[] { arg };
    }

    public IReadOnlyCollection<string> Values { get; set; } = Array.Empty<string>();
    public IReadOnlyCollection<LauncherRule> Rules { get; set; } = Array.Empty<LauncherRule>();
}