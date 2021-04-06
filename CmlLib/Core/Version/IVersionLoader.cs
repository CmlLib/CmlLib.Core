using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CmlLib.Core.Version
{
    public interface IVersionLoader
    {
        Task<MVersionCollection> GetVersionMetadatasAsync();
        MVersionCollection GetVersionMetadatas();
    }
}
