using CmlLib.Core.VersionMetadata;

namespace CmlLib.Core.VersionLoader
{
    public interface IVersionLoader
    {
        ValueTask<MVersionCollection> GetVersionMetadatasAsync();
    }
}
