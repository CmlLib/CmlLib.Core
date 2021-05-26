using CmlLib.Utils;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace CmlLib.Core.Downloader
{
    public class SequenceDownloader : IDownloader
    {
        public bool IgnoreInvalidFiles { get; set; } = true;
        private IProgress<ProgressChangedEventArgs> pChangeProgress;

        public async Task DownloadFiles(DownloadFile[] files, 
            IProgress<DownloadFileChangedEventArgs> fileProgress,
            IProgress<ProgressChangedEventArgs> downloadProgress)
        {
            if (files == null || files.Length == 0)
                return;

            pChangeProgress = downloadProgress;

            WebDownload downloader = new WebDownload();
            downloader.FileDownloadProgressChanged += Downloader_FileDownloadProgressChanged;

            //fireDownloadFileChangedEvent(null, 0, files.Length);

            for (int i = 0; i < files.Length; i++)
            {
                DownloadFile file = files[i];

                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(file.Path));
                    await downloader.DownloadFileAsync(file).ConfigureAwait(false);

                    if (file.AfterDownload != null)
                    {
                        foreach (var item in file.AfterDownload)
                        {
                            await item().ConfigureAwait(false);
                        }
                    }
                    
                    fileProgress?.Report(
                        new DownloadFileChangedEventArgs(file.Type, file.Name, files.Length, i));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());

                    if (!IgnoreInvalidFiles)
                        throw new MDownloadFileException(ex.Message, ex, files[i]);
                }
            }
        }

        private void Downloader_FileDownloadProgressChanged(object sender, FileDownloadProgress e)
        {
            pChangeProgress?.Report(new ProgressChangedEventArgs(e.ProgressPercentage, null));
        }
    }
}
