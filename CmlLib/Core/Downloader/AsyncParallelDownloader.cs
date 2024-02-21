using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CmlLib.Utils;

namespace CmlLib.Core.Downloader;

public class AsyncParallelDownloader : IDownloader
{
    private readonly object progressEventLock = new();

    private bool isRunning;
    private IProgress<DownloadFileChangedEventArgs>? pChangeFile;

    private IProgress<FileProgressChangedEventArgs>? pChangeProgress;
    private int progressedFiles;
    private long receivedBytes;

    private long totalBytes;

    private int totalFiles;

    public AsyncParallelDownloader() : this(10)
    {
    }

    public AsyncParallelDownloader(int parallelism)
    {
        MaxThread = parallelism;
    }

    public int MaxThread { get; }
    public bool IgnoreInvalidFiles { get; set; } = true;

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

        foreach (var item in files)
            if (item.Size > 0)
                totalBytes += item.Size;

        fileProgress?.Report(
            new DownloadFileChangedEventArgs(files[0].Type, this, null, files.Length, 0));
        await ForEachAsyncSemaphore(files, MaxThread, doDownload).ConfigureAwait(false);

        isRunning = false;
    }

    private async Task ForEachAsyncSemaphore<T>(IEnumerable<T> source,
        int degreeOfParallelism, Func<T, Task> body)
    {
        var tasks = new List<Task>();
        using var throttler = new SemaphoreSlim(degreeOfParallelism);
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
            var downloader = new WebDownload();
            downloader.FileDownloadProgressChanged += Downloader_FileDownloadProgressChanged;

            await downloader.DownloadFileAsync(file).ConfigureAwait(false);

            if (file.AfterDownload != null)
                foreach (var item in file.AfterDownload)
                    await item.Invoke().ConfigureAwait(false);

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

    private void Downloader_FileDownloadProgressChanged(object? sender, DownloadFileProgress e)
    {
        lock (progressEventLock)
        {
            if (e.File.Size <= 0)
            {
                totalBytes += e.TotalBytes;
                e.File.Size = e.TotalBytes;
            }

            receivedBytes += e.ProgressedBytes;

            if (receivedBytes > totalBytes)
                return;

            var percent = (float)receivedBytes / totalBytes * 100;
            if (percent >= 0)
                pChangeProgress?.Report(new FileProgressChangedEventArgs(totalBytes, receivedBytes, (int)percent));
            else
                Debug.WriteLine(
                    $"total: {totalBytes} received: {receivedBytes} filename: {e.File.Name} filesize: {e.File.Size}");
        }
    }
}
