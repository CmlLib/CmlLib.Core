using CmlLib.Core.Internals;

namespace CmlLib.Core.Test;

public class IOUtilTest
{
    [Theory(Skip = "Win")]
    [InlineData(@"C:\\a/", @"C:\a")]
    [InlineData(@"C:\\a\/b.txt", @"C:\a\b.txt")]
    [InlineData(@"C:\/a\\b/\c.txt", @"C:\a\b\c.txt")]
    [InlineData(@"/root/f1\.txt", @"C:\root\f1\.txt")]
    public void TestNormalizePathWin(string path, string expected)
    {
        var normalizedPath = IOUtil.NormalizePath(path);
        Assert.Equal(expected, normalizedPath);
    }

    [Theory(Skip = "Unix")]
    [InlineData(@"/root/f1\.txt", @"/root/f1\.txt")]
    [InlineData(@"/root/path1/", @"/root/path1")]
    public void TestNormalizePathUnix(string path, string expected)
    {
        var normalizedPath = IOUtil.NormalizePath(path);
        Assert.Equal(expected, normalizedPath);
    }

    [Fact(Skip = "Win")]
    public void TestCombinePathWin()
    {
        var paths = new[]
        {
            @"C:\test\lib1.zip", @"C:\test\lib space 2.zip"
        };
        var exp = @"C:\test\lib1.zip;""C:\test\lib space 2.zip""";
        
        Assert.Equal(exp, IOUtil.CombinePath(paths, Path.PathSeparator.ToString()));
    }

    [Fact(Skip = "Unix")]
    public void TestCombinePathUnix()
    {
        var paths = new[]
        {
            @"/root/f1.zip", @"/root/f2 space sss.txt"
        };
        var exp = @"/root/f1.zip:""/root/f2 space sss.txt""";
        
        Assert.Equal(exp, IOUtil.CombinePath(paths, Path.PathSeparator.ToString()));
    }
}