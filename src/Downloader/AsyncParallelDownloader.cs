using System.ComponentModel;
using System.Diagnostics;

namespace CmlLib.Core.Downloader;

public class AsyncParallelDownloader : IDownloader
{
    private readonly HttpClient _httpClient;

    public int MaxParallelism { get; private set; }
    public bool IgnoreInvalidFiles { get; set; } = true;

    private int totalFiles;
    private int progressedFiles;

    private long totalBytes;
    private long receivedBytes;

    private readonly object progressEventLock = new object();

    private bool isRunning;

    private IProgress<FileProgressChangedEventArgs>? pChangeProgress;
    private IProgress<DownloadFileChangedEventArgs>? pChangeFile;
    private IProgress<DownloadFileByteProgress> fileByteProgress;

    public AsyncParallelDownloader(HttpClient httpClient, int parallelism = 10)
    {
        _httpClient = httpClient;
        MaxParallelism = parallelism;
        fileByteProgress = new Progress<DownloadFileByteProgress>(byteProgressHandler);
    }

    public async Task DownloadFiles(DownloadFile[] files, 
        IProgress<DownloadFileChangedEventArgs>? fileProgress,
        IProgress<ProgressChangedEventArgs>? downloadProgress)
    {
        if (files.Length == 0)
            return;
        
        if (isRunning)
            throw new InvalidOperationException("already downloading");

        isRunning = true;

        pChangeFile = fileProgress;
        pChangeProgress = downloadProgress;
        
        totalFiles = files.Length;
        progressedFiles = 0;

        totalBytes = 0;
        receivedBytes = 0;

        foreach (DownloadFile item in files)
        {
            if (item.Size > 0)
                totalBytes += item.Size;
        }

        fileProgress?.Report(
            new DownloadFileChangedEventArgs(files[0].Type, this, null, files.Length, 0));
        await ForEachAsyncSemaphore(files, MaxParallelism, doDownload).ConfigureAwait(false);
        
        isRunning = false;
    }

    private async Task ForEachAsyncSemaphore<T>(IEnumerable<T> source,
        int degreeOfParallelism, Func<T, Task> body)
    {
        List<Task> tasks = new List<Task>();
        using SemaphoreSlim throttler = new SemaphoreSlim(degreeOfParallelism);
        foreach (var element in source)
        {
            await throttler.WaitAsync().ConfigureAwait(false);

            async Task work(T item)
            {
                try
                {
                    await body(item).ConfigureAwait(false);
                }
                finally
                {
                    throttler.Release();
                }
            }
            
            tasks.Add(work(element));
        }
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    private async Task doDownload(DownloadFile file)
    {
        try
        {
            await doDownload(file, 3).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (!IgnoreInvalidFiles)
                throw new MDownloadFileException("failed to download", ex, file);
        }
    }

    private async Task doDownload(DownloadFile file, int retry)
    {
        try
        {
            await HttpClientDownloadHelper.DownloadFileAsync(_httpClient, file, fileByteProgress)
                .ConfigureAwait(false);

            if (file.AfterDownload != null)
            {
                foreach (var item in file.AfterDownload)
                {
                    await item.Invoke().ConfigureAwait(false);
                }
            }

            Interlocked.Increment(ref progressedFiles);
            pChangeFile?.Report(
                new DownloadFileChangedEventArgs(file.Type, this, file.Name, totalFiles, progressedFiles));
        }
        catch (Exception ex)
        {
            if (retry <= 0)
                return;

            Debug.WriteLine(ex);
            retry--;

            await doDownload(file, retry).ConfigureAwait(false);
        }
    }

    private void byteProgressHandler(DownloadFileByteProgress progress)
    {
        lock (progressEventLock)
        {
            if (progress.File != null && progress.File.Size <= 0)
            {
                totalBytes += progress.TotalBytes;
                progress.File.Size = progress.TotalBytes;
            }

            receivedBytes += progress.ProgressedBytes;

            if (receivedBytes > totalBytes)
                return;

            double percent = (double)receivedBytes / totalBytes * 100;
            pChangeProgress?.Report(new FileProgressChangedEventArgs(totalBytes, receivedBytes, (int)percent));
        }
    }
}
