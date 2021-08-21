using System;
using System.IO;
using System.Threading.Tasks;
using CmlLib.Utils;

namespace CmlLib.Core.Version
{
    public abstract class StringVersionMetadata : MVersionMetadata
    {
        public StringVersionMetadata(string id) : base(id)
        {
            
        }
        
        protected abstract Task<string> ReadMetadata(bool async);

        protected virtual async Task SaveMetadata(string metadata, MinecraftPath path, bool async)
        {
            if (string.IsNullOrEmpty(Name))
                return;

            var metadataPath = path.GetVersionJsonPath(Name);

            // check if target path and current path are same 
            if (IsLocalVersion && !string.IsNullOrEmpty(Path))
            {
                var result = string.Compare(IOUtil.NormalizePath(Path), metadataPath,
                    StringComparison.InvariantCultureIgnoreCase);
                
                if (result != 0) // same path
                    return;
            }
            
            var directoryPath = System.IO.Path.GetDirectoryName(metadataPath);
            if (!string.IsNullOrEmpty(directoryPath))
                Directory.CreateDirectory(directoryPath);
    
            // note: File.WriteAllText is faster than File.Copy
            if (async)
                await IOUtil.WriteFileAsync(metadataPath, metadata).ConfigureAwait(false);
            else
                File.WriteAllText(metadataPath, metadata);
        }
        
        private async Task<MVersion?> get(MinecraftPath? savePath, bool parse, bool async)
        {
            string metadataJson;
            if (async)
                metadataJson = await ReadMetadata(true).ConfigureAwait(false);
            else
                metadataJson = ReadMetadata(false).GetAwaiter().GetResult();

            if (savePath != null)
            {
                if (async)
                    await SaveMetadata(metadataJson, savePath, true).ConfigureAwait(false);
                else
                    SaveMetadata(metadataJson, savePath, false).GetAwaiter().GetResult();
            }

            return parse ? MVersionParser.ParseFromJson(metadataJson) : null;
        }
        
        public override MVersion GetVersion()
            => get(null, true, false).GetAwaiter().GetResult()!;
        

        public override MVersion GetVersion(MinecraftPath savePath)
            => get(savePath, true, false).GetAwaiter().GetResult()!;

        public override Task<MVersion> GetVersionAsync()
            => get(null, true, true)!;

        public override Task<MVersion> GetVersionAsync(MinecraftPath savePath)
            => get(savePath, true, true)!;

        public override void Save(MinecraftPath path)
            => get(path, false, false).GetAwaiter().GetResult();

        public override Task SaveAsync(MinecraftPath path)
            => get(path, false, true);
    }
}