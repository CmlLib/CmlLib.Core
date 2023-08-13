using CmlLib.Core.Rules;
using CmlLib.Core.Internals;

namespace CmlLib.Core.Tasks;

public class ChmodTask : LinkedTask
{
    public string Path { get; private set; }

    public ChmodTask(string name, string path) : base(name) =>
        Path = path;

    protected override ValueTask<LinkedTask?> OnExecuted(
        IProgress<ByteProgressEventArgs>? progress,
        CancellationToken cancellationToken)
    {
        if (LauncherOSRule.Current.Name != LauncherOSRule.Windows)
            NativeMethods.Chmod(Path, NativeMethods.Chmod755);
        return new ValueTask<LinkedTask?>(NextTask);
    }
}