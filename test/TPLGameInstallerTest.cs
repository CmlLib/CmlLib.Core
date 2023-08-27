using CmlLib.Core.FileExtractors;
using CmlLib.Core.Installers;
using CmlLib.Core.Rules;

namespace CmlLib.Core.Test;

public class TPLGameInstallerTest
{
    public async Task test()
    {
        var path = new MinecraftPath();
        var version = new DummyVersion();
        var rulesContext = new RulesEvaluatorContext(LauncherOSRule.Current);
        var fileProgress = new Progress<InstallerProgressChangedEventArgs>(e =>
        {
            //Console.WriteLine($"[{e.ProceedTasks}/{e.TotalTasks}][{e.EventType}] {e.Name}");
        });
        var byteProgress = new Progress<ByteProgress>(e =>
        {
            Console.WriteLine($"{e.ProgressedBytes} / {e.TotalBytes}");
        });
        var installer = new TPLGameInstaller(1);
        var extractors = new IFileExtractor[]
        {
            new DummyDownloaderExtractor("a", 1024, 1024 * 512)
        };
        await installer.Install(extractors, path, version, rulesContext, fileProgress, byteProgress, default);
    }
}