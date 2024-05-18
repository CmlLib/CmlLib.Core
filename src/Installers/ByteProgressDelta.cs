namespace CmlLib.Core.Installers;

public class ByteProgressDelta : IProgress<ByteProgress>
{
    private readonly Action<ByteProgress> _action;
    private ByteProgress lastProgress;

    public ByteProgressDelta(long initialSize, Action<ByteProgress> action)
    {
        lastProgress = new ByteProgress(initialSize, 0);
        _action = action;
    }

    public ByteProgressDelta(ByteProgress initial, Action<ByteProgress> action)
    {
        lastProgress = initial;
        _action = action;
    }

    public void Report(ByteProgress value)
    {
        var delta = value - lastProgress;
        lastProgress = value;
        _action(delta);
    }

    public void ReportDone()
    {
        Report(new ByteProgress(lastProgress.TotalBytes, lastProgress.TotalBytes));
    }
}