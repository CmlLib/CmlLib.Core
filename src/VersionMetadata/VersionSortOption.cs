namespace CmlLib.Core.VersionMetadata;

public enum MVersionSortPropertyOption
{
    Name,
    Version,
    ReleaseDate
}

// Decide how to sort version if ReleaseDate is undefined
public enum MVersionNullReleaseDateSortOption
{
    AsLatest,
    AsOldest
}

public class MVersionSortOption
{
    public MVersionType[] TypeOrder { get; set; } =
    {
        MVersionType.Custom,
        MVersionType.Release,
        MVersionType.Snapshot,
        MVersionType.OldBeta,
        MVersionType.OldAlpha
    };

    public MVersionSortPropertyOption PropertyOrderBy { get; set; }
        = MVersionSortPropertyOption.Version;

    public bool AscendingPropertyOrder { get; set; } = true;

    public MVersionNullReleaseDateSortOption NullReleaseDateSortOption { get; set; } =
        MVersionNullReleaseDateSortOption.AsOldest;

    public bool CustomAsRelease { get; set; } = false;
    public bool TypeClassification { get; set; } = true;
}
