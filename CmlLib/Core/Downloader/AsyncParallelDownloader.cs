using CmlLib.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
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

        int total = 0;
        int progressed = 0;

        bool isRunning = false;

        public AsyncParallelDownloader() : this(10)
        {

        }

        public AsyncParallelDownloader(int parallelism)
        {
            this.MaxThread = parallelism;
        }

        public Task DownloadFiles(DownloadFile[] files)
        {
            if (isRunning)
                throw new InvalidOperationException("already downloading");

            total = files.Length;
            progressed = 0;

            return ForEachAsyncSemaphore(files, MaxThread, doDownload);
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
                //Console.WriteLine("start " + file.Name);
                await downloader.DownloadFileLimitTaskAsync(file.Url, file.Path);
                //Console.WriteLine("end " + file.Name);

                Interlocked.Increment(ref progressed);

                _ = Task.Run(() =>
                {
                    fireDownloadFileChangedEvent(file.Type, file.Name, total, progressed);
                });
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
