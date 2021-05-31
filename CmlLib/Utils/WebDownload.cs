using CmlLib.Core.Downloader;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace CmlLib.Utils
{
    public class WebDownload
    {
        private class TimeoutWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest w = base.GetWebRequest(uri);
                w.Timeout = 20 * 1000;
                return w;
            }
        }
        
        private static readonly int DefaultBufferSize = 1024 * 64; // 64kb
        private readonly object locker = new object();
        
        public event EventHandler<FileDownloadProgress> FileDownloadProgressChanged;
        public event ProgressChangedEventHandler DownloadProgressChangedEvent;

        public void DownloadFile(string url, string path)
        {
            var req = WebRequest.CreateHttp(url); // Request
            var response = req.GetResponse();
            var filesize = long.Parse(response.Headers.Get("Content-Length") ?? "0"); // Get File Length

            var webStream = response.GetResponseStream(); // Get NetworkStream
            if (webStream == null)
                throw new NullReferenceException(nameof(webStream));
            
            var fileStream = File.Open(path, FileMode.Create); // Open FileStream

            var bufferSize = DefaultBufferSize; // Make buffer
            var buffer = new byte[bufferSize];
            int length;

            var fireEvent = filesize > DefaultBufferSize;
            var processedBytes = 0;

            while ((length = webStream.Read(buffer, 0, bufferSize)) > 0) // read to end and write file
            {
                fileStream.Write(buffer, 0, length);

                // raise event
                if (fireEvent)
                {
                    processedBytes += length;
                    ProgressChanged(processedBytes, filesize);
                }
            }

            webStream.Dispose(); // Close streams
            fileStream.Dispose();
        }

        public async Task DownloadFileAsync(DownloadFile file)
        {
            string directoryName = Path.GetDirectoryName(file.Path);
            if (!string.IsNullOrEmpty(directoryName))
                Directory.CreateDirectory(directoryName);
            
            using (var wc = new TimeoutWebClient())
            {
                long lastBytes = 0;

                wc.DownloadProgressChanged += (s, e) =>
                {
                    lock (locker)
                    {
                        var progressedBytes = e.BytesReceived - lastBytes;
                        if (progressedBytes < 0)
                            return;

                        lastBytes = e.BytesReceived;

                        var progress = new FileDownloadProgress(
                            file, e.TotalBytesToReceive, progressedBytes, e.BytesReceived, e.ProgressPercentage);
                        FileDownloadProgressChanged?.Invoke(this, progress);
                    }
                };
                await wc.DownloadFileTaskAsync(file.Url, file.Path)
                    .ConfigureAwait(false);
            }
        }

        public void DownloadFileLimit(string url, string path)
        {
            string directoryName = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directoryName))
                Directory.CreateDirectory(directoryName);

            var req = WebRequest.CreateHttp(url);
            req.Method = "GET";
            req.Timeout = 5000;
            req.ReadWriteTimeout = 5000;
            req.ContinueTimeout = 5000;
            var res = req.GetResponse();

            using (var httpStream = res.GetResponseStream())
            using (var fs = File.OpenWrite(path))
            {
                httpStream?.CopyTo(fs);
            }
        }

        private void ProgressChanged(long value, long max)
        {
            var percentage = (float)value / max * 100;

            var e = new ProgressChangedEventArgs((int)percentage, null);
            DownloadProgressChangedEvent?.Invoke(this, e);
        }
    }
}
