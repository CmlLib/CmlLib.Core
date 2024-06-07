using CmlLib.Core.Rules;

namespace CmlLib.Core.Test.Rules;

public class RulesEvaluatorOSTest
{
    [Theory]
    [InlineData(LauncherOSRule.Windows, LauncherOSRule.X86)]
    [InlineData(LauncherOSRule.Windows, LauncherOSRule.X64)]
    [InlineData(LauncherOSRule.Linux, LauncherOSRule.X86)]
    [InlineData(LauncherOSRule.Linux, LauncherOSRule.X64)]
    [InlineData(LauncherOSRule.OSX, LauncherOSRule.X86)]
    [InlineData(LauncherOSRule.OSX, LauncherOSRule.X64)]
    public void allow_empty(string osname, string arch)
    {
        var result = testOSRule(osname, arch, []);
        Assert.True(result);
    }    

    [Theory]
    [InlineData(LauncherOSRule.Windows, LauncherOSRule.X86)]
    [InlineData(LauncherOSRule.Windows, LauncherOSRule.X64)]
    [InlineData(LauncherOSRule.Linux, LauncherOSRule.X86)]
    [InlineData(LauncherOSRule.Linux, LauncherOSRule.X64)]
    [InlineData(LauncherOSRule.OSX, LauncherOSRule.X86)]
    [InlineData(LauncherOSRule.OSX, LauncherOSRule.X64)]
    public void allow_no_rule(string osname, string arch)
    {
        var result = testOSRule(osname, arch, new LauncherRule[]
        {
            new LauncherRule
            {
                Action = "allow"
            }
        });
        Assert.True(result);
    }

    [Theory]
    [InlineData(LauncherOSRule.Windows, LauncherOSRule.X86)]
    [InlineData(LauncherOSRule.Windows, LauncherOSRule.X64)]
    [InlineData(LauncherOSRule.Linux, LauncherOSRule.X86)]
    [InlineData(LauncherOSRule.Linux, LauncherOSRule.X64)]
    [InlineData(LauncherOSRule.OSX, LauncherOSRule.X86)]
    [InlineData(LauncherOSRule.OSX, LauncherOSRule.X64)]
    public void disallow_no_rule(string osname, string arch)
    {
        var result = testOSRule(osname, arch, new LauncherRule[]
        {
            new LauncherRule
            {
                Action = "disallow"
            }
        });
        Assert.False(result);
    }

    [Theory]
    [InlineData(LauncherOSRule.Linux, LauncherOSRule.X86)]
    [InlineData(LauncherOSRule.Linux, LauncherOSRule.X64)]
    public void allow_linux(string osname, string arch)
    {
        var result = testOSRule(osname, arch, new LauncherRule[]
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
        Assert.True(result);
    }

    [Theory]
    [InlineData(LauncherOSRule.Windows, LauncherOSRule.X86)]
    [InlineData(LauncherOSRule.Windows, LauncherOSRule.X64)]
    [InlineData(LauncherOSRule.OSX, LauncherOSRule.X86)]
    [InlineData(LauncherOSRule.OSX, LauncherOSRule.X64)]
    public void disallow_not_linux(string osname, string arch)
    {
        var result = testOSRule(osname, arch, new LauncherRule[]
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
        Assert.False(result);
    }

    [Theory]
    [InlineData(LauncherOSRule.Windows, LauncherOSRule.X86)]
    [InlineData(LauncherOSRule.Linux, LauncherOSRule.X86)]
    [InlineData(LauncherOSRule.OSX, LauncherOSRule.X86)]
    public void allow_x86(string osname, string arch)
    {
        var result = testOSRule(osname, arch, new LauncherRule[]
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
        Assert.True(result);
    }

    [Theory]
    [InlineData(LauncherOSRule.Windows, LauncherOSRule.X64)]
    [InlineData(LauncherOSRule.Linux, LauncherOSRule.X64)]
    [InlineData(LauncherOSRule.OSX, LauncherOSRule.Arm64)]
    public void disallow_not_x86(string osname, string arch)
    {
        var result = testOSRule(osname, arch, new LauncherRule[]
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
        Assert.False(result);
    }

    [Theory]
    [InlineData(LauncherOSRule.Windows, "10.0")]
    public void allow_windows_10_later(string osname, string version)
    {
        var os = new LauncherOSRule
        {
            Name = osname,
            Arch = LauncherOSRule.X86,
            Version = version
        };
        var result = testOSRule(os, new LauncherRule[]
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
        Assert.True(result);
    }

    [Theory]
    [InlineData(LauncherOSRule.Windows, "7")]
    [InlineData(LauncherOSRule.Linux, "10.0")]
    public void disallow_not_windows_10_later(string osname, string version)
    {
        var os = new LauncherOSRule
        {
            Name = osname,
            Arch = LauncherOSRule.X86,
            Version = version
        };
        var result = testOSRule(os, new LauncherRule[]
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
        Assert.False(result);
    }

    [Theory]
    [InlineData(LauncherOSRule.OSX, "10.5.0")]
    [InlineData(LauncherOSRule.OSX, "10.5.8")]
    public void allow_osx_10_5(string osname, string version)
    {
        var os = new LauncherOSRule
        {
            Name = osname,
            Arch = LauncherOSRule.X86,
            Version = version
        };
        var result = testOSRule(os, new LauncherRule[]
        {
            new LauncherRule
            {
                Action = "allow",
                OS = new LauncherOSRule
                {
                    Name = "osx",
                    Version = "^10\\.5\\.\\d$"
                }
            }
        });
        Assert.True(result);
    }

    [Theory]
    [InlineData(LauncherOSRule.OSX, "10.5")]
    [InlineData(LauncherOSRule.OSX, "10.6.5")]
    [InlineData(LauncherOSRule.Windows, "10.5.0")]
    public void disallow_not_osx_10_5(string osname, string version)
    {
        var os = new LauncherOSRule
        {
            Name = osname,
            Arch = LauncherOSRule.X86,
            Version = version
        };
        var result = testOSRule(os, new LauncherRule[]
        {
            new LauncherRule
            {
                Action = "allow",
                OS = new LauncherOSRule
                {
                    Name = "osx",
                    Version = "^10\\.5\\.\\d$"
                }
            }
        });
        Assert.False(result);
    }

    [Theory]
    [InlineData(LauncherOSRule.Windows, LauncherOSRule.X86)]
    [InlineData(LauncherOSRule.Windows, LauncherOSRule.X64)]
    [InlineData(LauncherOSRule.Linux, LauncherOSRule.X86)]
    [InlineData(LauncherOSRule.Linux, LauncherOSRule.X64)]
    public void allow_all_except_one(string osname, string arch)
    {
        var result = testOSRule(osname, arch, new LauncherRule[]
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
        Assert.True(result);
    }

    [Theory]
    [InlineData(LauncherOSRule.OSX, LauncherOSRule.X86)]
    [InlineData(LauncherOSRule.OSX, LauncherOSRule.X64)]
    public void disallow_only_one(string osname, string arch)
    {
        var result = testOSRule(osname, arch, new LauncherRule[]
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
        Assert.False(result);
    }

    [Theory]
    [InlineData(LauncherOSRule.Windows, LauncherOSRule.X86)]
    [InlineData(LauncherOSRule.Windows, LauncherOSRule.X64)]
    [InlineData(LauncherOSRule.Linux, LauncherOSRule.X86)]
    [InlineData(LauncherOSRule.Linux, LauncherOSRule.X64)]
    public void disallow_all_except_one(string osname, string arch)
    {
        var result = testOSRule(osname, arch, new LauncherRule[]
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
        Assert.False(result);
    }

    [Theory]
    [InlineData(LauncherOSRule.OSX, LauncherOSRule.X86)]
    [InlineData(LauncherOSRule.OSX, LauncherOSRule.X64)]
    public void allow_only_one(string osname, string arch)
    {
        var result = testOSRule(osname, arch, new LauncherRule[]
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
        Assert.True(result);
    }

    [Theory]
    [InlineData(LauncherOSRule.Windows, LauncherOSRule.X64)]
    [InlineData(LauncherOSRule.Linux, LauncherOSRule.X86)]
    public void allow_only_two(string osname, string arch)
    {
        var result = testOSRule(osname, arch, new LauncherRule[]
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
        Assert.True(result);
    }

    [Theory]
    [InlineData(LauncherOSRule.Windows, LauncherOSRule.X86)]
    [InlineData(LauncherOSRule.Linux, LauncherOSRule.X64)]
    [InlineData(LauncherOSRule.OSX, LauncherOSRule.X86)]
    [InlineData(LauncherOSRule.OSX, LauncherOSRule.X64)]
    public void disallow_all_except_two(string osname, string arch)
    {
        var result = testOSRule(osname, arch, new LauncherRule[]
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
        Assert.False(result);
    }

    private bool testOSRule(string osname, string arch, IEnumerable<LauncherRule> rules)
    {
        var os = new LauncherOSRule(osname, arch, "");
        return testOSRule(os, rules);
    }

    private bool testOSRule(LauncherOSRule os, IEnumerable<LauncherRule> rules)
    {
        var evaluator = new RulesEvaluator();
        var context = new RulesEvaluatorContext(os);
        return evaluator.Match(rules, context);
    }
}