using CmlLib.Core.Tasks;

namespace CmlLib.Core.Test.Tasks;

public class MockDownloadTask : MockTask
{
    public MockDownloadTask(TaskFile file) : base(file)
    {
        TotalEventSize = file.Size;
    }

    public long TotalEventSize { get; set; }

    protected override ValueTask<LinkedTask?> OnExecuted(IProgress<ByteProgress>? progress, CancellationToken cancellationToken)
    {
        for (int i = 0; i <= TotalEventSize; i += 128)
        {
            progress?.Report(new ByteProgress
            {
                TotalBytes = Size,
                ProgressedBytes = i
            });
        }
        return base.OnExecuted(progress, cancellationToken);
    }
}