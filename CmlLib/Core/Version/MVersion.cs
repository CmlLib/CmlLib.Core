using CmlLib.Core.Files;
using System.Linq;

namespace CmlLib.Core.Version
{
    public class MVersion
    {
        public bool IsInherited { get; set; }
        public string ParentVersionId { get; set; }

        public string Id { get; set; }

        public string AssetId { get; set; } = "";
        public string AssetUrl { get; set; } = "";
        public string AssetHash { get; set; } = "";

        public string Jar { get; set; } = "";
        public string ClientDownloadUrl { get; set; } = "";
        public string ClientHash { get; set; } = "";
        public MLibrary[] Libraries { get; set; }
        public string MainClass { get; set; } = "";
        public string MinecraftArguments { get; set; } = "";
        public string[] GameArguments { get; set; }
        public string[] JvmArguments { get; set; }
        public string ReleaseTime { get; set; } = "";
        public MVersionType Type { get; set; } = MVersionType.Custom;
        public string TypeStr { get; set; } = "";

        public void InheritFrom(MVersion parentVersion)
        {
            // Inherit list
            // Overload : AssetId, AssetUrl, AssetHash, ClientDownloadUrl, ClientHash, MainClass, MinecraftArguments
            // Combine : Libraries, GameArguments, JvmArguments

            // Overloads

            if (nc(AssetId))
                AssetId = parentVersion.AssetId;

            if (nc(AssetUrl))
                AssetUrl = parentVersion.AssetUrl;

            if (nc(AssetHash))
                AssetHash = parentVersion.AssetHash;

            if (nc(ClientDownloadUrl))
                ClientDownloadUrl = parentVersion.ClientDownloadUrl;

            if (nc(ClientHash))
                ClientHash = parentVersion.ClientHash;

            if (nc(MainClass))
                MainClass = parentVersion.MainClass;

            if (nc(MinecraftArguments))
                MinecraftArguments = parentVersion.MinecraftArguments;

            Jar = parentVersion.Jar;

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
                    GameArguments = GameArguments.Concat(parentVersion.GameArguments).ToArray();
                else
                    GameArguments = parentVersion.GameArguments;
            }

            if (parentVersion.JvmArguments != null)
            {
                if (JvmArguments != null)
                    JvmArguments = JvmArguments.Concat(parentVersion.JvmArguments).ToArray();
                else
                    JvmArguments = parentVersion.JvmArguments;
            }
        }

        private static bool nc(string t) // check null string
        {
            return string.IsNullOrEmpty(t);
        }

        public override string ToString()
        {
            return this.Id;
        }
    }
}
