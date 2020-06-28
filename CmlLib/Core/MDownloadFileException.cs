using System;
using System.Collections.Generic;
using System.Text;

namespace CmlLib.Core
{
    public class MDownloadFileException : Exception
    {
        public MDownloadFileException(DownloadFile exFile)
            : this(null, null, exFile) { }

        public MDownloadFileException(string message, Exception innerException, DownloadFile exFile)
            : base(message, innerException)
        {
            this.ExceptionFile = exFile;
        }

        public DownloadFile ExceptionFile { get; private set; }
    }
}
