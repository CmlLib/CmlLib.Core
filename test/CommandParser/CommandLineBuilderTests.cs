using CmlLib.Core.CommandParser;

namespace CmlLib.Core.Test.CommandParser;

public class CommandLineBuilderTests
{
    [Fact]
    public void add_command_line()
    {
        var builder = new CommandLineBuilder();
        builder.AddCommandLine("--key");
        builder.AddCommandLine("value");
        builder.AddCommandLine("-a=b");
        builder.AddCommandLine("-c d");
        builder.AddCommandLine("--key value1 value2 -x=y value3");
        Assert.Equal("--key value -a=b -c d --key value1 value2 -x=y value3", builder.Build());
    }

    [Fact]
    public void add_arguments()
    {
        var builder = new CommandLineBuilder();
        builder.AddArguments(["-key1", "value 1", "-key2=value 2", "default value"]);
        builder.AddArguments(["-key3"]);
        builder.AddArguments(["value 3"]);
        builder.AddArguments(["-key4", "\"value 4\""]);
        Assert.Equal("-key1 \"value 1\" \"-key2=value 2\" \"default value\" -key3 \"value 3\" -key4 \"\\\"value 4\\\"\"", builder.Build());
    }

    [Fact]
    public void check_key_is_contained()
    {
        var builder = new CommandLineBuilder();
        builder.AddArguments(["--key", "value"]);
        builder.AddKeyValueArgument(KeyValueArgument.Create("-a", "b"));
        builder.AddCommandLine("-c d");
        builder.AddCommandLine("--key \"value1 -key2\" value2 -x=y value3");

        Assert.True(builder.ContainsKey("--key"));
        Assert.True(builder.ContainsKey("-a"));
        Assert.True(builder.ContainsKey("-c"));
        Assert.True(builder.ContainsKey("-x"));
        Assert.False(builder.ContainsKey("value"));
        Assert.False(builder.ContainsKey("value1"));
        Assert.False(builder.ContainsKey("-key2"));
        Assert.False(builder.ContainsKey("value2"));
        Assert.False(builder.ContainsKey("value3"));
        Assert.False(builder.ContainsKey("b"));
        Assert.False(builder.ContainsKey("d"));
        Assert.False(builder.ContainsKey("y"));
    }

    [Fact]
    public void find_arguments_with_key()
    {
        var builder = new CommandLineBuilder();
        builder.AddCommandLine("--key1");
        builder.AddCommandLine("--key1=b");
        builder.AddCommandLine("--key1 c d");

        KeyValueArgument[] expected = 
        [
            KeyValueArgument.CreateWithoutValidation("--key1", null),
            KeyValueArgument.CreateWithoutValidation("--key1", "b"),
            KeyValueArgument.CreateWithoutValidation("--key1", "c"),
        ];
        Assert.Equal(expected, builder.Find("--key1"));
    }

    [Fact]
    public void find_arguments_with_no_matched_key()
    {
        var builder = new CommandLineBuilder();
        builder.AddCommandLine("--key1=value");

        Assert.Empty(builder.Find("--no-key"));
    }
}