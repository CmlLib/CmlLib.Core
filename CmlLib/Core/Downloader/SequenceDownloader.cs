using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CmlLib.Utils;

namespace CmlLib.Core.Downloader;

public class SequenceDownloader : IDownloader
{
    private IProgress<ProgressChangedEventArgs>? pChangeProgress;
    public bool IgnoreInvalidFiles { get; set; } = true;

    public async Task DownloadFiles(DownloadFile[] files,
        IProgress<DownloadFileChangedEventArgs>? fileProgress,
        IProgress<ProgressChangedEventArgs>? downloadProgress)
    {
        if (files.Length == 0)
            return;

        pChangeProgress = downloadProgress;

        var downloader = new WebDownload();
        downloader.FileDownloadProgressChanged += Downloader_FileDownloadProgressChanged;

        fileProgress?.Report(
            new DownloadFileChangedEventArgs(files[0].Type, this, null, files.Length, 0));

        for (var i = 0; i < files.Length; i++)
        {
            var file = files[i];

            try
            {
                var directoryPath = Path.GetDirectoryName(file.Path);
                if (!string.IsNullOrEmpty(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                await downloader.DownloadFileAsync(file).ConfigureAwait(false);

                if (file.AfterDownload != null)
                    foreach (var item in file.AfterDownload)
                        await item().ConfigureAwait(false);

                fileProgress?.Report(
                    new DownloadFileChangedEventArgs(file.Type, this, file.Name, files.Length, i));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());

                if (!IgnoreInvalidFiles)
                    throw new MDownloadFileException(ex.Message, ex, files[i]);
            }
        }
    }

    private void Downloader_FileDownloadProgressChanged(object? sender, DownloadFileProgress e)
    {
        pChangeProgress?.Report(new ProgressChangedEventArgs(e.ProgressPercentage, null));
    }
}
