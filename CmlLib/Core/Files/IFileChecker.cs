using CmlLib.Core.Installer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CmlLib.Core.Version;

namespace CmlLib.Core.Files
{
    public interface IFileChecker
    {
        event DownloadFileChangedHandler ChangeFile;
        DownloadFile[] CheckFiles(MinecraftPath path, MVersion version);
        Task<DownloadFile[]> CheckFilesTaskAsync(MinecraftPath path, MVersion version);
    }
}
