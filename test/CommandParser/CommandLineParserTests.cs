using CmlLib.Core.CommandParser;

namespace CmlLib.Core.Test.CommandParser;

public class ArgumentParserTests
{
    [Fact]
    public void parse_empty()
    {
        Assert.Empty(Parser.ParseCommandLineToArguments(""));
        Assert.Empty(Parser.ParseCommandLineToArguments("  \n   "));
    }

    [Fact]
    public void parse_single_word()
    {
        Assert.Equal(["word"], Parser.ParseCommandLineToArguments("word"));
    }

    [Fact]
    public void parse_multiple_word()
    {
        Assert.Equal(["aa", "bb", "cc"], Parser.ParseCommandLineToArguments("aa bb cc"));
    }

    [Fact]
    public void ignore_spaces()
    {
        Assert.Equal(["a", "b", "c"], Parser.ParseCommandLineToArguments(" a \t\r b\n c "));
    }

    [Fact]
    public void escape_with_quotes()
    {
        Assert.Equal(
            ["-key=1 2 3", "word", "-1 + 3 = 2"], 
            Parser.ParseCommandLineToArguments("\"-key=1 2 3\" word \"-1 + 3 = 2\""));
    }

    [Fact]
    public void quotes_in_word()
    {
        Assert.Equal(
            ["-key=1 2 3"], 
            Parser.ParseCommandLineToArguments("-key=\"1 2 3\""));

        Assert.Equal(
            ["123456", "ab cd"], 
            Parser.ParseCommandLineToArguments("12\"34\"56 a\"b c\"d"));
    }

    [Fact]
    public void escape()
    {
        Assert.Equal(["\""], Parser.ParseCommandLineToArguments("\\\""));
        Assert.Equal(["hi\""], Parser.ParseCommandLineToArguments("hi\\\""));
        Assert.Equal(["-key=0\"0"], Parser.ParseCommandLineToArguments("-key=\"0\\\"0\""));
    }
}