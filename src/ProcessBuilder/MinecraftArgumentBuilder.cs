using CmlLib.Core.CommandParser;
using CmlLib.Core.Rules;

namespace CmlLib.Core.ProcessBuilder;

public class MinecraftArgumentBuilder
{
    public const int DefaultServerPort = 25565;

    private readonly CommandLineBuilder _builder;
    private readonly IReadOnlyDictionary<string, string?> _varDict;
    private readonly IRulesEvaluator _evaluator;
    private readonly RulesEvaluatorContext _context;

    public MinecraftArgumentBuilder(
        IRulesEvaluator evaluator,
        RulesEvaluatorContext context,
        IReadOnlyDictionary<string, string?> varDict)
    {
        _builder = new CommandLineBuilder();
        _varDict = varDict;
        _evaluator = evaluator;
        _context = context;
    }


    public void AddArguments(IEnumerable<string> args) =>
        _builder.AddArguments(getArguments(args, _varDict));

    public void AddArguments(IEnumerable<string> args, IReadOnlyDictionary<string, string?> varDict) =>
        _builder.AddArguments(getArguments(args, varDict));

    public void AddArguments(IEnumerable<MArgument> args) =>
        _builder.AddArguments(getArguments(args, _varDict));

    public void AddArguments(IEnumerable<MArgument> args, IReadOnlyDictionary<string, string?> varDict) =>
        _builder.AddArguments(getArguments(args, varDict));

    public bool ContainsKey(string key) => _builder.ContainsKey(key);
    public string Build() => _builder.Build();

    private IEnumerable<string> getArguments(IEnumerable<MArgument> args, IReadOnlyDictionary<string, string?> varDict)
    {
        return args
            .Where(arg => _evaluator.Match(arg.Rules, _context))
            .SelectMany(arg => arg.InterpolateValues(varDict));
    }

    private IEnumerable<string> getArguments(IEnumerable<string> args, IReadOnlyDictionary<string, string?> varDict)
    {
        return args.Select(arg => Mapper.InterpolateVariables(arg, varDict));
    }

    public bool ContainsXmx()
    {
        return _builder.Arguments.Any(arg => arg.Key.StartsWith("-Xmx"));
    }

    public bool ContainsXms()
    {
        return _builder.Arguments.Any(arg => arg.Key.StartsWith("-Xms"));
    }

    public void TryAddNativesDirectory()
    {
        if (!ContainsKey("-Djava.library.path"))
            AddArguments(["-Djava.library.path=${natives_directory}"]);
    }

    public void TryAddClassPath()
    {
        if (!ContainsKey("-cp"))
            AddArguments(["-cp", "${classpath}"]);
    }

    public void TryAddXmx(int xmx)
    {
        if (!ContainsXmx())
            AddArguments([$"-Xmx{xmx}m"]);
    }

    public void TryAddXms(int xms)
    {
        if (!ContainsXms())
            AddArguments([$"-Xms{xms}m"]);
    }

    public void TryAddDockName(string dockName)
    {
        if (!ContainsKey("-Xdock:name"))
            AddArguments(["-Xdock:name", dockName]);
    }

    public void TryAddDockIcon(string dockIcon)
    {
        if (!ContainsKey("-Xdock:icon"))
            AddArguments(["-Xdock:icon", dockIcon]);
    }

    public void SetDemo()
    {
        if (!ContainsKey("--demo"))
            AddArguments(["--demo"]);
    }

    public void TryAddScreenResolution(int width, int height)
    {
        if (!ContainsKey("--width"))
            AddArguments(["--width", width.ToString()]);
        if (!ContainsKey("--height"))
            AddArguments(["--height", height.ToString()]);
    }

    public void TryAddQuickPlayMultiplayer(string ip, int port)
    {
        if (!ContainsKey("--quickPlayMultiplayer"))
        {
            if (!ContainsKey("--server"))
                AddArguments(["--server", ip]);

            if (port != DefaultServerPort && !ContainsKey("--port"))
                AddArguments(["--port", port.ToString()]);
        }
    }

    public void SetFullscreen()
    {
        if (!ContainsKey("--fullscreen"))
            AddArguments(["--fullscreen"]);
    }
}