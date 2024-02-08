namespace CmlLib.Core.Installers;

public class InstallerProgressChangedEventArgs
{
    public int TotalTasks;
    public int ProgressedTasks;
    public string? Name;
}