using System.ComponentModel;
using System.Threading.Tasks;

namespace CmlLib.Core.Downloader
{
    public interface IDownloader
    {
        event DownloadFileChangedHandler ChangeFile;
        event ProgressChangedEventHandler ChangeProgress;
        Task DownloadFiles(DownloadFile[] files);
    }
}
