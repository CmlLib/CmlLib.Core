namespace CmlLib.Core.Installers;

public class ByteProgressDelta : IProgress<ByteProgress>
{
    private readonly Action<ByteProgress> _action;
    private ByteProgress lastProgress;

    public ByteProgressDelta(long initialSize, Action<ByteProgress> action)
    {
        lastProgress = new ByteProgress
        {
            TotalBytes = initialSize,
            ProgressedBytes = 0
        };
        _action = action;
    }

    public ByteProgressDelta(ByteProgress initial, Action<ByteProgress> action)
    {
        lastProgress = initial;
        _action = action;
    }

    public void Report(ByteProgress value)
    {
        var delta = new ByteProgress
        {
            TotalBytes = value.TotalBytes - lastProgress.TotalBytes,
            ProgressedBytes = value.ProgressedBytes - lastProgress.ProgressedBytes
        };
        lastProgress = value;
        _action(delta);
    }

    public void ReportDone()
    {
        Report(new ByteProgress
        {
            TotalBytes = lastProgress.TotalBytes,
            ProgressedBytes = lastProgress.TotalBytes
        });
    }
}