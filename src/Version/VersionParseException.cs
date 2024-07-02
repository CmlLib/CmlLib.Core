namespace CmlLib.Core.Version;

public class VersionParseException : Exception
{
    public VersionParseException(string message) : base(message)
    {

    }

    public VersionParseException(Exception inner) : base("Failed to parse version", inner)
    {

    }

    public string? VersionName { get; internal set; }
}
