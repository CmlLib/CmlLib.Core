using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using CmlLib.Utils;

namespace CmlLib.Core.Version
{
    public class WebVersionMetadata : StringVersionMetadata
    {
        public WebVersionMetadata(string id) : base(id)
        {
            IsLocalVersion = false;
        }

        protected override async Task<string> ReadMetadata(bool async)
        {
            if (string.IsNullOrEmpty(Path))
                throw new InvalidOperationException("Path property was null");
            
            using (var wc = new WebClient())
            {
                // below code will throw ArgumentNullException when Path is null
                string res;
                if (async)
                    res = await wc.DownloadStringTaskAsync(Path).ConfigureAwait(false);
                else
                    res = wc.DownloadString(Path);
                return res;
            }
        }
    }
}