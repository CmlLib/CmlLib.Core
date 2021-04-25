namespace CmlLib.Core.Files
{
    public class MLibrary
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Url { get; set; }
        public string Hash { get; set; }
        public long Size { get; set; }
        public bool IsRequire { get; set; }
        public bool IsNative { get; set; }
    }
}
