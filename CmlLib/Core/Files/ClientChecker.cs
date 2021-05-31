using System;
using CmlLib.Core.Downloader;
using CmlLib.Core.Version;
using CmlLib.Utils;
using System.Threading.Tasks;

namespace CmlLib.Core.Files
{
    public sealed class ClientChecker : IFileChecker
    {
        public bool CheckHash { get; set; } = true;

        public DownloadFile[] CheckFiles(MinecraftPath path, MVersion version,
            IProgress<DownloadFileChangedEventArgs> progress)
        {
            progress?.Report(new DownloadFileChangedEventArgs(MFile.Minecraft, version.Jar, 1, 0));
            DownloadFile result = checkClientFile(path, version);
            progress?.Report(new DownloadFileChangedEventArgs(MFile.Minecraft, version.Jar, 1, 1));

            if (result == null)
                return null;
            else
                return new [] { result };
        }

        public async Task<DownloadFile[]> CheckFilesTaskAsync(MinecraftPath path, MVersion version,
            IProgress<DownloadFileChangedEventArgs> progress)
        {
            progress?.Report(new DownloadFileChangedEventArgs(MFile.Minecraft, version.Jar, 1, 0));
            DownloadFile result = await Task.Run(() => checkClientFile(path, version))
                .ConfigureAwait(false);
            progress?.Report(new DownloadFileChangedEventArgs(MFile.Minecraft, version.Jar, 1, 1));

            if (result == null)
                return null;
            else
                return new [] { result };
        }

        private DownloadFile checkClientFile(MinecraftPath path, MVersion version)
        {
            if (string.IsNullOrEmpty(version.ClientDownloadUrl)) return null;

            string id = version.Jar;
            string clientPath = path.GetVersionJarPath(id);

            if (!IOUtil.CheckFileValidation(clientPath, version.ClientHash, CheckHash))
            {
                return new DownloadFile
                {
                    Type = MFile.Minecraft,
                    Name = id,
                    Path = clientPath,
                    Url = version.ClientDownloadUrl
                };
            }
            else
                return null;
        }
    }
}
