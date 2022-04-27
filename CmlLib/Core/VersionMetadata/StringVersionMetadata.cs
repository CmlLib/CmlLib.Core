using System;
using System.IO;
using System.Threading.Tasks;
using CmlLib.Core.Version;
using CmlLib.Utils;

namespace CmlLib.Core.VersionMetadata
{
    /// <summary>
    /// Represent JSON text based version metadata
    /// </summary>
    public abstract class StringVersionMetadata : MVersionMetadata
    {
        protected StringVersionMetadata(string name) : base(name)
        {
            
        }
        
        /// <summary>
        /// Get actual version data as string
        /// </summary>
        /// <returns>Version metadata</returns>
        protected abstract Task<string> ReadVersionDataAsync();
        
        /// <summary>
        /// Save version data into a file. It does not overwrite file
        /// </summary>
        /// <param name="metadata">The content of version metadata</param>
        /// <param name="path">Game directory</param>
        /// <returns></returns>
        protected virtual Task SaveVersionDataAsync(string metadata, MinecraftPath path)
        {
            var metadataPath = prepareSaveVersiondata(path);
            if (!string.IsNullOrEmpty(metadataPath))
                return IOUtil.WriteFileAsync(metadataPath, metadata);
            else
                return Task.CompletedTask;
        }

        private string? prepareSaveVersiondata(MinecraftPath path)
        {
            if (string.IsNullOrEmpty(Name))
                return null;

            var metadataPath = path.GetVersionJsonPath(Name);

            // check if target path and current path are same to prevent overwriting
            if (IsLocalVersion && !string.IsNullOrEmpty(Path))
            {
                var result = string.Compare(IOUtil.NormalizePath(Path), metadataPath,
                    StringComparison.InvariantCultureIgnoreCase);

                if (result == 0) // same path
                    return null;
            }

            var directoryPath = System.IO.Path.GetDirectoryName(metadataPath);
            if (!string.IsNullOrEmpty(directoryPath))
                Directory.CreateDirectory(directoryPath);

            return metadataPath;
        }

        private async Task<MVersion?> getAsync(MinecraftPath? savePath, bool parse)
        {
            string metadataJson;
            metadataJson = await ReadVersionDataAsync().ConfigureAwait(false);

            if (savePath != null)
                await SaveVersionDataAsync(metadataJson, savePath).ConfigureAwait(false);

            return parse ? MVersionParser.ParseFromJson(metadataJson) : null;
        }

        public override Task<MVersion> GetVersionAsync()
            => getAsync(null, true)!;

        public override Task<MVersion> GetVersionAsync(MinecraftPath savePath)
            => getAsync(savePath, true)!;

        public override Task SaveAsync(MinecraftPath path)
            => getAsync(path, false);
    }
}