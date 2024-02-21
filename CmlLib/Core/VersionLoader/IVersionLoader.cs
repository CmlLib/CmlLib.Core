using System.Threading.Tasks;
using CmlLib.Core.Version;

namespace CmlLib.Core.VersionLoader;

public interface IVersionLoader
{
    Task<MVersionCollection> GetVersionMetadatasAsync();
    MVersionCollection GetVersionMetadatas();
}
