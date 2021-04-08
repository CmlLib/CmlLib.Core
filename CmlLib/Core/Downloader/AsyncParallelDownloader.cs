using CmlLib.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace CmlLib.Core.Downloader
{
    public class AsyncParallelDownloader : IDownloader
    {
        public event DownloadFileChangedHandler ChangeFile;
        public event ProgressChangedEventHandler ChangeProgress;

        public int MaxThread { get; private set; }
        public bool IgnoreInvalidFiles { get; set; } = true;

        int totalFiles = 0;
        int progressedFiles = 0;

        long totalBytes = 0;
        long receivedBytes = 0;

        object progressEventLock = new object();

        bool isRunning = false;

        IProgress<FileProgressChangedEventArgs> pChangeProgress;
        IProgress<DownloadFileChangedEventArgs> pChangeFile;

        public AsyncParallelDownloader() : this(10)
        {

        }

        public AsyncParallelDownloader(int parallelism)
        {
            MaxThread = parallelism;
        }

        public async Task DownloadFiles(DownloadFile[] files)
        {
            if (isRunning)
                throw new InvalidOperationException("already downloading");

            totalFiles = files.Length;
            progressedFiles = 0;

            totalBytes = 1;
            receivedBytes = 0;

            pChangeFile = new Progress<DownloadFileChangedEventArgs>(
                (e) => fireDownloadFileChangedEvent(e));

            pChangeProgress = new Progress<FileProgressChangedEventArgs>(
                (e) => ChangeProgress?.Invoke(this, e));

            foreach (var item in files)
            {
                if (item.Size > 0)
                    totalBytes += item.Size;
            }

            await ForEachAsyncSemaphore(files, MaxThread, doDownload);

            //var lastFile = files.Last();
            //fireDownloadFileChangedProgress(lastFile, files.Length, files.Length);
        }

        private async Task ForEachAsyncSemaphore<T>(IEnumerable<T> source,
    int degreeOfParallelism, Func<T, Task> body)
        {
            var tasks = new List<Task>();
            using (var throttler = new SemaphoreSlim(degreeOfParallelism))
            {
                foreach (var element in source)
                {
                    await throttler.WaitAsync();
                    tasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            await body(element);
                        }
                        finally
                        {
                            throttler.Release();
                        }
                    }));
                }
                await Task.WhenAll(tasks);
            }
        }

        private async Task doDownload(DownloadFile file)
        {
            try
            {
                await doDownload(file, 3);
            }
            catch (Exception ex)
            {
                if (!IgnoreInvalidFiles)
                    throw new MDownloadFileException("failed to download", ex, file);
            }
        }

        private async Task doDownload(DownloadFile file, int retry)
        {
            try
            {
                var downloader = new WebDownload();
                downloader.FileDownloadProgressChanged += Downloader_FileDownloadProgressChanged;

                var downloadTask = downloader.DownloadFileAsync(file);

                fireDownloadFileChangedProgress(file.Type, file.Name, totalFiles, progressedFiles);
                await downloadTask;

                if (file.AfterDownload != null)
                {
                    foreach (var item in file.AfterDownload)
                    {
                        await item?.Invoke();
                    }
                }

                Interlocked.Increment(ref progressedFiles);
            }
            catch (Exception ex)
            {
                if (retry <= 0)
                    return;

                System.Diagnostics.Debug.WriteLine(ex);
                retry--;

                await doDownload(file, retry);
            }
        }

        private void Downloader_FileDownloadProgressChanged(object sender, FileDownloadProgress e)
        {
            lock (progressEventLock)
            {
                if (e.File.Size <= 0)
                {
                    totalBytes += e.TotalBytes;
                    e.File.Size = e.TotalBytes;
                }

                receivedBytes += e.ProgressedBytes;

                if (receivedBytes > totalBytes)
                    return;

                float percent = (float)receivedBytes / totalBytes * 100;
                pChangeProgress.Report(new FileProgressChangedEventArgs(totalBytes, receivedBytes, (int)percent));
            }
        }

        private void fireDownloadFileChangedProgress(MFile file, string name, int totalFiles, int progressedFiles)
        {
            var e = new DownloadFileChangedEventArgs(file, name, totalFiles, progressedFiles);
            pChangeFile.Report(e);
        }

        private void fireDownloadFileChangedEvent(DownloadFileChangedEventArgs e)
        {
            ChangeFile?.Invoke(e);
        }
    }
}
