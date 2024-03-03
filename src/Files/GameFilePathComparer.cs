namespace CmlLib.Core.Files;

public class GameFilePathComparer : IEqualityComparer<GameFile>
{
    public static readonly GameFilePathComparer Default = new GameFilePathComparer();

    public bool Equals(GameFile? x, GameFile? y)
    {
        return x?.Path == y?.Path;
    }

    public int GetHashCode(GameFile obj)
    {
        return (obj.Path ?? "").GetHashCode();
    }
}