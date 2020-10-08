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
    public class MParallelDownloader : MDownloader
    {
        public MParallelDownloader(MinecraftPath path, MVersion mVersion) : this(path, mVersion, 10, true)
        {

        }

        public MParallelDownloader(MinecraftPath path, MVersion mVersion, int maxThread, bool setConnectionLimit) : base(path, mVersion)
        {
            MaxThread = maxThread;

            if (setConnectionLimit)
                ServicePointManager.DefaultConnectionLimit = maxThread + 5;
        }

        public int MaxThread { get; private set; }

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

            var downloadTasks = new List<Task>(files.Length);
            var semaphore = new SemaphoreSlim(parallelDegree, parallelDegree);

            var progressed = 0;

            foreach (var file in files)
            {
                await semaphore.WaitAsync().ConfigureAwait(false);
                var t = Task.Run(() => doDownload(file.Path, file.Url));
                downloadTasks.Add(t);
            }

            Task waitEvent = null;
            async Task doDownload(string path, string url)
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));

                    var req = WebRequest.CreateHttp(url);
                    req.Method = "GET";
                    var res = await req.GetResponseAsync().ConfigureAwait(false);

                    using (var httpStream = res.GetResponseStream())
                    using (var fs = File.OpenWrite(path))
                    {
                        await httpStream.CopyToAsync(fs).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
                finally
                {
                    Interlocked.Increment(ref progressed);
                    waitEvent = Task.Run(() =>
                    {
                        fireDownloadFileChangedEvent(filetype, "", files.Length, progressed);
                    });

                    semaphore.Release();
                }
            }

            var download = Task.WhenAll(downloadTasks);
            await download;

            if (waitEvent != null)
                await waitEvent;
        }
    }
}
