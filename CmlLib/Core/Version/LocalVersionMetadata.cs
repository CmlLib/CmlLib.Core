using System;
using System.IO;
using System.Threading.Tasks;
using CmlLib.Utils;

namespace CmlLib.Core.Version
{
    public class LocalVersionMetadata : StringVersionMetadata
    {
        public LocalVersionMetadata(string id) : base(id)
        {
            IsLocalVersion = true;
        }

        protected override Task<string> ReadMetadata(bool async)
        {
            if (string.IsNullOrEmpty(Path))
                throw new InvalidOperationException("Path property was null");
            
            // FileNotFoundException will be thrown if Path does not exist.
            if (async)
                return IOUtil.ReadFileAsync(Path);
            else
                return Task.FromResult(File.ReadAllText(Path));
        }
    }
}