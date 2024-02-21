using System;
using System.Threading.Tasks;
using CmlLib.Core.Downloader;
using CmlLib.Core.Version;

namespace CmlLib.Core.Files;

public interface IFileChecker
{
    DownloadFile[]? CheckFiles(MinecraftPath path, MVersion version,
        IProgress<DownloadFileChangedEventArgs>? downloadProgress);

    Task<DownloadFile[]?> CheckFilesTaskAsync(MinecraftPath path, MVersion version,
        IProgress<DownloadFileChangedEventArgs>? downloadProgress);
}
