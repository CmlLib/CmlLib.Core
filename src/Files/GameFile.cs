using CmlLib.Core.Tasks;

namespace CmlLib.Core.Files;

public record GameFile
{
    public GameFile(string name) => 
        Name = name;

    public string Name { get; }
    public string? Path { get; init; }
    public string? Hash { get; init; }
    public string? Url { get; init; }
    public long Size { get; init; }
    public IUpdateTask? UpdateTask { get; init; }

    public async ValueTask ExecuteUpdateTask(CancellationToken cancellationToken)
    {
        if (UpdateTask != null)
            await UpdateTask.Execute(this, cancellationToken);
    }
}