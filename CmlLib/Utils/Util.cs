using System;
using System.IO;

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
    }
}
