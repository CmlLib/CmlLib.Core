namespace CmlLib.Core.VersionMetadata;

public static class MVersionTypeConverter
{
    public static MVersionType Parse(string input) => input switch
    {
        "release" => MVersionType.Release,
        "snapshot" => MVersionType.Snapshot,
        "old_alpha" => MVersionType.OldAlpha,
        "old_beta" => MVersionType.OldBeta,
        _ => MVersionType.Custom,
    };

    public static string ConvertToTypeString(this MVersionType type) => type switch
    {
        MVersionType.Release => "release",
        MVersionType.Snapshot => "snapshot",
        MVersionType.OldAlpha => "old_alpha",
        MVersionType.OldBeta => "old_beta",
        _ => "custom"
    };

    public static bool IsOldVersion(this MVersionType type) =>
        type == MVersionType.OldAlpha || type == MVersionType.OldBeta;
}