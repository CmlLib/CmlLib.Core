using System;
using System.Threading.Tasks;
using CmlLib.Core.Downloader;
using CmlLib.Core.Version;
using CmlLib.Utils;

namespace CmlLib.Core.FileChecker
{
    public sealed class LogChecker : IFileChecker
    {
        public bool CheckHash { get; set; } = true;

        public DownloadFile[]? CheckFiles(MinecraftPath path, MVersion version,
            IProgress<DownloadFileChangedEventArgs>? progress)
        {
            return internalCheckLogFile(path, version, progress, async: false).GetAwaiter().GetResult();
        }

        public Task<DownloadFile[]?> CheckFilesTaskAsync(MinecraftPath path, MVersion version,
            IProgress<DownloadFileChangedEventArgs>? progress)
        {
            return internalCheckLogFile(path, version, progress, async: true);
        }

        private async Task<DownloadFile[]?> internalCheckLogFile(MinecraftPath path, MVersion version,
            IProgress<DownloadFileChangedEventArgs>? progress, bool async)
        {
            if (version.LoggingClient == null)
                return null;

            DownloadFile? result;

            progress?.Report(new DownloadFileChangedEventArgs(
                MFile.Others, this, version.LoggingClient?.LogFile?.Id, 1, 0));
            if (async)
            {
                result = await Task.Run(() => internalCheckLogFile(path, version))
                    .ConfigureAwait(false);
            }
            else
            {
                result = internalCheckLogFile(path, version);
            }
            progress?.Report(new DownloadFileChangedEventArgs(
                MFile.Others, this, version.LoggingClient?.LogFile?.Id, 1, 1));

            if (result == null)
                return null;
            else
                return new[] { result };
        }

        private DownloadFile? internalCheckLogFile(MinecraftPath path, MVersion version)
        {
            if (version.LoggingClient == null)
                return null;
            
            var url = version.LoggingClient?.LogFile?.Url;
            if (string.IsNullOrEmpty(url))
                return null;
            
            var id = version.LoggingClient?.LogFile?.Id ?? version.Id;
            var clientPath = path.GetLogConfigFilePath(id);

            if (!IOUtil.CheckFileValidation(clientPath, version.LoggingClient?.LogFile?.Sha1, CheckHash))
            {
                return new DownloadFile(clientPath, url)
                {
                    Type = MFile.Others,
                    Name = id
                };
            }
            
            return null;
        }
    }
}
