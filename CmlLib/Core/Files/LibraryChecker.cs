using CmlLib.Core.Downloader;
using CmlLib.Core.Version;
using CmlLib.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CmlLib.Core.Files
{
    public sealed class LibraryChecker : IFileChecker
    {
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

            return CheckFiles(path, version.Libraries);
        }

        public DownloadFile[] CheckFiles(MinecraftPath path, MLibrary[] libs)
        {
            if (libs == null)
                throw new ArgumentNullException(nameof(libs));

            return libs
                .Where(lib => CheckDownloadRequire(path, lib))
                .Select(lib => new DownloadFile
                {
                    Type = MFile.Library,
                    Name = lib.Name,
                    Path = Path.Combine(path.Library, lib.Path),
                    Url = CreateDownloadUrl(lib)
                })
                .Distinct()
                .ToArray();
        }

        private string CreateDownloadUrl(MLibrary lib)
        {
            var url = lib.Url;

            if (url == null)
                url = LibraryServer + lib.Path;
            else if (url == "")
                url = null;
            else if (url.Split('/').Last() == "")
                url += lib.Path;

            return url;
        }

        private bool CheckDownloadRequire(MinecraftPath path, MLibrary lib)
        {
            return lib.IsRequire
                && !string.IsNullOrEmpty(lib.Path)
                && !string.IsNullOrEmpty(lib.Url)
                && !CheckFileValidation(Path.Combine(path.Library, lib.Path), lib.Hash);
        }

        private bool CheckFileValidation(string path, string hash)
        {
            if (!File.Exists(path))
                return false;

            if (!CheckHash)
                return true;
            else
                return IOUtil.CheckFileValidation(path, hash);
        }
    }
}
