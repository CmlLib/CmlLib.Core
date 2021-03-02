using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace CmlLib.Core.Installer
{
    public interface IDownloader
    {
        event DownloadFileChangedHandler ChangeFile;
        event ProgressChangedEventHandler ChangeProgress;
        Task DownloadFiles(DownloadFile[] files);
    }
}
