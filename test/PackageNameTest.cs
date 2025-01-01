namespace CmlLib.Core.Test;

public class PackageNameTest
{
    [Theory]
    [InlineData("a:b:c", "a", "b", "c")]
    [InlineData("a.b:c.d:e.f", "a.b", "c.d", "e.f")]
    public void TestParse(string input, string package, string name, string version)
    {
        var packageName = PackageName.Parse(input);
        Assert.Equal(name, packageName.Name);
        Assert.Equal(package, packageName.Package);
        Assert.Equal(version, packageName.Version);
    }

    [Theory]
    [InlineData(
        @"de.oceanlabs.mcp:mcp_config:1.16.2-20200812.004259:mappings", 
        @"de/oceanlabs/mcp/mcp_config/1.16.2-20200812.004259/mcp_config-1.16.2-20200812.004259-mappings.jar")]
    [InlineData(
        @"net.java.dev.jna:platform:3.4.0", 
        @"net/java/dev/jna/platform/3.4.0/platform-3.4.0.jar")]
    public void TestGetPathUnix(string input, string exp)
    {
        var packageName = PackageName.Parse(input);
        var result = packageName.GetPath(null, "jar", '/');
        Assert.Equal(exp, result);
    }

    [Theory]
    [InlineData(
        @"com.mojang:text2speech:1.10.3", 
        @"com\mojang\text2speech\1.10.3\text2speech-1.10.3-natives-windows.jar")]
    public void TestGetPathWithNativeWin(string input, string exp)
    {
        var packageName = PackageName.Parse(input);
        var result = packageName.GetPath("natives-windows", '\\');
        Assert.Equal(exp, result);
    }

    [Theory]
    [InlineData(
        @"ca.weblite:java-objc-bridge:1.0.0", 
        @"ca/weblite/java-objc-bridge/1.0.0/java-objc-bridge-1.0.0-natives-osx.jar")]
    public void TestGetPathWithNativeUnix(string input, string exp)
    {
        var packageName = PackageName.Parse(input);
        var result = packageName.GetPath("natives-osx", '/');
        Assert.Equal(exp, result);
    }
}