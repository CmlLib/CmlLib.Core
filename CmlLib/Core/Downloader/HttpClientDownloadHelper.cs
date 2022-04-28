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
    internal class HttpClientDownloadHelper
    {
        public HttpClientDownloadHelper(HttpClient client) 
        {
            this.httpClient = client;
        }

        private readonly HttpClient httpClient;

        public int BufferSize { get; set; } = 81920;

        public Task DownloadFileAsync(DownloadFile file,
            IProgress<DownloadFileByteProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            IOUtil.CreateParentDirectory(file.Path);
            var destination = File.Create(file.Path);
            return DownloadFileAsync(file, destination, progress, cancellationToken);
        }

        public async Task DownloadFileAsync(DownloadFile file, Stream destination,
            IProgress<DownloadFileByteProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            // Get the http headers first to examine the content length
            using var response = await httpClient.GetAsync(file.Url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            var fileSize = response.Content.Headers.ContentLength ?? file.Size;

            using var download = await response.Content.ReadAsStreamAsync();

            // Ignore progress reporting when no progress reporter was 
            // passed or when the content length is unknown
            if (progress == null)
            {
                await download.CopyToAsync(destination);
                return;
            }

            var buffer = new byte[BufferSize];
            int bytesRead;
            while ((bytesRead = await download.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                progress?.Report(new DownloadFileByteProgress(
                    file: file,
                    total: fileSize,
                    progressed: bytesRead));
            }

            progress?.Report(new DownloadFileByteProgress(file, fileSize, fileSize));
        }
    }
}
