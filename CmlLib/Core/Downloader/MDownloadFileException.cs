using System;

namespace CmlLib.Core.Installer
{
    public class MDownloadFileException : Exception
    {
        public MDownloadFileException(DownloadFile exFile)
            : this(null, null, exFile) { }

        public MDownloadFileException(string message, Exception innerException, DownloadFile exFile)
            : base(message, innerException)
        {
            ExceptionFile = exFile;
        }

        public DownloadFile ExceptionFile { get; private set; }
    }
}
