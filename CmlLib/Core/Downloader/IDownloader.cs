using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CmlLib.Core.Downloader
{
    public interface IDownloader
    {
        Task DownloadFiles(DownloadFile[] files, 
            IProgress<DownloadFileChangedEventArgs>? fileProgress, 
            IProgress<ProgressChangedEventArgs>? downloadProgress);
    }
}
