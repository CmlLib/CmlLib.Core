namespace CmlLib.Core.Installers;

public struct InstallerProgressChangedEventArgs
{
    public InstallerProgressChangedEventArgs(string name, TaskStatus status) =>
        (Name, EventType) = (name, status);

    public int TotalTasks { get; set; } = 0;
    public int ProceedTasks { get; set; } = 0;
    public TaskStatus EventType { get; }
    public string Name { get; }
}