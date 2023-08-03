using CmlLib.Core.Launcher;
using CmlLib.Core.Rules;

namespace CmlLib.Core.ProcessBuilder;

public class MinecraftArgumentBuilder
{
    private readonly IRulesEvaluator _evaluator;

    public IEnumerable<string> Build(IEnumerable<MArgument> arguments, Dictionary<string, string?> mapper)
    {
        foreach (var arg in arguments)
        {
            foreach (var value in Build(arg, mapper))
                yield return value;
        }
    }

    public IEnumerable<string> Build(MArgument arg, Dictionary<string, string?> mapper)
    {   
        if (arg.Values == null)
            yield break;

        if (arg.Rules != null)
        {
            var isMatch = _evaluator.Match(arg.Rules);
            if (!isMatch)
                yield break;
        }

        foreach (var value in arg.Values)
        {
            var mappedValue = Mapper.Interpolation(value, mapper, true);
            if (!string.IsNullOrEmpty(mappedValue))
                yield return mappedValue;
        }
    }
}