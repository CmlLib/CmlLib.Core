using CmlLib.Core.Installer;
using CmlLib.Core.Files;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CmlLib.Core.Files
{
    public interface IFileChecker
    {
        event DownloadFileChangedHandler ChangeFile;
        DownloadFile[] CheckFiles(MinecraftPath path, MVersion version);
        Task<DownloadFile[]> CheckFilesTaskAsync(MinecraftPath path, MVersion version);
    }
}
