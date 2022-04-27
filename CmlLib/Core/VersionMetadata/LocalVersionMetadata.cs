using System;
using System.IO;
using System.Threading.Tasks;
using CmlLib.Utils;

namespace CmlLib.Core.VersionMetadata
{
    /// <summary>
    /// Represent metadata where the actual version data is in local file
    /// </summary>
    public class LocalVersionMetadata : StringVersionMetadata
    {
        public LocalVersionMetadata(string id) : base(id)
        {
            IsLocalVersion = true;
        }

        protected override Task<string> ReadVersionDataAsync()
        {
            if (string.IsNullOrEmpty(Path))
                throw new InvalidOperationException("Path property was null");
            
            // FileNotFoundException will be thrown if Path does not exist.
            return IOUtil.ReadFileAsync(Path);
        }
    }
}