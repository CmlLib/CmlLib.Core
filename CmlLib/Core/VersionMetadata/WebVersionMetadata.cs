using System;
using System.Net;
using System.Threading.Tasks;

namespace CmlLib.Core.VersionMetadata
{
    public class WebVersionMetadata : StringVersionMetadata
    {
        public WebVersionMetadata(string id) : base(id)
        {
            IsLocalVersion = false;
        }

        protected override string ReadMetadata()
        {
            if (string.IsNullOrEmpty(Path))
                throw new InvalidOperationException("Path property was null");
            
            // below code will throw ArgumentNullException when Path is null
            using var wc = new WebClient();
            return wc.DownloadString(Path);
        }

        protected override async Task<string> ReadMetadataAsync()
        {
            if (string.IsNullOrEmpty(Path))
                throw new InvalidOperationException("Path property was null");
            
            // below code will throw ArgumentNullException when Path is null
            using var wc = new WebClient();
            return await wc.DownloadStringTaskAsync(Path)
                .ConfigureAwait(false);
        }
    }
}