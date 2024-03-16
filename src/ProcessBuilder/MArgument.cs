using CmlLib.Core.CommandParser;
using CmlLib.Core.Rules;

namespace CmlLib.Core.ProcessBuilder;

public class MArgument
{
    public static MArgument FromCommandLine(string cmd)
    {
        var args = Parser.ParseCommandLineToArguments(cmd).ToList();
        return new MArgument(args);
    }

    public MArgument()
    {

    }

    public MArgument(string arg)
    {
        Values = [arg];
    }

    public MArgument(IReadOnlyCollection<string> args)
    {
        Values = args;
    }

    public IReadOnlyCollection<string> Values { get; set; } = Array.Empty<string>();
    public IReadOnlyCollection<LauncherRule> Rules { get; set; } = Array.Empty<LauncherRule>();

    public IEnumerable<string> InterpolateValues(IReadOnlyDictionary<string, string?> varDict)
    {
        if (Values == null)
            yield break;

        foreach (var value in Values)
        {
            yield return Mapper.InterpolateVariables(value, varDict);
        }
    }
}