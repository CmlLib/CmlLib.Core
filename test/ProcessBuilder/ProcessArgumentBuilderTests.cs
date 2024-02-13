using CmlLib.Core.ProcessBuilder;

namespace CmlLib.Core.Test.ProcessBuilder;

public class ProcessArgumentBuilderTests
{
    [Fact]
    public void ignore_null_or_empty()
    {
        // Given
        var sut = createBuilder();

        // When
        sut.Add(null);
        sut.Add("");

        // Then
        var actual = sut.Build();
        Assert.Empty(actual);
    }

    [Theory]
    // value without space
    [InlineData("word", "word")]
    // escape spaced value
    [InlineData("double word", "\"double word\"")]
    // do not escape already escaped value
    [InlineData("\"word\"", "\"word\"")]
    [InlineData("\"\"", "\"\"")]
    [InlineData("\"double word\"", "\"double word\"")]
    // thread as value (not key-value)
    [InlineData("\"-k=v\"", "\"-k=v\"")]
    [InlineData("\"-k=double word\"", "\"-k=double word\"")]
    [InlineData("-k", "-k")]
    // special
    [InlineData(" - ", "-")]
    public void add_value(string input, string expected)
    {
        // Given
        var sut = createBuilder();

        // When
        sut.Add(input);

        // Then
        Assert.Empty(sut.Keys);
        Assert.Equal(new []{expected}, sut.Build());
    }

    [Theory]
    // key-value
    [InlineData("-k=v", "-k=v", "-k")]
    [InlineData("-k=", "-k=", "-k")]
    // key-value, with space in value
    [InlineData("-k=\"\"", "-k=\"\"", "-k")]
    [InlineData("-k=\"v\"", "-k=\"v\"", "-k")]
    [InlineData("-k=\"double word\"", "-k=\"double word\"", "-k")]
    public void add_key_value_by_Add(string input, string expectedArg, string expectedKey)
    {
        // Given
        var sut = createBuilder();

        // When
        sut.Add(input);

        // Then
        Assert.Equal(new []{expectedKey}, sut.Keys);
        Assert.Equal(new []{expectedArg}, sut.Build());
    }

    [Theory]
    // null value
    [InlineData("-k", null, "-k")]
    // empty value
    [InlineData("-k", "", "-k=\"\"")]
    // value
    [InlineData("-k", "value", "-k=value")]
    // value with space
    [InlineData("-k", "double word", "-k=\"double word\"")]
    // do not escape already escaped value
    [InlineData("-k", "\"value\"", "-k=\"value\"")]
    [InlineData("-k", "\"double word\"", "-k=\"double word\"")]
    // string that don't start with '-' also thread as key
    [InlineData("a", "b", "a=b")]
    public void add_key_value_by_AddKeyValue(string key, string value, string expectedArg)
    {
        // Given
        var sut = createBuilder();

        // When
        sut.AddKeyValue(key, value);

        // Then
        Assert.Equal(new []{key}, sut.Keys);
        Assert.Equal(new []{expectedArg}, sut.Build());
    }

    [Theory]
    // cannot add key-value with a space in the value
    [InlineData("-key value")]
    [InlineData("-key=value and")]
    public void add_invalid_value(string test)
    {
        var sut = createBuilder();
        Assert.Throws<FormatException>(() =>
        {
            sut.Add(test);
        });
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("a b")]
    [InlineData("\"\"")]
    [InlineData("\"a b\"")]
    public void add_invalid_key(string key)
    {
        var builder = createBuilder();
        Assert.Throws<FormatException>(() =>
        {
            builder.AddKeyValue(key, "value");
        });
    }

    private ProcessArgumentBuilder createBuilder()
    {
        var builder = new ProcessArgumentBuilder();
        return builder;
    }
}