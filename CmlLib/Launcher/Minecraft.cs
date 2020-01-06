using System;
using System.IO;

namespace CmlLib.Launcher
{
    public class Minecraft
    {
        public readonly static string
            MacDefaultPath = "",
            LinuxDefaultPath = "",
            WindowsDefaultPath = Environment.GetEnvironmentVariable("appdata") + "\\.minecraft";

        public static string path = "";
        public static string Library;
        public static string Versions;
        public static string Resource;
        public static string Index;
        public static string Assets;
        public static string AssetObject;
        public static string AssetLegacy;

        /// <summary>
        /// Set Minecraft Path
        /// </summary>
        public static void Initialize(string p)
        {
            Initialize(p, p);
        }

        /// <summary>
        /// Set Minecraft Path
        /// </summary>
        /// <param name="p">path</param>
        /// <param name="assetsPath">set base path of Index, Assets, AssetObject, AssetLegacy</param>
        public static void Initialize(string p, string assetsPath)
        {
            path = c(p);

            Library = c(path + "/libraries");
            Versions = c(path + "/versions");
            Resource = c(path + "/resources");

            Index = c(assetsPath + "/assets/indexes");
            Assets = c(assetsPath + "/assets");
            AssetObject = c(Assets + "/objects");
            AssetLegacy = c(Assets + "/virtual/legacy");
        }

        static string c(string path)
        {
            var p = Path.GetFullPath(path);
            if (!Directory.Exists(p))
                Directory.CreateDirectory(p);

            return p;
        }

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
    }
}
