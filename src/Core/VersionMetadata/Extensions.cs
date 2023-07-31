namespace CmlLib.Core.VersionMetadata;

public static class Extensions
{
    public static MVersionType GetVersionType(this IVersionMetadata version)
    {
        return MVersionTypeConverter.FromString(version.Type);
    }
}