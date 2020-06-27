using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace CmlLib.Core
{
    public class MParallelDownloader : MDownloader
    {
        public MParallelDownloader(MProfile mProfile) : this(mProfile, 10, true)
        {

        }

        public MParallelDownloader(MProfile mProfile, int maxThread, bool setConnectionLimit) : base(mProfile)
        {
            this.MaxThread = maxThread;

            if (setConnectionLimit)
                ServicePointManager.DefaultConnectionLimit = maxThread;
        }

        public int MaxThread { get; private set; }
        object lockEvent = new object();

        public override void DownloadFiles(DownloadFile[] files)
        {
            TryDownloadFiles(files, 3, null);
        }

        private void TryDownloadFiles(DownloadFile[] files, int retry, Exception failEx)
        {
            if (retry == 0)
            {
                if (failEx == null)
                    failEx = new WebException("Cannot download files");

                failEx.Data["files"] = files;
                throw failEx;
            }

            var length = files.Length;
            if (length == 0)
                return;

            var progressed = 0;
            fireDownloadFileChangedEvent(files[0].Type, files[0].Name, length, 0);

            var option = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 10
            };

            var failedFiles = new List<DownloadFile>();

            Exception lastEx = null;
            Parallel.ForEach(files, option, (DownloadFile file) =>
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(file.Path));
                    using (var wc = new WebClient())
                    {
                        wc.DownloadFile(file.Url, file.Path);
                    }

                    lock (lockEvent)
                    {
                        progressed++;
                        fireDownloadFileChangedEvent(file.Type, file.Name, length, progressed);
                    }
                }
                catch (Exception ex)
                {
                    failedFiles.Add(file);
                    lastEx = ex;
                }
            });

            TryDownloadFiles(failedFiles.ToArray(), retry - 1, lastEx);
        }
    }
}
