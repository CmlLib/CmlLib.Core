using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CmlLib.Launcher
{
    class WebDownload
    {
        static int DefaultBufferSize = 1024 * 1024 * 1; // 1MB

        public event ProgressChangedEventHandler DownloadProgressChangedEvent;

        public void DownloadFile(string url, string path)
        {
            var req = WebRequest.CreateHttp(url); // Request
            var response = req.GetResponse();
            var filesize = long.Parse(response.Headers.Get("Content-Length")); // Get File Length

            var webStream = response.GetResponseStream(); // Get NetworkStream
            var fileStream = File.Open(path, FileMode.Create); // Open FileStream

            var bufferSize = DefaultBufferSize; // Make buffer
            var buffer = new byte[bufferSize];
            var length = 0;

            var processedBytes = 0;

            while ((length = webStream.Read(buffer, 0, bufferSize)) > 0) // read to end and write file
            {
                fileStream.Write(buffer, 0, length);

                // raise event
                processedBytes += length;
                ProgressChanged(processedBytes, filesize);
            }

            buffer = null;
            webStream.Dispose(); // Close streams
            fileStream.Dispose();
        }

        void ProgressChanged(long value, long max)
        {
            var percentage = ((float)value / max) * 100;

            var e = new ProgressChangedEventArgs((int)percentage, null);
            DownloadProgressChangedEvent?.Invoke(this, e);
        }
    }
}
