using CmlLib.Core.Rules;
using CmlLib.Core.Internals;

namespace CmlLib.Core.Tasks;

public class ChmodTask : IUpdateTask
{
    public ChmodTask(int mode) => Mode = mode;

    public int Mode { get; }

    public ValueTask Execute(
        GameFile file,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(file.Path))
            throw new InvalidOperationException();
        if (LauncherOSRule.Current.Name != LauncherOSRule.Windows)
            NativeMethods.Chmod(file.Path, Mode);
        return new ValueTask();
    }
}