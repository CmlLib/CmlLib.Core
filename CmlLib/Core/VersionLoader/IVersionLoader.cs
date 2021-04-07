using CmlLib.Core.Version;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CmlLib.Core.VersionLoader
{
    public interface IVersionLoader
    {
        Task<MVersionCollection> GetVersionMetadatasAsync();
        MVersionCollection GetVersionMetadatas();
    }
}
