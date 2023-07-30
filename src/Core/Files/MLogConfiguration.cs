namespace CmlLib.Core.Files
{
    // Represent log configuration. "logging" property of <version>.json file
    public class MLogConfiguration
    {
        public MFileMetadata? LogFile { get; set; }
        public string? Argument { get; set; }
        public string? Type { get; set; }
    }
}
