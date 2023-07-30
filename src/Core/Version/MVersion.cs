using CmlLib.Core.Files;
using System.Linq;

namespace CmlLib.Core.Version
{
    public class MVersion
    {
        public MVersion(string id)
        {
            this.Id = id;
        }
        
        public bool IsInherited { get; set; }
        public string? ParentVersionId { get; set; }

        public string Id { get; set; }

        public MFileMetadata? Assets { get; set; }

        public string? JavaVersion { get; set; }
        public string? JavaBinaryPath { get; set; }
        public string? Jar { get; set; }
        public MFileMetadata? Client { get; set; }
        public MLibrary[]? Libraries { get; set; }
        public string? MainClass { get; set; }
        public string? MinecraftArguments { get; set; }
        public string[]? GameArguments { get; set; }
        public string[]? JvmArguments { get; set; }
        public string? ReleaseTime { get; set; }
        public MVersionType Type { get; set; } = MVersionType.Custom;
        public string? TypeStr { get; set; }
        public MLogConfiguration? LoggingClient { get; set; }

        public void InheritFrom(MVersion parentVersion)
        {
            /*
               Overload : 
               AssetId, AssetUrl, AssetHash, ClientDownloadUrl,
               ClientHash, MainClass, MinecraftArguments, JavaVersion
               
               Combine : 
               Libraries, GameArguments, JvmArguments
            */

            // Overloads

            if (Assets == null)
                Assets = parentVersion.Assets;
            else
            {
                if (string.IsNullOrEmpty(Assets.Id))
                    Assets.Id = parentVersion.Assets?.Id;

                if (string.IsNullOrEmpty(Assets.Url))
                    Assets.Url = parentVersion.Assets?.Url;

                if (string.IsNullOrEmpty(Assets.Sha1))
                    Assets.Sha1 = parentVersion.Assets?.Sha1;
            }

            if (Client == null)
                Client = parentVersion.Client;
            else
            {
                if (string.IsNullOrEmpty(Client.Url))
                    Client.Url = parentVersion.Client?.Url;

                if (string.IsNullOrEmpty(Client.Sha1))
                    Client.Sha1 = parentVersion.Client?.Sha1;
            }

            if (string.IsNullOrEmpty(MainClass))
                MainClass = parentVersion.MainClass;

            if (string.IsNullOrEmpty(MinecraftArguments))
                MinecraftArguments = parentVersion.MinecraftArguments;

            if (string.IsNullOrEmpty(JavaVersion))
                JavaVersion = parentVersion.JavaVersion;

            if (LoggingClient == null)
                LoggingClient = parentVersion.LoggingClient;
            //Jar = parentVersion.Jar;

            // Combine

            if (parentVersion.Libraries != null)
            {
                if (Libraries != null)
                    Libraries = Libraries.Concat(parentVersion.Libraries).ToArray();
                else
                    Libraries = parentVersion.Libraries;
            }

            if (parentVersion.GameArguments != null)
            {
                if (GameArguments != null)
                    GameArguments = parentVersion.GameArguments.Concat(GameArguments).ToArray();
                else
                    GameArguments = parentVersion.GameArguments;
            }

            if (parentVersion.JvmArguments != null)
            {
                if (JvmArguments != null)
                    JvmArguments = parentVersion.JvmArguments.Concat(JvmArguments).ToArray();
                else
                    JvmArguments = parentVersion.JvmArguments;
            }
        }

        public override string ToString()
        {
            return this.Id;
        }
    }
}
