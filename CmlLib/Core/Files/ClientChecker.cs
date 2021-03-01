using CmlLib.Core.Downloader;
using CmlLib.Core.Version;
using CmlLib.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CmlLib.Core.Files
{
    public sealed class ClientChecker : IFileChecker
    {
        public event DownloadFileChangedHandler ChangeFile;

        public bool CheckHash { get; set; } = true;

        public DownloadFile[] CheckFiles(MinecraftPath path, MVersion version)
        {
            return CheckFilesTaskAsync(path, version).GetAwaiter().GetResult();
        }

        public async Task<DownloadFile[]> CheckFilesTaskAsync(MinecraftPath path, MVersion version)
        {
            if (string.IsNullOrEmpty(version.ClientDownloadUrl)) return null;

            string id = version.Jar;
            string clientPath = path.GetVersionJarPath(id);

            if (!await IOUtil.CheckFileValidationAsync(clientPath, version.ClientHash))
            {
                ChangeFile?.Invoke(new DownloadFileChangedEventArgs(MFile.Minecraft, id, 1, 1));

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
    }
}
