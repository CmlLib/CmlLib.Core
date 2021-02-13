using CmlLib.Core.Downloader;
using CmlLib.Core.Version;
using System;
using System.Collections.Generic;
using System.Text;

namespace CmlLib.Core.Files
{
    public interface IFileChecker
    {
        event DownloadFileChangedHandler ChangeFile;
        DownloadFile[] CheckFiles(MinecraftPath path, MVersion version);
    }
}
