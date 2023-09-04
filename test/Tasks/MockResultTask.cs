using CmlLib.Core.Tasks;

namespace CmlLib.Core.Test.Tasks;

public class MockResultTask : ResultTask
{
    public bool Result { get; set; }

    public MockResultTask(string name) : base(name)
    {
    }

    protected override ValueTask<bool> OnExecutedWithResult(IProgress<ByteProgress>? progress, CancellationToken cancellationToken)
    {
        return new ValueTask<bool>(Result);
    }
}