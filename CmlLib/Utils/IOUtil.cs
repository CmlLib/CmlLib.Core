using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable InconsistentNaming

namespace CmlLib.Utils
{
    internal static class IOUtil
    {
        private const int DefaultBufferSize = 4096;

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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        public static string CombinePath(string[] paths)
        {
            return string.Join(Path.PathSeparator.ToString(),
                paths.Select(x =>
                {
                    string path = Path.GetFullPath(x);
                    if (path.Contains(' '))
                        return "\"" + path + "\"";
                    else
                        return path;
                }));
        }

        public static void CopyDirectory(string org, string des, bool overwrite)
        {
            var dir = new DirectoryInfo(org);
            if (!dir.Exists)
                return;

            CopyDirectoryFiles(org, des, "", overwrite);
        }

        private static void CopyDirectoryFiles(string org, string des, string path, bool overwrite)
        {
            var orgpath = Path.Combine(org, path);
            var orgdir = new DirectoryInfo(orgpath);

            var despath = Path.Combine(des, path);
            if (!Directory.Exists(despath))
                Directory.CreateDirectory(despath);

            foreach (var dir in orgdir.GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                var innerpath = Path.Combine(path, dir.Name);
                CopyDirectoryFiles(org, des, innerpath, overwrite);
            }

            foreach (var file in orgdir.GetFiles("*", SearchOption.TopDirectoryOnly))
            {
                var innerpath = Path.Combine(path, file.Name);
                var desfile = Path.Combine(des, innerpath);

                file.CopyTo(desfile, overwrite);
            }
        }

        // If we use the path-taking constructors we will not have FileOptions.Asynchronous set and
        // we will have asynchronous file access faked by the thread pool. We want the real thing.
        public static StreamReader AsyncStreamReader(string path, Encoding encoding)
        {
            FileStream stream = AsyncReadStream(path);
            return new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks: false);
        }

        public static FileStream AsyncReadStream(string path)
        {
            FileStream stream = new FileStream(
    path, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize,
    FileOptions.Asynchronous | FileOptions.SequentialScan);

            return stream;
        }

        private static StreamWriter AsyncStreamWriter(string path, Encoding encoding, bool append)
        {
            FileStream stream = AsyncWriteStream(path, append);
            return new StreamWriter(stream, encoding);
        }

        public static FileStream AsyncWriteStream(string path, bool append)
        {
            FileStream stream = new FileStream(
    path, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read, DefaultBufferSize,
    FileOptions.Asynchronous | FileOptions.SequentialScan);

            return stream;
        }

        public static async Task<string> ReadFileAsync(string path)
        {
            using (var reader = AsyncStreamReader(path, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync().ConfigureAwait(false);
            }
        }

        public static bool CheckSHA1(string path, string compareHash)
        {
            try
            {
                if (string.IsNullOrEmpty(compareHash))
                    return true;

                string fileHash;

                using (var file = File.OpenRead(path))
                using (var hasher = new System.Security.Cryptography.SHA1CryptoServiceProvider())
                {
                    var binaryHash = hasher.ComputeHash(file);
                    fileHash = BitConverter.ToString(binaryHash).Replace("-", "").ToLowerInvariant();
                }

                return fileHash == compareHash;
            }
            catch
            {
                return false;
            }
        }

        public static bool CheckFileValidation(string path, string hash, bool checkHash)
        {
            if (!File.Exists(path))
                return false;

            if (!checkHash)
                return true;
            else
                return CheckSHA1(path, hash);
        }

        public static async Task<bool> CheckFileValidationAsync(string path, string hash, bool checkHash)
        {
            if (!File.Exists(path))
                return false;

            if (!checkHash)
                return true;
            else
                return await Task.Run(() => CheckSHA1(path, hash)).ConfigureAwait(false);
        }

        public static bool CheckFileValidation(string path, string hash, long size)
        {
            var file = new FileInfo(path);
            return file.Exists && file.Length == size && CheckSHA1(path, hash);
        }

        public static async Task CopyFileAsync(string sourceFile, string destinationFile)
        {
            using (var sourceStream = AsyncReadStream(sourceFile))
            using (var destinationStream = AsyncWriteStream(destinationFile, false))
                await sourceStream.CopyToAsync(destinationStream).ConfigureAwait(false);
        }

        [DllImport("libc", SetLastError = true)]
        private static extern int chmod(string pathname, int mode);

        // user permissions
        private const int S_IRUSR = 0x100;
        private const int S_IWUSR = 0x80;
        private const int S_IXUSR = 0x40;

        // group permission
        private const int S_IRGRP = 0x20;
        private const int S_IWGRP = 0x10;
        private const int S_IXGRP = 0x8;

        // other permissions
        private const int S_IROTH = 0x4;
        private const int S_IWOTH = 0x2;
        private const int S_IXOTH = 0x1;

        public const int Chmod755 = S_IRUSR | S_IXUSR | S_IWUSR
                            | S_IRGRP | S_IXGRP
                            | S_IROTH | S_IXOTH;

        public static void Chmod(string path, int mode)
        {
            chmod(path, mode);
        }
    }
}
