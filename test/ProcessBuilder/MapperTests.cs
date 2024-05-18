using CmlLib.Core.ProcessBuilder;

namespace CmlLib.Core.Test;

public class MapperTest
{
    private readonly static Dictionary<string, string?> TestVarDict = new()
    {
        { "word1", "value 1" },
        { "word 2", "value 2" },
        { "empty", "" },
        { "null", null }
    };

    [Theory]
    [InlineData("${word1}", "value 1")]
    [InlineData("insert${word1}here", "insertvalue 1here")]
    [InlineData("insert here ${word1} and ${word 2}...", "insert here value 1 and value 2...")]
    [InlineData("...${word1}${word 2}...", "...value 1value 2...")]
    public void interpolate_variables(string input, string expected)
    {
        var result = Mapper.InterpolateVariables(input, TestVarDict);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void interpolate_empty_value()
    {
        var result = Mapper.InterpolateVariables("insert${empty}here", TestVarDict);
        Assert.Equal("inserthere", result);
    }

    [Fact]
    public void interpolate_null_value()
    {
        var result = Mapper.InterpolateVariables("insert${null}here", TestVarDict);
        Assert.Equal("inserthere", result);
    }

    [Theory]
    [InlineData("${}", "${}")]
    [InlineData("insert${}here", "insert${}here")]
    public void interpolate_empty_variable(string input, string expected)
    {
        var result = Mapper.InterpolateVariables(input, TestVarDict);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("${not_found}", "${not_found}")]
    [InlineData("insert${not_found}here", "insert${not_found}here")]
    public void variable_not_found(string input, string expected)
    {
        var result = Mapper.InterpolateVariables(input, TestVarDict);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void bracket_inside_bracket()
    {
        var result = Mapper.InterpolateVariables("insert${insert_${word1}_here}here", TestVarDict);
        Assert.Equal("insert${insert_${word1}_here}here", result);
    }
}