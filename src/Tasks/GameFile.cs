namespace CmlLib.Core.Tasks;

public class GameFile
{
    public GameFile(string name) => 
        Name = name;

    public string Name { get; }
    public string? Path { get; set; }
    public string? Hash { get; set; }
    public string? Url { get; set; }
    public long Size { get; set; }
    public IUpdateTask? UpdateTask { get; set; }

    public async ValueTask ExecuteUpdateTask(CancellationToken cancellationToken)
    {
        if (UpdateTask != null)
            await UpdateTask.Execute(this, cancellationToken);
    }
}