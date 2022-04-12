using System;
using System.IO;
using System.Threading.Tasks;
using CmlLib.Core.Version;
using CmlLib.Utils;

namespace CmlLib.Core.VersionMetadata
{
    public abstract class StringVersionMetadata : MVersionMetadata
    {
        protected StringVersionMetadata(string name) : base(name)
        {
            
        }
        
        protected abstract Task<string> ReadMetadataAsync();

        private string? prepareSaveMetadata(MinecraftPath path)
        {
            if (string.IsNullOrEmpty(Name))
                return null;

            var metadataPath = path.GetVersionJsonPath(Name);

            // check if target path and current path are same 
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
        
        protected virtual Task SaveMetadataAsync(string metadata, MinecraftPath path)
        {
            var metadataPath = prepareSaveMetadata(path);
            if (!string.IsNullOrEmpty(metadataPath))
                return IOUtil.WriteFileAsync(metadataPath, metadata);
            else
                return Task.CompletedTask;
        }
        
        private async Task<MVersion?> getAsync(MinecraftPath? savePath, bool parse)
        {
            string metadataJson;
            metadataJson = await ReadMetadataAsync().ConfigureAwait(false);

            if (savePath != null)
                await SaveMetadataAsync(metadataJson, savePath).ConfigureAwait(false);

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