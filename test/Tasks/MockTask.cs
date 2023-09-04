using CmlLib.Core.Tasks;

namespace CmlLib.Core.Test.Tasks;

public class MockTask : LinkedTask
{
    public bool IsExecuted { get; private set; }
    public TaskFile File { get; }

    public MockTask(TaskFile file) : base(file)
    {
        File = file;
    }

    protected override ValueTask<LinkedTask?> OnExecuted(IProgress<ByteProgress>? progress, CancellationToken cancellationToken)
    {
        IsExecuted = true;
        return new ValueTask<LinkedTask?>(NextTask);
    }
}