using CmlLib.Core.CommandParser;

namespace CmlLib.Core.Test.CommandParser;

public class KeyValueArgumentParserTests
{
    private static void assert((string, string?)[] expected, string[] input)
    {
        var result = Parser.ParseArgumentsToKeyValue(input);
        var expectedArgs = expected.Select(tuple => KeyValueArgument.CreateWithoutValidation(tuple.Item1, tuple.Item2));
        Assert.Equal(expectedArgs, result);
    }

    [Fact]
    public void parse_empty_args()
    {
        assert([], []);
    }

    [Fact]
    public void parse_empty_string()
    {
        assert([("", "")], [""]);
        assert([("", "   ")], ["   "]);
    }

    [Fact]
    public void parse_empty_value()
    {
        assert([("-key", "")], ["-key="]);
        assert([("-key", null)], ["-key"]);
    }

    [Fact]
    public void parse_single_key_value()
    {
        assert([("-key", "value")], ["-key=value"]);
        assert([("-key", "value and value")], ["-key=value and value"]);
        assert([("-key", "value -key")], ["-key=value -key"]);
        assert([("-key", "\\value \"\"")], ["-key=\\value \"\""]);
        assert([("-key", "value -key=value")], ["-key=value -key=value"]);
        assert([("-key", "value -key value")], ["-key=value -key value"]);
    }

    [Fact]
    public void parse_value_without_key()
    {
        assert([("", "value2")], ["value2"]);
        assert([("", "value1"), ("", "value 2")], ["value1", "value 2"]);

    }

    [Fact]
    public void parse_continous_key_values()
    {
        assert(
            [("-key", "value"), ("-key", "value"), ("", "value"), ("-key", null)], 
            ["-key", "value", "-key=value", "value", "-key"]);
    }
}