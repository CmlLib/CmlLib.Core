using CmlLib.Core.Rules;
using CmlLib.Core.Files;
using CmlLib.Core.Internals;

namespace CmlLib.Core.Tasks;

public class ChmodTask : IUpdateTask
{
    private readonly string? _filePath;

    public ChmodTask(int mode) => Mode = mode;
    public ChmodTask(int mode, string filePath) => 
        (Mode, _filePath) = (mode, filePath);

    public int Mode { get; }

    public ValueTask Execute(
        GameFile file,
        CancellationToken cancellationToken)
    {
        var target = _filePath ?? file.Path;
        if (string.IsNullOrEmpty(target))
            throw new InvalidOperationException("Target path was null");
        if (LauncherOSRule.Current.Name != LauncherOSRule.Windows)
            NativeMethods.Chmod(target, Mode);
        return new ValueTask();
    }
}