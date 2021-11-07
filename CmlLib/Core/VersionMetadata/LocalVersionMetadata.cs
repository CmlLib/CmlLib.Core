using System;
using System.IO;
using System.Threading.Tasks;
using CmlLib.Utils;

namespace CmlLib.Core.VersionMetadata
{
    public class LocalVersionMetadata : StringVersionMetadata
    {
        public LocalVersionMetadata(string id) : base(id)
        {
            IsLocalVersion = true;
        }

        protected override string ReadMetadata()
        {
            if (string.IsNullOrEmpty(Path))
                throw new InvalidOperationException("Path property was null");
            
            // FileNotFoundException will be thrown if Path does not exist.
            return File.ReadAllText(Path);
        }

        protected override Task<string> ReadMetadataAsync()
        {
            if (string.IsNullOrEmpty(Path))
                throw new InvalidOperationException("Path property was null");
            
            // FileNotFoundException will be thrown if Path does not exist.
            return IOUtil.ReadFileAsync(Path);
        }
    }
}