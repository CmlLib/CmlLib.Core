using CmlLib.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmlLib.Core.Version;
using System.Threading;
using CmlLib.Core.Downloader;

namespace CmlLib.Core.Files
{
    public sealed class LibraryChecker : IFileChecker
    {
        IProgress<DownloadFileChangedEventArgs> pChangeFile;
        public event DownloadFileChangedHandler ChangeFile;

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

        public DownloadFile[] CheckFiles(MinecraftPath path, MVersion version)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));

            return CheckFilesTaskAsync(path, version.Libraries).GetAwaiter().GetResult();
        }

        public DownloadFile[] CheckFiles(MinecraftPath path, MLibrary[] libs)
        {
            if (libs == null)
                throw new ArgumentNullException(nameof(libs));

            return CheckFilesTaskAsync(path, libs).GetAwaiter().GetResult();
        }

        public Task<DownloadFile[]> CheckFilesTaskAsync(MinecraftPath path, MVersion version)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));

            return CheckFilesTaskAsync(path, version.Libraries);
        }

        [MethodTimer.Time]
        public async Task<DownloadFile[]> CheckFilesTaskAsync(MinecraftPath path, MLibrary[] libs)
        {
            if (libs == null)
                throw new ArgumentNullException(nameof(libs));

            pChangeFile = new Progress<DownloadFileChangedEventArgs>(
                (e) => ChangeFile?.Invoke(e));

            string lastLibraryName = "";
            int progressed = 0;
            var files = new List<DownloadFile>(libs.Length);
            foreach (MLibrary library in libs)
            {
                var downloadRequire = await CheckDownloadRequireAsync(path, library);

                pChangeFile.Report(new DownloadFileChangedEventArgs(
                    MFile.Library, library.Name, libs.Length, progressed));

                if (downloadRequire)
                {
                    files.Add(new DownloadFile
                    {
                        Type = MFile.Library,
                        Name = library.Name,
                        Path = Path.Combine(path.Library, library.Path),
                        Size = library.Size,
                        Url = CreateDownloadUrl(library)
                    });

                    lastLibraryName = library.Name;
                }
                Interlocked.Increment(ref progressed);
            }

            ChangeFile?.Invoke(new DownloadFileChangedEventArgs(
                MFile.Library, lastLibraryName, libs.Length, libs.Length));

            return files.Distinct().ToArray();
        }

        private string CreateDownloadUrl(MLibrary lib)
        {
            var url = lib.Url;

            if (url == null)
                url = LibraryServer + lib.Path;
            else if (url == "")
                url = null;
            else if (url.Split('/').Last() == "")
                url += lib.Path.Replace("\\", "/");

            return url;
        }

        private async Task<bool> CheckDownloadRequireAsync(MinecraftPath path, MLibrary lib)
        {
            return lib.IsRequire
                && !string.IsNullOrEmpty(lib.Path)
                && !await IOUtil.CheckFileValidationAsync(Path.Combine(path.Library, lib.Path), lib.Hash, CheckHash).ConfigureAwait(true);
        }
    }
}
