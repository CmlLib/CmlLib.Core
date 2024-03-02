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
        if (string.IsNullOrEmpty(file.Path))
            throw new InvalidOperationException();
        if (LauncherOSRule.Current.Name != LauncherOSRule.Windows)
            NativeMethods.Chmod(_filePath ?? file.Path, Mode);
        return new ValueTask();
    }
}