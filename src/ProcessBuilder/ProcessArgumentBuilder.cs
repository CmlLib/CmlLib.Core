namespace CmlLib.Core.ProcessBuilder;

public class ProcessArgumentBuilder
{
    private readonly HashSet<string> _keys = new();
    private readonly List<string> _args = new();

    public IEnumerable<string> Keys => _keys;
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
                        _keys.Add(k);
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

        _keys.Add(key);
        AddRaw(result);
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