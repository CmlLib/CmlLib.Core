using System;
using System.IO;
using CmlLib.Utils;

namespace CmlLib.Core.Version
{
    public class LocalVersionMetadata : MVersionMetadata
    {
        public LocalVersionMetadata(string id) : base(id)
        {
            IsLocalVersion = true;
        }

        private string readMetadata()
        {
            // FileNotFoundException will be thrown if Path does not exist.
            return File.ReadAllText(Path);
        }
        
        private void saveMetadata(string json, MinecraftPath path)
        {
            if (string.IsNullOrEmpty(Name))
                return;

            var metadataPath = path.GetVersionJsonPath(Name);

            // check if target path and current path are same 
            if (!string.IsNullOrEmpty(Path))
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
            File.WriteAllText(metadataPath, json);
        }
        
        public override MVersion GetVersion()
        {
            var metadataJson = readMetadata();
            return MVersionParser.ParseFromJson(metadataJson);
        }

        public override MVersion GetVersion(MinecraftPath savePath)
        {
            var metadataJson = readMetadata();
            saveMetadata(metadataJson, savePath);
            return MVersionParser.ParseFromJson(metadataJson);
        }

        public override void Save(MinecraftPath path)
        {
            var metadataJson = readMetadata();
            saveMetadata(metadataJson, path);
        }
    }
}