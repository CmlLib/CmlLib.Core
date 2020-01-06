using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CmlLib.Utils
{
    public class SharpZip
    {
        public SharpZip(string path)
        {
            this.ZipPath = path;
        }

        public event EventHandler<int> ProgressEvent;
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
                        IOUtil.CopyStreamToFile(s, zfile, 1024 * 1024);

                    //Console.WriteLine(zfile);
                    ev(s.Position, length);
                }
            }
        }

        int previousPerc = 0;
        void ev(long curr, long total)
        {
            if (ProgressEvent == null)
                return;

            var progress = (int)((double)curr / (double)total * 100);
            if (previousPerc != progress)
            {
                previousPerc = progress;
                ProgressEvent.Invoke(this, progress);
            }
        }
    }
}
