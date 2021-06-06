using System;

namespace CmlLib.Core.Downloader
{
    public delegate void DownloadFileChangedHandler(DownloadFileChangedEventArgs e);

    public enum MFile { Runtime, Library, Resource, Minecraft, Others }

    public class DownloadFileChangedEventArgs : EventArgs
    {
        public DownloadFileChangedEventArgs(MFile kind, bool isDownloader, string? filename, int total, int progressed)
        {
            FileKind = kind;
            FileName = filename;
            TotalFileCount = total;
            ProgressedFileCount = progressed;
            this.IsDownloader = isDownloader;
        }

        public MFile FileKind { get; private set; }
        public string? FileName { get; private set; }
        public int TotalFileCount { get; private set; }
        public int ProgressedFileCount { get; private set; }
        public bool IsDownloader { get; private set; }
    }
}
