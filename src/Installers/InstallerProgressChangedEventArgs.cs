namespace CmlLib.Core.Installers;

public struct InstallerProgressChangedEventArgs
{
    public int TotalTasks;
    public int ProgressedTasks;
    public string? Name;
}