using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

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
                    var path = Path.GetFullPath(x);
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

        public static bool CheckSHA1(string path, string compareHash)
        {
            try
            {
                if (string.IsNullOrEmpty(compareHash))
                    return true;

                var fileHash = "";

                using (var file = File.OpenRead(path))
                using (var hasher = new System.Security.Cryptography.SHA1CryptoServiceProvider())
                {
                    var binaryHash = hasher.ComputeHash(file);
                    fileHash = BitConverter.ToString(binaryHash).Replace("-", "").ToLower();
                }

                return fileHash == compareHash;
            }
            catch
            {
                return false;
            }
        }

        public static bool CheckFileValidation(string path, string hash)
        {
            return File.Exists(path) && CheckSHA1(path, hash);
        }

        public static async Task<bool> CheckFileValidationAsync(string path, string hash, bool checkHash=true)
        {
            if (!File.Exists(path))
                return false;

            if (!checkHash)
                return true;
            else
                return await Task.Run(() => CheckSHA1(path, hash));
        }

        public static bool CheckFileValidation(string path, string hash, long size)
        {
            var file = new FileInfo(path);
            return file.Exists && file.Length == size && CheckSHA1(path, hash);
        }

        public static async Task CopyFileAsync(string sourceFile, string destinationFile)
        {
            using (var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan))
            using (var destinationStream = new FileStream(destinationFile, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan))
                await sourceStream.CopyToAsync(destinationStream);
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
