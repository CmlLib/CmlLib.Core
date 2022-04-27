using System;
using System.IO;
using System.Threading.Tasks;
using CmlLib.Core.Version;
using CmlLib.Utils;

namespace CmlLib.Core.VersionMetadata
{
    public abstract class StringVersionMetadata : MVersionMetadata
    {
        protected StringVersionMetadata(string id) : base(id)
        {
            
        }
        
        protected abstract string ReadMetadata();
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

        protected virtual void SaveMetadata(string metadata, MinecraftPath path)
        {
            var metadataPath = prepareSaveMetadata(path);
            if (!string.IsNullOrEmpty(metadataPath))
                File.WriteAllText(metadataPath, metadata);
        }
        
        protected virtual Task SaveMetadataAsync(string metadata, MinecraftPath path)
        {
            var metadataPath = prepareSaveMetadata(path);
            if (!string.IsNullOrEmpty(metadataPath))
                return IOUtil.WriteFileAsync(metadataPath, metadata);
            else
                return Task.CompletedTask;
        }
        
        // note: sync flag
        // https://docs.microsoft.com/en-us/archive/msdn-magazine/2015/july/async-programming-brownfield-async-development#the-flag-argument-hack
        private async Task<MVersion?> getAsync(MinecraftPath? savePath, bool parse, bool sync)
        {
            string metadataJson;
            if (sync)
                metadataJson = ReadMetadata();
            else
                metadataJson = await ReadMetadataAsync().ConfigureAwait(false);

            if (savePath != null)
            {
                if (sync)
                    SaveMetadata(metadataJson, savePath);
                else
                    await SaveMetadataAsync(metadataJson, savePath).ConfigureAwait(false);
            }

            return parse ? MVersionParser.ParseFromJson(metadataJson) : null;
        }
        
        public override MVersion GetVersion()
            => getAsync(null, true, true).GetAwaiter().GetResult()!;
        

        public override MVersion GetVersion(MinecraftPath savePath)
            => getAsync(savePath, true, true).GetAwaiter().GetResult()!;

        public override Task<MVersion> GetVersionAsync()
            => getAsync(null, true, false)!;

        public override Task<MVersion> GetVersionAsync(MinecraftPath savePath)
            => getAsync(savePath, true, false)!;

        public override void Save(MinecraftPath path)
            => getAsync(path, false, true).GetAwaiter().GetResult();

        public override Task SaveAsync(MinecraftPath path)
            => getAsync(path, false, false);
    }
}