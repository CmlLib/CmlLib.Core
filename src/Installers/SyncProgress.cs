namespace CmlLib.Core.Installers;

public class SyncProgress<T> : IProgress<T>
{
    private readonly Action<T> _report;

    public SyncProgress(Action<T> report) =>
        _report = report;

    public void Report(T value)
    {
        _report.Invoke(value);
    }
}