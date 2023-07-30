using CmlLib.Utils;
using System;
using System.Threading.Tasks;

namespace CmlLib.Core.VersionMetadata
{
    /// <summary>
    /// Represent metadata where the actual version data is on web
    /// </summary>
    public class WebVersionMetadata : StringVersionMetadata
    {
        public WebVersionMetadata(string name) : base(name)
        {
            IsLocalVersion = false;
        }

        protected override async Task<string> ReadVersionDataAsync()
        {
            if (string.IsNullOrEmpty(Path))
                throw new InvalidOperationException("Path property was null");

            // below code will throw ArgumentNullException when Path is null
            return await HttpUtil.HttpClient.GetStringAsync(Path)
                .ConfigureAwait(false);
        }
    }
}