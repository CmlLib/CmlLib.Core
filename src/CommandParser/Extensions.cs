namespace CmlLib.Core.CommandParser;

public static class Extensions
{
    public static bool ContainsXmx(this CommandLineBuilder builder)
    {
        return builder.Arguments.Any(arg => arg.Key.StartsWith("-Xmx"));
    }

    public static bool ContainsXms(this CommandLineBuilder builder)
    {
        return builder.Arguments.Any(arg => arg.Key.StartsWith("-Xms"));
    }
}