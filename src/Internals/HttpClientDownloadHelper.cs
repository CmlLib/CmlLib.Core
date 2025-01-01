namespace CmlLib.Core.Internals;

// To implement System.Net.WebClient.DownloadFileTaskAsync
// https://source.dot.net/#System.Net.WebClient/System/Net/WebClient.cs,c2224e40a6960929
internal static class HttpClientDownloadHelper
{
    private const int DefaultDownloadBufferLength = 65536;

    public static async Task DownloadFileAsync(
        HttpClient httpClient,
        string url, 
        long size, 
        string path,
        IProgress<ByteProgress>? progress = null, 
        CancellationToken cancellationToken = default)
    {
        IOUtil.CreateParentDirectory(path);
        using var destination = File.Create(path);
        await DownloadFileAsync(
            httpClient,
            url, 
            size,
            destination, 
            progress, 
            cancellationToken)
            .ConfigureAwait(false);
    }

    public static async Task DownloadFileAsync(
        HttpClient httpClient,
        string url, 
        long size,
        Stream destination,
        IProgress<ByteProgress>? progress = null, // it must report last progress (100%)
        CancellationToken cancellationToken = default)
    {
        // Get the http headers first to examine the content length
        using var response = await httpClient.GetAsync(
            url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        response.EnsureSuccessStatusCode();

        var contentLength = response.Content.Headers.ContentLength ?? size;
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

        long totalRead = 0;
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

            totalRead += bytesRead;
            progress?.Report(new ByteProgress(contentLength, totalRead));
        }
    }
}
