using CmlLib.Core.Rules;

namespace CmlLib.Core.ProcessBuilder;

public class ProcessArgumentBuilder
{
    private readonly IRulesEvaluator _evaluator;
    private readonly RulesEvaluatorContext _context;
    private readonly HashSet<string> _keys = new();
    private readonly List<string> _args = new();

    public ProcessArgumentBuilder(IRulesEvaluator evaluator, RulesEvaluatorContext context)
    {
        _evaluator = evaluator;
        _context = context;
    }

    public bool CheckKeyAdded(string key) => _keys.Contains(key);

    public void AddRaw(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return;
        _args.Add(value);
    }

    public void AddRange(IEnumerable<string?> values)
    {
        foreach (var item in values)
            Add(item);
    }

    public void AddRange(params string?[] values)
    {
        foreach (var item in values)
            Add(item);
    }

    public void Add(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return;

        string result;
        var trimmed = value.Trim();
        if (trimmed.StartsWith("-"))
        {
            if (trimmed == "-")
            {
                result = "-";
            }
            else
            {
                var seperator = trimmed.IndexOf('=');
                if (seperator == -1) // does not contain '='
                {
                    if (trimmed.Contains(" "))
                        throw new FormatException();
                    result = trimmed;
                }
                else
                {
                    var k = trimmed.Substring(0, seperator);
                    if (seperator == trimmed.Length - 1) // last character was '='
                    {
                        if (trimmed.Contains(" "))
                            throw new FormatException();
                        result = trimmed;
                        addKey(k);
                    }
                    else // contains '='
                    {
                        var v = trimmed.Substring(seperator + 1);
                        if (!isEscaped(v) && v.Contains(" "))
                            throw new FormatException();
                        AddKeyValue(k, v);
                        return;
                    }
                }
            }
        }
        else if (isEscaped(value))
        {
            result = value;
        }
        else
        {
            result = escapeValue(value);
        }

        AddRaw(result);
    }

    public bool TryAddKeyValue(string key, string? value)
    {
        if (CheckKeyAdded(key))
            return false;
        else
        {
            AddKeyValue(key, value);
            return true;
        }
    }

    public void AddKeyValue(string key, string? value)
    {
        if (string.IsNullOrEmpty(key) ||
            isEscaped(key) ||
            key.Contains(" "))
            throw new FormatException();

        string result;
        if (value == null)
            result = key;
        else if (value == "")
            result = $"{key}=\"\"";
        else if (value == "\"\"")
            result = $"{key}=\"\"";
        else if (isEscaped(value))
            result = $"{key}={value}";
        else
            result = $"{key}={escapeValue(value)}";

        addKey(key);
        AddRaw(result);
    }

    private void addKey(string key)
    {
        if (key.StartsWith("-Xmx"))
            _keys.Add("-Xmx");
        else if (key.StartsWith("-Xms"))
            _keys.Add("-Xms");
        else
            _keys.Add(key);
    }

    public void AddArguments(IEnumerable<MArgument> arguments, Dictionary<string, string?> mapper)
    {
        foreach (var arg in arguments)
        {
            AddArgument(arg, mapper);
        }
    }

    public void AddArgument(MArgument arg, Dictionary<string, string?> mapper)
    {
        if (arg.Values == null)
            return;

        if (arg.Rules != null)
        {
            var isMatch = _evaluator.Match(arg.Rules, _context);
            if (!isMatch)
                return;
        }

        foreach (var value in arg.Values)
        {
            var mappedValue = Mapper.Interpolation(value, mapper);
            if (!string.IsNullOrEmpty(mappedValue))
                Add(mappedValue);
        }
    }

    public string[] Build()
    {
        return _args.ToArray();
    }

    private string escapeValue(string value)
    {
        if (value.Contains(' ') && !isEscaped(value))
        {
            return $"\"{value}\"";
        }
        else
        {
            return value;
        }
    }

    private bool isEscaped(string value)
    {
        return value.StartsWith("\"") && value.EndsWith("\"");
    }
}