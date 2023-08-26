using CmlLib.Core.Internals;
using NUnit.Framework;

namespace CmlLib.Core.Test;

public class IOUtilTest
{
    [Platform("Win")]
    [TestCase(@"C:\\a/", @"C:\a")]
    [TestCase(@"C:\\a\/b.txt", @"C:\a\b.txt")]
    [TestCase(@"C:\/a\\b/\c.txt", @"C:\a\b\c.txt")]
    [TestCase(@"/root/f1\.txt", @"C:\root\f1\.txt")]
    public void TestNormalizePathWin(string path, string expected)
    {
        var normalizedPath = IOUtil.NormalizePath(path);
        Assert.AreEqual(expected, normalizedPath);
    }

    [Platform("Unix")]
    [TestCase(@"/root/f1\.txt", @"/root/f1\.txt")]
    [TestCase(@"/root/path1/", @"/root/path1")]
    public void TestNormalizePathUnix(string path, string expected)
    {
        var normalizedPath = IOUtil.NormalizePath(path);
        Assert.AreEqual(expected, normalizedPath);
    }

    [Platform("Win")]
    [Test]
    public void TestCombinePathWin()
    {
        var paths = new[]
        {
            @"C:\test\lib1.zip", @"C:\test\lib space 2.zip"
        };
        var exp = @"C:\test\lib1.zip;""C:\test\lib space 2.zip""";
        
        Assert.AreEqual(exp, IOUtil.CombinePath(paths));
    }

    [Platform("Unix")]
    [Test]
    public void TestCombinePathUnix()
    {
        var paths = new[]
        {
            @"/root/f1.zip", @"/root/f2 space sss.txt"
        };
        var exp = @"/root/f1.zip:""/root/f2 space sss.txt""";
        
        Assert.AreEqual(exp, IOUtil.CombinePath(paths));
    }
}