using System;
using System.IO;

namespace CmlLib.Core
{
    public class MinecraftPath
    {
        public readonly static string
    MacDefaultPath = Environment.GetEnvironmentVariable("HOME") + "/Library/Application Support/minecraft",
    LinuxDefaultPath = Environment.GetEnvironmentVariable("HOME") + "/.minecraft",
    WindowsDefaultPath = Environment.GetEnvironmentVariable("appdata") + "\\.minecraft";

        public static string GetOSDefaultPath()
        {
            switch (MRule.OSName)
            {
                case "osx":
                    return MacDefaultPath;
                case "linux":
                    return LinuxDefaultPath;
                case "windows":
                    return WindowsDefaultPath;
                default:
                    return Environment.CurrentDirectory;
            }
        }

        public string BasePath { get; set; }
        public string Library { get; set; }
        public string Versions { get; set; }
        public string Resource { get; set; }
        public string Index { get; set; }
        public string Assets { get; set; }
        public string AssetObject { get; set; }
        public string AssetLegacy { get; set; }
        public string Runtime { get; set; }

        public MinecraftPath()
        {
            var basePath = GetOSDefaultPath();
            Initialize(basePath, basePath);
        }

        public MinecraftPath(string basePath)
        {
            Initialize(basePath, basePath);
        }

        public MinecraftPath(string basePath, string basePathForAssets)
        {
            Initialize(basePath, basePathForAssets);
        }

        private void Initialize(string p, string assetsPath)
        {
            BasePath = c(p);

            Library = c(BasePath + "/libraries");
            Versions = c(BasePath + "/versions");
            Resource = c(BasePath + "/resources");

            Runtime = c(BasePath + "/runtime");
            SetAssetsPath(assetsPath + "/assets");
        }

        public void SetAssetsPath(string p)
        {
            Assets = c(p);
            Index = c(Assets + "/indexes");
            AssetObject = c(Assets + "/objects");
            AssetLegacy = c(Assets + "/virtual/legacy");
        }

        public override string ToString()
        {
            return BasePath;
        }

        static string c(string path)
        {
            var p = Path.GetFullPath(path);
            if (!Directory.Exists(p))
                Directory.CreateDirectory(p);

            return p;
        }
    }
}
