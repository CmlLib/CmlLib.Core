namespace CmlLib.Core.Installers;

public enum InstallerEventType
{
    Queued,
    Done
}

public class InstallerProgressChangedEventArgs
{
    public InstallerProgressChangedEventArgs(
        int totalTasks,
        int progressedTasks,
        string? name,
        InstallerEventType type)
    {
        TotalTasks = totalTasks;
        ProgressedTasks = progressedTasks;
        Name = name;
        EventType = type;
    }

    public int TotalTasks { get; }
    public int ProgressedTasks { get; }
    public string? Name { get; }
    public InstallerEventType EventType { get; }
}