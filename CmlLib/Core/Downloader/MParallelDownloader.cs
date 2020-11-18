using CmlLib.Core.Version;
using CmlLib.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
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

        int total = 0;
        int progressed = 0;

        public override void DownloadFiles(DownloadFile[] files)
        {
            total = files.Length;
            progressed = 0;

            Parallel.ForEach(
                files,
                new ParallelOptions() { MaxDegreeOfParallelism = MaxThread },
                doDownload);

            Console.WriteLine("completed");
        }

        private void doDownload(DownloadFile file)
        {
            doDownload(file, 0);
        }

        private bool doDownload(DownloadFile file, int failedCount)
        {
            try
            {
                if (failedCount > 2)
                    return false;

                var downloader = new WebDownload();
                Console.WriteLine("start " + file.Name);
                downloader.DownloadFileLimit(file.Url, file.Path);
                Console.WriteLine("end " + file.Name);

                Interlocked.Increment(ref progressed);

                var ev = Task.Run(() =>
                {
                    fireDownloadFileChangedEvent(file.Type, file.Name, total, progressed);
                });
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                failedCount++;

                return doDownload(file, failedCount);
            }
        }
    }
}
