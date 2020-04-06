using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CmlLib.Utils
{
    public class IOUtil
    {
        public static void DeleteDirectory(string target_dir)
        {
            try
            {
                string[] files = Directory.GetFiles(target_dir);
                string[] dirs = Directory.GetDirectories(target_dir);

                foreach (string file in files)
                {
                    File.Delete(file);
                }

                foreach (string dir in dirs)
                {
                    DeleteDirectory(dir);
                }

                Directory.Delete(target_dir, true);
            }
            catch
            {

            }
        }

        public static void CopyStreamToFile(Stream stream, string path, int bufferSize)
        {
            using (var fs = File.Create(path))
            {
                int length = 0;
                byte[] buffer = new byte[bufferSize];
                while ((length = stream.Read(buffer, 0, bufferSize)) > 0)
                {
                    fs.Write(buffer, 0, length);
                }
            }
        }

        [DllImport("libc", SetLastError = true)]
        private static extern int chmod(string pathname, int mode);

        // user permissions
        const int S_IRUSR = 0x100;
        const int S_IWUSR = 0x80;
        const int S_IXUSR = 0x40;

        // group permission
        const int S_IRGRP = 0x20;
        const int S_IWGRP = 0x10;
        const int S_IXGRP = 0x8;

        // other permissions
        const int S_IROTH = 0x4;
        const int S_IWOTH = 0x2;
        const int S_IXOTH = 0x1;

        public const int Chmod755 = S_IRUSR | S_IXUSR | S_IWUSR
                            | S_IRGRP | S_IXGRP
                            | S_IROTH | S_IXOTH;

        public static void Chmod(string path, int mode)
        {
            chmod(path, mode);
        }
    }
}
