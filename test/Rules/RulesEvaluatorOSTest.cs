using CmlLib.Core.Rules;
using NUnit.Framework;

namespace CmlLib.Core.Test.Rules;

public class RulesEvaluatorOSTest
{
    [Test]
    [TestCase("windows", "x86", true)]
    [TestCase("windows", "x64", true)]
    [TestCase("linux", "x86", true)]
    [TestCase("linux", "x64", true)]
    [TestCase("osx", "x86", true)]
    [TestCase("osx", "x64", true)]
    public void TestAllow(string osname, string arch, bool expected)
    {
        testOSRule(osname, arch, expected, new LauncherRule[]
        {
            new LauncherRule
            {
                Action = "allow"
            }
        });
    }

    [Test]
    [TestCase("windows", "x86", false)]
    [TestCase("windows", "x64", false)]
    [TestCase("linux", "x86", false)]
    [TestCase("linux", "x64", false)]
    [TestCase("osx", "x86", false)]
    [TestCase("osx", "x64", false)]
    public void TestDisallow(string osname, string arch, bool expected)
    {
        testOSRule(osname, arch, expected, new LauncherRule[]
        {
            new LauncherRule
            {
                Action = "disallow"
            }
        });
    }

    [Test]
    [TestCase("windows", "x86", false)]
    [TestCase("windows", "x64", false)]
    [TestCase("linux", "x86", true)]
    [TestCase("linux", "x64", true)]
    [TestCase("osx", "x86", false)]
    [TestCase("osx", "x64", false)]
    public void TestAllowOSName(string osname, string arch, bool expected)
    {
        testOSRule(osname, arch, expected, new LauncherRule[]
        {
            new LauncherRule
            {
                Action = "allow",
                OS = new LauncherOSRule
                {
                    Name = "linux"
                }
            }
        });
    }

    [Test]
    [TestCase("windows", "x86", true)]
    [TestCase("windows", "x64", false)]
    [TestCase("linux", "x86", true)]
    [TestCase("linux", "x64", false)]
    [TestCase("osx", "x86", true)]
    [TestCase("osx", "x64", false)]
    public void TestAllowOSArch(string osname, string arch, bool expected)
    {
        testOSRule(osname, arch, expected, new LauncherRule[]
        {
            new LauncherRule
            {
                Action = "allow",
                OS = new LauncherOSRule
                {
                    Arch = "x86"
                }
            }
        });
    }


    [Test]
    [TestCase("windows", "10.0", true)]
    [TestCase("windows", "7", false)]
    [TestCase("linux", "10.0", false)]
    public void TestAllowOSVersion(string osname, string version, bool expected)
    {
        var os = new LauncherOSRule
        {
            Name = osname,
            Arch = "x86",
            Version = version
        };
        testOSRule(os, expected, new LauncherRule[]
        {
            new LauncherRule
            {
                Action = "allow",
                OS = new LauncherOSRule
                {
                    Name = "windows",
                    Version = "^10\\."
                }
            }
        });
    }

    [Test]    
    [TestCase("windows", "x86", true)]
    [TestCase("windows", "x64", true)]
    [TestCase("linux", "x86", true)]
    [TestCase("linux", "x64", true)]
    [TestCase("osx", "x86", false)]
    [TestCase("osx", "x64", false)]
    public void TestAllowCompositedOSName(string osname, string arch, bool expected)
    {
        testOSRule(osname, arch, expected, new LauncherRule[]
        {
            new LauncherRule
            {
                Action = "allow"
            },
            new LauncherRule
            {
                Action = "disallow",
                OS = new LauncherOSRule
                {
                    Name = "osx"
                }
            }
        });
    }

    [Test]
    [TestCase("windows", "x86", false)]
    [TestCase("windows", "x64", false)]
    [TestCase("linux", "x86", false)]
    [TestCase("linux", "x64", false)]
    [TestCase("osx", "x86", true)]
    [TestCase("osx", "x64", true)]
    public void TestDisallowCompositedOSName(string osname, string arch, bool expected)
    {
        testOSRule(osname, arch, expected, new LauncherRule[]
        {
            new LauncherRule
            {
                Action = "disallow"
            },
            new LauncherRule
            {
                Action = "allow",
                OS = new LauncherOSRule
                {
                    Name = "osx"
                }
            }
        });
    }

    [Test]
    [TestCase("windows", "x86", false)]
    [TestCase("windows", "x64", true)]
    [TestCase("linux", "x86", true)]
    [TestCase("linux", "x64", false)]
    [TestCase("osx", "x86", false)]
    [TestCase("osx", "x64", false)]
    public void TestCompositedAllows(string osname, string arch, bool expected)
    {
        testOSRule(osname, arch, expected, new LauncherRule[]
        {
            new LauncherRule
            {
                Action = "allow", 
                OS = new LauncherOSRule
                {
                    Name = "windows",
                    Arch = "x64"
                }
            },
            new LauncherRule
            {
                Action = "allow",
                OS = new LauncherOSRule
                {
                    Name = "linux",
                    Arch = "x86"
                }
            }
        });
    }

    [Test]
    public void TestDisallowMismatchFeatures()
    {
        var os = new LauncherOSRule
        {
            Name = "linux", Arch = "x86"
        };
        var context = new RulesEvaluatorContext(os);
        context.Features = new string[] { "feature1" };
        var evaluator = new RulesEvaluator();
        var result = evaluator.Match(new LauncherRule[]
        {
            new LauncherRule
            {
                Action = "allow",
                OS = os,
                Features = new Dictionary<string, bool>
                {
                    ["feature1"] = false
                }
            }
        }, context);
        Assert.False(result);
    }

    private void testOSRule(string osname, string arch, bool expected, IEnumerable<LauncherRule> rules)
    {
        var os = new LauncherOSRule(osname, arch);
        testOSRule(os, expected, rules);
    }

    private void testOSRule(LauncherOSRule os, bool expected, IEnumerable<LauncherRule> rules)
    {
        var evaluator = new RulesEvaluator();
        var context = new RulesEvaluatorContext(os);
        var result = evaluator.Match(rules, context);
        Assert.That(result, Is.EqualTo(expected));
    }
}