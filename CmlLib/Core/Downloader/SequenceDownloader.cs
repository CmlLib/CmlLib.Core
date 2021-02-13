using CmlLib.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CmlLib.Core.Downloader
{
    public class SequenceDownloader : IDownloader
    {
        public event DownloadFileChangedHandler ChangeFile;
        public event ProgressChangedEventHandler ChangeProgress;

        public bool IgnoreInvalidFiles { get; set; } = true;

        public async Task DownloadFiles(DownloadFile[] files)
        {
            if (files == null || files.Length == 0)
                return;

            var downloader = new WebDownload();
            downloader.DownloadProgressChangedEvent += fireDownloadProgressChangedEvent;

            fireDownloadFileChangedEvent(files[0], 0, 0);

            for (var i = 0; i < files.Length; i++)
            {
                DownloadFile file = files[i];

                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(file.Path));
                    await downloader.DownloadFileAsync(file.Url, file.Path);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());

                    if (!IgnoreInvalidFiles)
                        throw new MDownloadFileException(ex.Message, ex, files[i]);
                }
                finally
                {
                    fireDownloadFileChangedEvent(file, files.Length, i);
                }
            }
        }

        private void fireDownloadFileChangedEvent(MFile file, string name, int totalFiles, int progressedFiles)
        {
            var e = new DownloadFileChangedEventArgs(file, name, totalFiles, progressedFiles);
            fireDownloadFileChangedEvent(e);
        }

        private void fireDownloadFileChangedEvent(DownloadFile file, int totalFiles, int progressedFiles)
        {
            fireDownloadFileChangedEvent(file.Type, file.Name, totalFiles, progressedFiles);
        }

        private void fireDownloadFileChangedEvent(DownloadFileChangedEventArgs e)
        {
            ChangeFile?.Invoke(e);
        }

        private void fireDownloadProgressChangedEvent(object sender, ProgressChangedEventArgs e)
        {
            ChangeProgress?.Invoke(this, e);
        }
    }
}
