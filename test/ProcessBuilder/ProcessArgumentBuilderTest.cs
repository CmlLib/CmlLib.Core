using NUnit.Framework;
using CmlLib.Core.Rules;
using CmlLib.Core.ProcessBuilder;

namespace CmlLib.Core.Test.ProcessBuilder;

[TestFixture]
public class ProcessArgumentBuilderTest
{
    [TestCase(null)]
    [TestCase("")]
    public void TestAddEmptyValue(string test)
    {
        var builder = createBuilder();
        builder.Add(test);
        var actual = builder.Build();
        Assert.That(actual.Length, Is.Zero);
    }

    [TestCase("\"\"", "\"\"")]
    [TestCase("word", "word")]
    [TestCase("\"word\"", "\"word\"")]
    [TestCase("double word", "\"double word\"")]
    [TestCase("\"double word\"", "\"double word\"")]
    [TestCase("\"-k=v\"", "\"-k=v\"")]
    [TestCase("\"-k=double word\"", "\"-k=double word\"")]
    [TestCase(" - ", "-")]
    [TestCase("-k", "-k")]
    [TestCase("-k=", "-k=")]
    [TestCase("-k=\"\"", "-k=\"\"")]
    [TestCase("-k=v", "-k=v")]
    [TestCase("-k=\"v\"", "-k=\"v\"")]
    [TestCase("-k=\"double word\"", "-k=\"double word\"")]
    public void TestAddValue(string test, string expected)
    {
        var builder = createBuilder();
        builder.Add(test);
        var actual = builder.Build();
        Assert.That(actual, Is.EqualTo(new string[] { expected }));
    }

    [TestCase("-k", null, "-k")]
    [TestCase("-k", "", "-k=\"\"")]
    [TestCase("-k", "value", "-k=value")]
    [TestCase("-k", "\"value\"", "-k=\"value\"")]
    [TestCase("-k", "double word", "-k=\"double word\"")]
    [TestCase("-k", "\"double word\"", "-k=\"double word\"")]
    [TestCase("a", "b", "a=b")] // key not starts with '-'
    public void TestAddKeyValue(string key, string value, string expected)
    {
        var builder = createBuilder();
        builder.AddKeyValue(key, value);
        var actual = builder.Build();
        Assert.That(actual, Is.EqualTo(new string[] { expected }));
    }

    [TestCase("-key value")]
    [TestCase("-key=value and")]
    public void TestAddValueException(string test)
    {
        var builder = createBuilder();
        Assert.Throws<FormatException>(() =>
        {
            builder.Add(test);
        });
    }

    [Test, Combinatorial]
    public void TestAddKeyValueException(
        [Values(null, "", "-k -v", "-k and", "\"key\"")] string key, 
        [Values(null, "", "word", "double word")] string value)
    {
        var builder = createBuilder();
        Assert.Throws<FormatException>(() =>
        {
            builder.AddKeyValue(key, value);
        });
    }

    private ProcessArgumentBuilder createBuilder()
    {
        var evaluator = new RulesEvaluator();
        var context = new RulesEvaluatorContext(LauncherOSRule.Current);
        var builder = new ProcessArgumentBuilder(evaluator, context);
        return builder;
    }
}