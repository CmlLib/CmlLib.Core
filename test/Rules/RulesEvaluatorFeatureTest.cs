using CmlLib.Core.Rules;
using NUnit.Framework;

namespace CmlLib.Core.Test.Rules;

public class RulesEvaluatorFeatureTest
{
    private LauncherOSRule TestOS = new LauncherOSRule
    {
        Name = "linux",
        Arch = "x86"
    };

    [Test]
    public void TestAllowEmptyFeature()
    {
        testFeatures(true, new string[] {}, new LauncherRule[]
        {
            new LauncherRule
            {
                Action = "allow",
                OS = TestOS,
                Features = new Dictionary<string, bool>()
            }
        });
    }

    [Test]
    [TestCase(true, "feature1", "feature1", true)]
    [TestCase(false, "feature1", "a", true)]
    [TestCase(false, "a", "feature1", true)]
    [TestCase(false, "feature1", "feature1", false)]
    [TestCase(true, "feature1", "a", false)]
    [TestCase(true, "a", "feature1", false)]
    public void TestOneFeature(
        bool expected,
        string inputFeature, 
        string ruleFeature, 
        bool ruleValue)
    {
        testFeatures(expected, new string[] { inputFeature }, new LauncherRule[]
        {
            new LauncherRule
            {
                Action = "allow",
                OS = TestOS,
                Features = new Dictionary<string, bool>
                {
                    [ruleFeature] = ruleValue
                }
            }
        });
    }

    [Test]
    [TestCase(true, "a", "b", "a", "b", true, true)]
    [TestCase(false, "a", "b", "a", "x", true, true)]
    [TestCase(false, "a", "x", "a", "b", true, true)]
    [TestCase(false, "a", "b", "a", "b", true, false)]
    [TestCase(true, "x", "b", "a", "b", false, true)]
    [TestCase(true, "a", "b", "x", "y", false, false)]
    public void TestTwoMatchFeature(
        bool expected,
        string inputFeature1, string inputFeature2,
        string ruleFeature1, string ruleFeature2,
        bool ruleValue1, bool ruleValue2)
    {
        testFeatures(expected, new string[] { inputFeature1, inputFeature2 }, new LauncherRule[]
        {
            new LauncherRule
            {
                Action = "allow",
                OS = TestOS,
                Features = new Dictionary<string, bool>
                {
                    [ruleFeature1] = ruleValue1,
                    [ruleFeature2] = ruleValue2
                }
            }
        });
    }

    [Test]
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

    private void testFeatures(bool expected, string[] features, IEnumerable<LauncherRule> rules)
    {
        var evaluator = new RulesEvaluator();
        var context = new RulesEvaluatorContext(TestOS);
        context.Features = features;
        var result = evaluator.Match(rules, context);
        Assert.That(result, Is.EqualTo(expected));
    }
}