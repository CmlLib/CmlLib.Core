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

        private int totalFiles = 0;
        private int progressedFiles = 0;

        private long totalBytes = 0;
        private long receivedBytes = 0;

        private readonly object progressEventLock = new object();

        private bool isRunning = false;

        private IProgress<FileProgressChangedEventArgs> pChangeProgress;
        private IProgress<DownloadFileChangedEventArgs> pChangeFile;

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

            isRunning = true;

            totalFiles = files.Length;
            progressedFiles = 0;

            totalBytes = 1;
            receivedBytes = 0;

            pChangeFile = new Progress<DownloadFileChangedEventArgs>(
                (e) => fireDownloadFileChangedEvent(e));

            pChangeProgress = new Progress<FileProgressChangedEventArgs>(
                (e) => ChangeProgress?.Invoke(this, e));

            foreach (DownloadFile item in files)
            {
                if (item.Size > 0)
                    totalBytes += item.Size;
            }

            await ForEachAsyncSemaphore(files, MaxThread, doDownload).ConfigureAwait(false);

            isRunning = false;
        }

        private async Task ForEachAsyncSemaphore<T>(IEnumerable<T> source,
    int degreeOfParallelism, Func<T, Task> body)
        {
            List<Task> tasks = new List<Task>();
            using (SemaphoreSlim throttler = new SemaphoreSlim(degreeOfParallelism))
            {
                foreach (T element in source)
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
                WebDownload downloader = new WebDownload();
                downloader.FileDownloadProgressChanged += Downloader_FileDownloadProgressChanged;

                Task downloadTask = downloader.DownloadFileAsync(file);

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

                await doDownload(file, retry).ConfigureAwait(false);
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
