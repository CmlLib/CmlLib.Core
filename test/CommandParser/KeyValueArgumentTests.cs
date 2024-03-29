using CmlLib.Core.CommandParser;

namespace CmlLib.Core.Test.CommandParser;

public class KeyValueArgumentTests
{
    [Fact]
    public void disallow_key_without_prefix()
    {
        Assert.Throws<FormatException>(() =>
            KeyValueArgument.Create("abc", null));
    }

    [Fact]
    public void key_and_value_to_string()
    {
        Assert.Equal("-key=value", KeyValueArgument.Create("-key", "value").ToString());
    }

    [Fact]
    public void key_and_null_value_to_string()
    {
        Assert.Equal("-key", KeyValueArgument.Create("-key", null).ToString());
    }

    [Fact]
    public void key_and_empty_value_to_string()
    {
        Assert.Equal("-key=\"\"", KeyValueArgument.Create("-key", "").ToString(true));
        Assert.Equal("-key \"\"", KeyValueArgument.Create("-key", "").ToString(false));
    }

    [Fact]
    public void empty_key_and_value_to_string()
    {
        Assert.Equal("value", KeyValueArgument.Create("", "value").ToString());
    }

    [Fact]
    public void empty_key_and_empty_value_to_string()
    {
        Assert.Equal("", KeyValueArgument.Create("", null).ToString());
        Assert.Equal("", KeyValueArgument.Create("", "").ToString());
    }

    [Fact]
    public void escape_spaces()
    {
        Assert.Equal("-key=\"a b\"", KeyValueArgument.Create("-key", "a b").ToString(true));
        Assert.Equal("-key \"a b\"", KeyValueArgument.Create("-key", "a b").ToString(false));
    }

    [Fact]
    public void escape_quotes()
    {
        // (-key, " ") -> -key="\"\""
        Assert.Equal("-key=\"\\\" \\\"\"", KeyValueArgument.Create("-key", "\" \"").ToString(true));
        // (-key, " ") -> -key "\" \""
        Assert.Equal("-key \"\\\" \\\"\"", KeyValueArgument.Create("-key", "\" \"").ToString(false));
    }
}