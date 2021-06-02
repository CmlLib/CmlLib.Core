using CmlLib.Core.Version;
using System.Threading.Tasks;

namespace CmlLib.Core.VersionLoader
{
    public interface IVersionLoader
    {
        Task<MVersionCollection> GetVersionMetadatasAsync();
        MVersionCollection GetVersionMetadatas();
    }
}
