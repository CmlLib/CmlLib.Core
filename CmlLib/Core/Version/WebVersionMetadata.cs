using System.IO;
using System.Net;

namespace CmlLib.Core.Version
{
    public class WebVersionMetadata : MVersionMetadata
    {
        public WebVersionMetadata(string id) : base(id)
        {
            IsLocalVersion = false;
        }

        private string readMetadata()
        {
            using (var wc = new WebClient())
            {
                // below code will throw ArgumentNullException when Path is null
                var res = wc.DownloadString(Path);
                return res;
            }
        }

        private void saveMetadata(string json, MinecraftPath path)
        {
            if (string.IsNullOrEmpty(Name))
                return;

            var metadataPath = path.GetVersionJsonPath(Name);
            var directoryPath = System.IO.Path.GetDirectoryName(metadataPath);
            if (!string.IsNullOrEmpty(directoryPath))
                Directory.CreateDirectory(directoryPath);
            
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