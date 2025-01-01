namespace CmlLib.Core.VersionMetadata;

public static class Extensions
{
    public static MVersionType GetVersionType(this IVersionMetadata version)
    {
        if (string.IsNullOrEmpty(version.Type))
            return MVersionType.Custom;
        else
            return MVersionTypeConverter.Parse(version.Type);
    }
}