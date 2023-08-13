using CmlLib.Core.Tasks;

namespace CmlLib.Core.Benchmarks;

public class DummyTask : LinkedTask
{
    public static int Seed { get; set; }

    public DummyTask(TaskFile file, int seed) : base(file) => Seed = seed;

    protected override ValueTask<LinkedTask?> OnExecuted(
        IProgress<ByteProgress>? progress,
        CancellationToken cancellationToken)
    {
        for (int j = 0; j < 1024*256; j++)
            Seed += j;
        return new ValueTask<LinkedTask?>(NextTask);
    }
}