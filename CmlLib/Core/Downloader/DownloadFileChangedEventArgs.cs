using System;

namespace CmlLib.Core.Downloader
{
    public delegate void DownloadFileChangedHandler(DownloadFileChangedEventArgs e);

    public enum MFile { Runtime, Library, Resource, Minecraft, Others }

    public class DownloadFileChangedEventArgs : EventArgs
    {
        public DownloadFileChangedEventArgs(MFile type, object source, string? filename, int total, int progressed)
        {
            FileKind = type;
            FileType = type;
            FileName = filename;
            TotalFileCount = total;
            ProgressedFileCount = progressed;
            Source = source;
        }

        // FileType and FileKind is exactly same.
        public MFile FileType { get; private set; }
        public MFile FileKind { get; private set; }
        public string? FileName { get; private set; }
        public int TotalFileCount { get; private set; }
        public int ProgressedFileCount { get; private set; }
        public object Source { get; private set; }
    }
}
