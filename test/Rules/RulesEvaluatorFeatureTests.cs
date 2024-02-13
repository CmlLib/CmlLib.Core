using CmlLib.Core.Rules;

namespace CmlLib.Core.Test.Rules;

public class RulesEvaluatorFeatureTest
{
    private LauncherOSRule TestOS = new LauncherOSRule
    {
        Name = "linux",
        Arch = "x86"
    };

    [Fact]
    public void allow_empty_feature()
    {
        allow(new string[] {}, new LauncherRule[]
        {
            new LauncherRule
            {
                Action = "allow",
                OS = TestOS,
                Features = new Dictionary<string, bool>()
            }
        });
    }

    [Fact]
    public void allow_one_feature()
    {
        allow(new string[] { "demo" }, new LauncherRule[]
        {
            new LauncherRule
            {
                Action = "allow",
                OS = TestOS,
                Features = new Dictionary<string, bool>
                {
                    ["demo"] = true
                }
            }
        });
    }

    [Fact]
    public void allow_not_ruled_feature()
    {
        allow(new string[] { "demo" }, new LauncherRule[]
        {
            new LauncherRule
            {
                Action = "allow",
                OS = TestOS,
                Features = new Dictionary<string, bool>
                {
                    ["fullscreen"] = false
                }
            }
        });
    }

    [Fact]
    public void disallow_no_feature()
    {
        disallow(new string[] { "demo" }, new LauncherRule[]
        {
            new LauncherRule
            {
                Action = "allow",
                OS = TestOS,
                Features = new Dictionary<string, bool>
                {
                    ["fullscreen"] = true
                }                
            }
        });
    }

    [Fact]
    public void disallow_disallowed_feature()
    {
        disallow(new string[] { "demo" }, new LauncherRule[]
        {
            new LauncherRule
            {
                Action = "allow",
                OS = TestOS,
                Features = new Dictionary<string, bool>
                {
                    ["demo"] = false
                }                
            }
        });
    }

    [Theory]
    [InlineData("demo", "cmllib")]
    public void allow_two_features(string input1, string input2)
    {
        allow(new string[] { input1, input2 }, new LauncherRule[]
        {
            new LauncherRule
            {
                Action = "allow",
                OS = TestOS,
                Features = new Dictionary<string, bool>
                {
                    ["demo"] = true,
                    ["fullscreen"] = false,
                    ["cmllib"] = true
                }
            }
        });
    }

    [Theory]
    [InlineData("demo", "not_included_feature")]
    public void allow_included_feature_and_not_included_featre(string input1, string input2)
    {
        allow(new string[] { input1, input2 }, new LauncherRule[]
        {
            new LauncherRule
            {
                Action = "allow",
                OS = TestOS,
                Features = new Dictionary<string, bool>
                {
                    ["demo"] = true,
                    ["fullscreen"] = false,
                    ["cmllib"] = false
                }
            }
        });
    }

    [Theory]
    [InlineData("demo", "fullscreen")]
    [InlineData("fullscreen", "cmllib")]
    public void disallow_two_features(string input1, string input2)
    {
        disallow(new string[] { input1, input2 }, new LauncherRule[]
        {
            new LauncherRule
            {
                Action = "allow",
                OS = TestOS,
                Features = new Dictionary<string, bool>
                {
                    ["demo"] = true,
                    ["fullscreen"] = false,
                    ["cmllib"] = true
                }
            }
        });
    }

    [Fact]
    public void TestDisallowMismatchOS()
    {
        var evaluator = new RulesEvaluator();
        var context = new RulesEvaluatorContext(new LauncherOSRule
        {
            Name = "windows", Arch = "x86"
        });
        context.Features = new string[] { "feature1" };
        var result = evaluator.Match(new LauncherRule[]
        {
            new LauncherRule
            {
                Action = "allow",
                OS = new LauncherOSRule
                {
                    Name = "linux", Arch = "x86"
                },
                Features = new Dictionary<string, bool>
                {
                    ["feature1"] = true,
                    ["feature2"] = false
                }
            }
        }, context);
        Assert.False(result);
    }

    private void allow(string[] features, IEnumerable<LauncherRule> rules)
    {
        Assert.True(testFeatures(features, rules));
    }

    private void disallow(string[] features, IEnumerable<LauncherRule> rules)
    {
        Assert.False(testFeatures(features, rules));
    }

    private bool testFeatures(string[] features, IEnumerable<LauncherRule> rules)
    {
        var evaluator = new RulesEvaluator();
        var context = new RulesEvaluatorContext(TestOS);
        context.Features = features;
        return evaluator.Match(rules, context);
    }
}