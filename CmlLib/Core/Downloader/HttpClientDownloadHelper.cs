using CmlLib.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CmlLib.Core.Downloader
{
    internal class DownloadFileByteProgress
    {
        public DownloadFile? File { get; set; }
        public long TotalBytes { get; set; }
        public long ProgressedBytes { get; set; }
    }

    // To implement System.Net.WebClient.DownloadFileTaskAsync
    // https://source.dot.net/#System.Net.WebClient/System/Net/WebClient.cs,c2224e40a6960929
    internal class HttpClientDownloadHelper
    {
        private const int DefaultCopyBufferLength = 8192;
        private const int DefaultDownloadBufferLength = 65536;

        public HttpClientDownloadHelper(HttpClient client) 
        {
            this.httpClient = client;
        }

        private readonly HttpClient httpClient;

        public async Task DownloadFileAsync(DownloadFile file,
            IProgress<DownloadFileByteProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            IOUtil.CreateParentDirectory(file.Path);
            using var destination = IOUtil.AsyncWriteStream(file.Path, false);
            await DownloadFileAsync(file, destination, progress, cancellationToken).ConfigureAwait(false);
        }

        public async Task DownloadFileAsync(DownloadFile file, Stream destination,
            IProgress<DownloadFileByteProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            // Get the http headers first to examine the content length
            using var response = await httpClient.GetAsync(file.Url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            var contentLength = response.Content.Headers.ContentLength ?? file.Size;

            using var download = await response.Content.ReadAsStreamAsync();

            // Ignore progress reporting when no progress reporter was 
            // passed or when the content length is unknown
            if (progress == null)
            {
                await download.CopyToAsync(destination);
                return;
            }

            byte[] copyBuffer = new byte[contentLength == -1 || contentLength > DefaultDownloadBufferLength ? DefaultDownloadBufferLength : contentLength];
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                int bytesRead = await download.ReadAsync(copyBuffer, 0, copyBuffer.Length).ConfigureAwait(false);
                if (bytesRead == 0)
                    break;

                //await writeStream.WriteAsync(new ReadOnlyMemory<byte>(copyBuffer, 0, bytesRead)).ConfigureAwait(false);
                //await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                await destination.WriteAsync(copyBuffer, 0, bytesRead).ConfigureAwait(false);

                progress?.Report(new DownloadFileByteProgress()
                {
                    File = file,
                    TotalBytes = contentLength,
                    ProgressedBytes = bytesRead
                });
            }
        }
    }
}
