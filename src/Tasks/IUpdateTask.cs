namespace CmlLib.Core.Tasks;

public interface IUpdateTask
{
    ValueTask Execute(GameFile file, CancellationToken cancellationToken);
}