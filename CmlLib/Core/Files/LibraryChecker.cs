using CmlLib.Core.Downloader;
using CmlLib.Core.Version;
using CmlLib.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CmlLib.Core.Files
{
    public sealed class LibraryChecker : IFileChecker
    {
        private string libServer = MojangServer.Library;
        public string LibraryServer
        {
            get => libServer;
            set
            {
                if (value.Last() == '/')
                    libServer = value;
                else
                    libServer = value + "/";
            }
        }
        public bool CheckHash { get; set; } = true;

        public DownloadFile[] CheckFiles(MinecraftPath path, MVersion version,
            IProgress<DownloadFileChangedEventArgs> progress)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));
            return CheckFiles(path, version.Libraries, progress);
        }

        public Task<DownloadFile[]> CheckFilesTaskAsync(MinecraftPath path, MVersion version,
            IProgress<DownloadFileChangedEventArgs> progress)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));
            
            return Task.Run(() => checkLibraries(path, version.Libraries, progress));
        }

        public DownloadFile[] CheckFiles(MinecraftPath path, MLibrary[] libs,
            IProgress<DownloadFileChangedEventArgs> progress)
        {
            return checkLibraries(path, libs, progress);
        }
        
        [MethodTimer.Time]
        private DownloadFile[] checkLibraries(MinecraftPath path, MLibrary[] libs,
            IProgress<DownloadFileChangedEventArgs> progress)
        {
            if (libs == null)
                throw new ArgumentNullException(nameof(libs));
            
            int progressed = 0;
            var files = new List<DownloadFile>(libs.Length);
            foreach (MLibrary library in libs)
            {
                bool downloadRequire = checkDownloadRequire(path, library);
                
                if (downloadRequire)
                {
                    files.Add(new DownloadFile
                    {
                        Type = MFile.Library,
                        Name = library.Name,
                        Path = Path.Combine(path.Library, library.Path),
                        Size = library.Size,
                        Url = createDownloadUrl(library)
                    });
                }

                progressed++;
                progress?.Report(new DownloadFileChangedEventArgs(
                    MFile.Library, library.Name, libs.Length, progressed));
            }
            return files.Distinct().ToArray();
        }

        private string createDownloadUrl(MLibrary lib)
        {
            string url = lib.Url;

            if (url == null)
                url = LibraryServer + lib.Path;
            else if (url == "")
                url = null;
            else if (url.Split('/').Last() == "")
                url += lib.Path.Replace("\\", "/");

            return url;
        }

        private bool checkDownloadRequire(MinecraftPath path, MLibrary lib)
        {
            return lib.IsRequire
                   && !string.IsNullOrEmpty(lib.Path)
                   && !IOUtil.CheckFileValidation(Path.Combine(path.Library, lib.Path), lib.Hash, CheckHash);
        }
        
        private async Task<bool> checkDownloadRequireAsync(MinecraftPath path, MLibrary lib)
        {
            return lib.IsRequire
                && !string.IsNullOrEmpty(lib.Path)
                && !await IOUtil.CheckFileValidationAsync(Path.Combine(path.Library, lib.Path), lib.Hash, CheckHash)
                    .ConfigureAwait(false);
        }
    }
}
