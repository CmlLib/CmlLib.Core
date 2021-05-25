using System;
using CmlLib.Core.Downloader;
using CmlLib.Core.Version;
using System.Threading.Tasks;

namespace CmlLib.Core.Files
{
    public interface IFileChecker
    {
        DownloadFile[] CheckFiles(MinecraftPath path, MVersion version,
            IProgress<DownloadFileChangedEventArgs> downloadProgress);
        Task<DownloadFile[]> CheckFilesTaskAsync(MinecraftPath path, MVersion version, 
            IProgress<DownloadFileChangedEventArgs> downloadProgress);
    }
}
