namespace CmlLib.Core.ProcessBuilder;

public class ProcessArgumentBuilder
{
    private List<string> _args = new();

    public void AddArgument(string arg)
    {
        AddRawArgument(escapeEmpty(arg));
    }

    public void AddKeyValue(string key, string? value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(key);

        var escapedValue = escapeEmpty(value);
        if (string.IsNullOrEmpty(escapedValue))
            escapedValue = "\"\"";

        AddRawArgument($"{key}={escapedValue}");
    }

    public void AddRawArgument(string arg)
    {
        _args.Add(arg);
    }

    private string escapeEmpty(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return "";
        if (input.Contains(" "))
            return "\"" + input + "\"";
        return input;
    }
}