using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;

namespace CmlLib.Utils
{
    internal class SharpZip
    {
        public SharpZip(string path)
        {
            this.ZipPath = path;
        }

        public event EventHandler<int>? ProgressEvent;
        public string ZipPath { get; private set; }

        public void Unzip(string path)
        {
            using (var fs = File.OpenRead(ZipPath))
            using (var s = new ZipInputStream(fs))
            {
                long length = fs.Length;
                ZipEntry e;
                while ((e = s.GetNextEntry()) != null)
                {
                    var zfile = Path.Combine(path, e.Name);

                    var dirName = Path.GetDirectoryName(zfile);
                    var fileName = Path.GetFileName(zfile);

                    if (!string.IsNullOrWhiteSpace(dirName))
                        Directory.CreateDirectory(dirName);

                    if (!string.IsNullOrWhiteSpace(fileName))
                    {
                        using (var zFileStream = File.OpenWrite(zfile))
                        {
                            s.CopyTo(zFileStream);
                        }
                    }

                    ev(s.Position, length);
                }
            }
        }

        private int previousPerc;

        private void ev(long curr, long total)
        {
            if (ProgressEvent == null)
                return;

            int progress = (int)(curr / (double)total * 100);
            if (previousPerc != progress)
            {
                previousPerc = progress;
                ProgressEvent.Invoke(this, progress);
            }
        }
    }
}
