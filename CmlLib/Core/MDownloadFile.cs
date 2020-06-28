using System;
using System.Collections.Generic;
using System.Text;

namespace CmlLib.Core
{
    public class DownloadFile : IEquatable<DownloadFile>
    {
        public DownloadFile(MFile type, string name, string path, string url)
        {
            this.Type = type;
            this.Name = name;
            this.Path = path;
            this.Url = url;
        }

        public MFile Type { get; private set; }
        public string Name { get; private set; }
        public string Path { get; private set; }
        public string Url { get; private set; }

        bool IEquatable<DownloadFile>.Equals(DownloadFile other)
        {
            if (other == null)
                return false;

            return this.Path == other.Path;
        }

        public override int GetHashCode()
        {
            return this.Path.GetHashCode();
        }
    }
}
