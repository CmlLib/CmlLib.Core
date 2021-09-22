using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmlLib.Utils
{
    internal static class IOUtil
    {
        private const int DefaultBufferSize = 4096;

        public static string NormalizePath(string path)
        {
            return Path.GetFullPath(path)
                .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
                .TrimEnd(Path.DirectorySeparatorChar);
        }
        
        public static void DeleteDirectory(string targetDir)
        {
            try
            {
                string[] files = Directory.GetFiles(targetDir);
                string[] dirs = Directory.GetDirectories(targetDir);

                foreach (string file in files)
                {
                    File.Delete(file);
                }

                foreach (string dir in dirs)
                {
                    DeleteDirectory(dir);
                }

                Directory.Delete(targetDir, true);
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

            copyDirectoryFiles(org, des, "", overwrite);
        }

        private static void copyDirectoryFiles(string org, string des, string path, bool overwrite)
        {
            var orgpath = Path.Combine(org, path);
            var orgdir = new DirectoryInfo(orgpath);

            var despath = Path.Combine(des, path);
            if (!Directory.Exists(despath))
                Directory.CreateDirectory(despath);

            foreach (var dir in orgdir.GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                var innerpath = Path.Combine(path, dir.Name);
                copyDirectoryFiles(org, des, innerpath, overwrite);
            }

            foreach (var file in orgdir.GetFiles("*", SearchOption.TopDirectoryOnly))
            {
                var innerpath = Path.Combine(path, file.Name);
                var desfile = Path.Combine(des, innerpath);

                file.CopyTo(desfile, overwrite);
            }
        }

        public static bool CheckSHA1(string path, string? compareHash)
        {
            if (string.IsNullOrEmpty(compareHash))
                return true;
            
            try
            {
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

        public static bool CheckFileValidation(string path, string? hash, bool checkHash)
        {
            if (!File.Exists(path))
                return false;

            if (!checkHash)
                return true;
            else
                return CheckSHA1(path, hash);
        }

        public static async Task<bool> CheckFileValidationAsync(string path, string? hash, bool checkHash)
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

        #region Async File IO
        
        // from .NET Framework reference source code
        // If we use the path-taking constructors we will not have FileOptions.Asynchronous set and
        // we will have asynchronous file access faked by the thread pool. We want the real thing.
        public static FileStream AsyncReadStream(string path)
        {
            FileStream stream = new FileStream(
                path, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize,
                FileOptions.Asynchronous | FileOptions.SequentialScan);

            return stream;
        }
        
        public static FileStream AsyncWriteStream(string path, bool append)
        {
            FileStream stream = new FileStream(
                path, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read, DefaultBufferSize,
                FileOptions.Asynchronous | FileOptions.SequentialScan);

            return stream;
        }
        
        public static StreamReader AsyncStreamReader(string path, Encoding encoding)
        {
            FileStream stream = AsyncReadStream(path);
            return new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks: false);
        }

        public static StreamWriter AsyncStreamWriter(string path, Encoding encoding, bool append)
        {
            FileStream stream = AsyncWriteStream(path, append);
            return new StreamWriter(stream, encoding);
        }

        // In .NET Standard 2.0, There is no File.ReadFileTextAsync. so I copied it from .NET Core source code
        public static async Task<string> ReadFileAsync(string path)
        {
            using var reader = AsyncStreamReader(path, Encoding.UTF8);
            var content = await reader.ReadToEndAsync()
                .ConfigureAwait(false); // **MUST be awaited in this scope**
            await reader.BaseStream.FlushAsync().ConfigureAwait(false);
            return content;
        }

        // In .NET Standard 2.0, There is no File.WriteFileTextAsync. so I copied it from .NET Core source code
        public static async Task WriteFileAsync(string path, string content)
        {
            // UTF8 with BOM might not be recognized by minecraft. not tested
            var encoder = new UTF8Encoding(false);
            using var writer = AsyncStreamWriter(path, encoder, false);
            await writer.WriteAsync(content).ConfigureAwait(false); // **MUST be awaited in this scope**
            await writer.FlushAsync().ConfigureAwait(false);
        }
        
        public static async Task CopyFileAsync(string sourceFile, string destinationFile)
        {
            using var sourceStream = AsyncReadStream(sourceFile);
            using var destinationStream = AsyncWriteStream(destinationFile, false);
            
            await sourceStream.CopyToAsync(destinationStream).ConfigureAwait(false);
        
            await destinationStream.FlushAsync().ConfigureAwait(false);
        }

        #endregion
    }
}
