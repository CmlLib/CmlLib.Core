using NUnit.Framework;

namespace CmlLib.Core.Test;

public class PackageNameTest
{
    [TestCase("a:b:c", "a", "b", "c")]
    [TestCase("a.b:c.d:e.f", "a.b", "c.d", "e.f")]
    public void TestParse(string input, string package, string name, string version)
    {
        var packageName = PackageName.Parse(input);
        Assert.AreEqual(name, packageName.Name);
        Assert.AreEqual(package, packageName.Package);
        Assert.AreEqual(version, packageName.Version);
    }

    [Platform("Win")]
    [TestCase("de.oceanlabs.mcp:mcp_config:1.16.2-20200812.004259:mappings", 
        @"de\oceanlabs\mcp\mcp_config\1.16.2-20200812.004259\mcp_config-1.16.2-20200812.004259-mappings.jar")]
    [TestCase("net.java.dev.jna:platform:3.4.0", 
        @"net\java\dev\jna\platform\3.4.0\platform-3.4.0.jar")]
    public void TestGetPathWin(string input, string exp)
    {
        var packageName = PackageName.Parse(input);
        var result = packageName.GetPath(null, "jar");
        Assert.AreEqual(exp, result);
    }

    [Platform("Unix")]
    [TestCase("de.oceanlabs.mcp:mcp_config:1.16.2-20200812.004259:mappings", 
        @"de/oceanlabs/mcp/mcp_config/1.16.2-20200812.004259/mcp_config-1.16.2-20200812.004259.jar")]
    [TestCase("net.java.dev.jna:platform:3.4.0", 
        @"net/java/dev/jna/platform/3.4.0/platform-3.4.0.jar")]
    public void TestGetPathUnix(string input, string exp)
    {
        var packageName = PackageName.Parse(input);
        var result = packageName.GetPath(null, "jar");
        Assert.AreEqual(exp, result);
    }

    [Platform("Win")]
    [TestCase("com.mojang:text2speech:1.10.3", 
        @"com\mojang\text2speech\1.10.3\text2speech-1.10.3-natives-windows.jar")]
    public void TestGetPathWithNativeWin(string input, string exp)
    {
        var packageName = PackageName.Parse(input);
        var result = packageName.GetPath("natives-windows");
        Assert.AreEqual(exp, result);
    }

    [Platform("Unix")]
    [TestCase("ca.weblite:java-objc-bridge:1.0.0", 
        "ca/weblite/java-objc-bridge/1.0.0/java-objc-bridge-1.0.0-natives-osx.jar")]
    public void TestGetPathWithNativeUnix(string input, string exp)
    {
        var packageName = PackageName.Parse(input);
        var result = packageName.GetPath("natives-osx");
        Assert.AreEqual(exp, result);
    }
}