using CmlLib.Core.Downloader;
using CmlLib.Core.Version;

namespace CmlLib.Core.FileChecker
{
    public interface IFileChecker
    {
        DownloadFile[]? CheckFiles(MinecraftPath path, IVersion version,
            IProgress<DownloadFileChangedEventArgs>? downloadProgress);
        Task<DownloadFile[]?> CheckFilesTaskAsync(MinecraftPath path, IVersion version, 
            IProgress<DownloadFileChangedEventArgs>? downloadProgress);
    }
}
