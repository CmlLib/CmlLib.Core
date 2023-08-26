using NUnit.Framework;

namespace CmlLib.Core.Test;

public class MapperTest
{
    [Platform("win")]
    [TestCase(
        @"[de.oceanlabs.mcp:mcp_config:1.16.2-20200812.004259@zip]", 
        @"C:\libraries\de\oceanlabs\mcp\mcp_config\1.16.2-20200812.004259\mcp_config-1.16.2-20200812.004259.zip")]
    [TestCase(
        @"[net.minecraft:client:1.16.2-20200812.004259:slim]",
        @"C:\libraries\net\minecraft\client\1.16.2-20200812.004259\client-1.16.2-20200812.004259-slim.jar")]
    public void TestFullPath(string input, string exp)
    {
        Assert.AreEqual(exp, Mapper.ToFullPath(input, @"C:\libraries\"));
    }

    [TestCase("", "")]
    [TestCase("key=value", "key=value")]
    [TestCase("key=value 1", "key=\"value 1\"")]
    [TestCase("key=\"value 2\"", "key=\"value 2\"")]
    [TestCase("value 3", "\"value 3\"")]
    [TestCase("\"value 4\"", "\"value 4\"")]
    public void TestHandleEmptyArg(string input, string exp)
    {
        //Assert.AreEqual(exp, Mapper.HandleEmptyArg(input));
    }
}