using System.Diagnostics;
using CmlLib.Utils;

namespace CmlLib.Core.Tasks;

public class ChmodTask : LinkedTask
{
    public string Path { get; private set; }

    public ChmodTask(string name, string path) : base(name) =>
        Path = path;

    protected override ValueTask<LinkedTask?> OnExecuted()
    {
        try
        {
            if (MRule.OSName != MRule.Windows)
                NativeMethods.Chmod(Path, NativeMethods.Chmod755);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }

        return new ValueTask<LinkedTask?>(NextTask);
    }
}