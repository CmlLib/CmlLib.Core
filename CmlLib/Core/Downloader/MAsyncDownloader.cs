using CmlLib.Core.Version;
using CmlLib.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CmlLib.Core.Downloader
{
    public class MAsyncDownloader : MDownloader
    {
        public MAsyncDownloader(MinecraftPath path, MVersion mVersion) : this(path, mVersion, 10, true)
        {

        }

        public MAsyncDownloader(MinecraftPath path, MVersion mVersion, int maxThread, bool setConnectionLimit) : base(path, mVersion)
        {
            MaxThread = maxThread;

            if (setConnectionLimit)
                ServicePointManager.DefaultConnectionLimit = maxThread + 5;
        }

        public int MaxThread { get; private set; }

        int total = 0;
        int progressed = 0;
        SemaphoreSlim semaphore;
        Task progressEvent;

        public override void DownloadFiles(DownloadFile[] files)
        {
            DownloadParallelAsync(files, MaxThread)
                .Wait();
        }

        public async Task DownloadParallelAsync(DownloadFile[] files, int parallelDegree)
        {
            MFile filetype = MFile.Library;
            if (files.Length > 0)
                filetype = files[0].Type;

            semaphore = new SemaphoreSlim(parallelDegree, parallelDegree);
            total = files.Length;
            progressed = 0;
            progressEvent = null;

            var downloadTasks = new List<Task>(files.Length);

            foreach (var file in files)
            {
                await semaphore.WaitAsync().ConfigureAwait(false);
                var t = Task.Run(() => doDownload(file));
                downloadTasks.Add(t);
            }

            var download = Task.WhenAll(downloadTasks);
            await download;

            //if (progressEvent != null && !progressEvent.IsCompleted)
            //    await progressEvent;
        }

        private async Task<bool> doDownload(DownloadFile file, int failedCount = 0)
        {
            try
            {
                if (failedCount > 2)
                    return false;

                var downloader = new WebDownload();
                var dn = downloader.DownloadFileAsync(file.Url, file.Path);

                var ev = Task.Run(() =>
                {
                    fireDownloadFileChangedEvent(file.Type, file.Name, total, progressed);
                });

                Interlocked.Increment(ref progressed);
                await Task.WhenAll(dn, ev);

                semaphore.Release();
                return true;
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine(ex);
                failedCount++;

                var result = await doDownload(file, failedCount);
                if (!result)
                    semaphore.Release();
                return result;
            }
        }
    }
}
