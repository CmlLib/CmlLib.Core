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
    public class ModChecker : IFileChecker
    {
        public event DownloadFileChangedHandler ChangeFile;

        public bool CheckHash { get; set; } = true;
        public ModFile[] Mods;

        public DownloadFile[] CheckFiles(MinecraftPath path, MVersion version)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));

            int progressed = 0;
            var files = new List<DownloadFile>(Mods.Length);
            foreach (ModFile mod in Mods)
            {
                if (CheckDownloadRequire(path, mod))
                {
                    files.Add(new DownloadFile
                    {
                        Type = MFile.Others,
                        Name = mod.Name,
                        Path = Path.Combine(path.BasePath, mod.Path),
                        Url = mod.Url
                    });
                }

                progressed++;
                ChangeFile?.Invoke(new DownloadFileChangedEventArgs(
                    MFile.Others, mod.Name, Mods.Length, progressed));
            }

            return files.Distinct().ToArray();
        }

        private bool CheckDownloadRequire(MinecraftPath path, ModFile mod)
        {
            return !string.IsNullOrEmpty(mod.Url)
                && !string.IsNullOrEmpty(mod.Path)
                && !CheckFileValidation(Path.Combine(path.BasePath, mod.Path), mod.Hash);
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
