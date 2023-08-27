#if TestSdk

namespace CmlLib.Core.Test;

internal class Program
{
    public static async Task Main()
    {
        var tester = new TPLGameInstallerTest();
        await tester.test();
    }
}

#endif