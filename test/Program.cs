#if TestSdk

using CmlLib.Core.Test.Installers;

var tester = new TPLGameInstallerTest();
int count = 0;
while (true)
{
    Console.WriteLine("try " + count);
    await tester.TestByteProgressReachesTo100();
    count++;
}

#endif