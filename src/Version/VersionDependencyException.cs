namespace CmlLib.Core.Version;

public class VersionDependencyException : Exception
{
    public VersionDependencyException(string message) : base(message)
    {

    }

    internal static VersionDependencyException CreateCircularDependencyException(string versionId)
    {
        var msg = $"Circular dependency detected in version tree. Cycle includes version: {versionId}";
        return new VersionDependencyException(msg);
    }

    internal static VersionDependencyException CreateExcessiveDepthMessage(int maxDepth, string versionId)
    {
        var msg = $"Version dependency tree exceeds maximum depth of {maxDepth}. Deepest version: {versionId}";
        return new VersionDependencyException(msg);
    }
}
