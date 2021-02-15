using CmlLib.Core.Downloader;
using CmlLib.Core.Version;
using CmlLib.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CmlLib.Core.Files
{
    public sealed class ClientChecker : IFileChecker
    {
        public event DownloadFileChangedHandler ChangeFile;

        public bool CheckHash { get; set; } = true;

        public DownloadFile[] CheckFiles(MinecraftPath path, MVersion version)
        {
            if (string.IsNullOrEmpty(version.ClientDownloadUrl)) return null;

            string id = version.Jar;
            string clientPath = path.GetVersionJarPath(id);

            if (!CheckFileValidation(clientPath, version.ClientHash))
            {
                return new DownloadFile[]
                {
                    new DownloadFile
                    {
                        Type = MFile.Minecraft,
                        Name = id,
                        Path = clientPath,
                        Url = version.ClientDownloadUrl
                    }
                };
            }
            else
                return null;
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
