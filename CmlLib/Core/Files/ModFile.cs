namespace CmlLib.Core.Files
{
    public class ModFile
    {
        public ModFile()
        {

        }

        public ModFile(string filename, string url)
        {
            this.Name = filename;
            this.Path = System.IO.Path.Combine("mods", filename);
            this.Url = url;
        }

        public ModFile(string filename, string url, string hash) : this(filename, url)
        {
            this.Hash = hash;
        }

        public string Name { get; set; }
        public string Hash { get; set; }
        public string Path { get; set; }
        public string Url { get; set; }
    }
}
