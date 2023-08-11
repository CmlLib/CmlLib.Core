using CmlLib.Utils;

namespace CmlLib.Core.Downloader;

internal struct DownloadFileByteProgress
{
    public DownloadFile? File { get; set; }
    public long TotalBytes { get; set; }
    public long ProgressedBytes { get; set; }
}

// To implement System.Net.WebClient.DownloadFileTaskAsync
// https://source.dot.net/#System.Net.WebClient/System/Net/WebClient.cs,c2224e40a6960929
internal static class HttpClientDownloadHelper
{
    private const int DefaultDownloadBufferLength = 65536;

    public static async Task DownloadFileAsync(
        this HttpClient httpClient,
        DownloadFile file,
        IProgress<DownloadFileByteProgress>? progress = null, 
        CancellationToken cancellationToken = default)
    {
        IOUtil.CreateParentDirectory(file.Path);
        using var destination = File.Create(file.Path);
        await DownloadFileAsync(
            httpClient,
            file, 
            destination, 
            progress, 
            cancellationToken)
            .ConfigureAwait(false);
    }

    public static async Task DownloadFileAsync(
        this HttpClient httpClient,
        DownloadFile file, 
        Stream destination,
        IProgress<DownloadFileByteProgress>? progress = null, 
        CancellationToken cancellationToken = default)
    {
        // Get the http headers first to examine the content length
        using var response = await httpClient.GetAsync(
            file.Url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        var contentLength = response.Content.Headers.ContentLength ?? file.Size;
        using var download = await response.Content.ReadAsStreamAsync();

        if (download.CanTimeout)
            download.ReadTimeout = 10000;

        // Ignore progress reporting when no progress reporter was 
        // passed or when the content length is unknown
        if (progress == null)
        {
            await download.CopyToAsync(destination);
            return;
        }
        
        var bufferSize = (contentLength == -1 || contentLength > DefaultDownloadBufferLength)
            ? DefaultDownloadBufferLength
            : contentLength;
        var copyBuffer = new byte[bufferSize];

        while (true)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            int bytesRead = await download.ReadAsync(
                copyBuffer, 
                0, 
                copyBuffer.Length)
                .ConfigureAwait(false);

            if (bytesRead == 0)
                break;

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
