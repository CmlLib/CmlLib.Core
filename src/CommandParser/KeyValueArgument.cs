namespace CmlLib.Core.CommandParser;

public class KeyValueArgument
{
    public static KeyValueArgument Create(string key, string? value)
    {
        if (key != "" && !key.StartsWith("-"))
            throw new FormatException("key should start with key prefix (-)");
        return new KeyValueArgument(key, value);
    }

    public static KeyValueArgument CreateWithoutValidation(string key, string? value)
    {
        return new KeyValueArgument(key, value);
    }

    public static string Escape(string input)
    {
        input = input.Replace("\"", "\\\"");

        if (input == "")
        {
            return "\"\"";
        }
        else if (input.Any(c => char.IsWhiteSpace(c)))
        {
            return "\"" + input + "\"";
        }
        else
        {
            return input;
        }
    }

    private KeyValueArgument(string key, string? value)
    {
        Key = key;
        Value = value;
    }

    public string Key { get; } 
    public string? Value { get; }

    public override string ToString()
    {
        return ToString(true);
    }

    public string ToString(bool formatMode)
    {
        if (Key == "" && Value == null)
            return "";
        else if (Value == null)
            return Key;
        else if (Key == "")
            return Value;
        else
        {
            if (formatMode)
                return Key + "=" + Escape(Value);
            else
                return Key + " " + Escape(Value);
        }
    }

    public override int GetHashCode()
    {
        return Key.GetHashCode() ^ (Value ?? "").GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is KeyValueArgument arg)
        {
            return this.Key == arg.Key && this.Value == arg.Value;
        }
        else
        {
            return false;
        }
    }
}

public class KeyValueArgumentBuilder
{
    private readonly Queue<KeyValueArgument> _q = new();

    public string Key { get; set; } = "";
    public string? Value { get; set; }

    public void Clear()
    {
        Key = "";
        Value = null;
    }

    public void Complete()
    {   
        _q.Enqueue(KeyValueArgument.CreateWithoutValidation(Key, Value));
        Clear();
    }

    public IEnumerable<KeyValueArgument> PopArguments()
    {
        while (_q.Any())
        {
            yield return _q.Dequeue();
        }
    }
}