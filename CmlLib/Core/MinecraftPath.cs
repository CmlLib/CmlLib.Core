using System;
using System.IO;

namespace CmlLib.Core
{
    public class MinecraftPath
    {
        public static readonly string
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

        public MinecraftPath() : this(GetOSDefaultPath())
        {

        }

        public MinecraftPath(string basePath) : this(basePath, basePath)
        {

        }

        public MinecraftPath(string basePath, string basePathForAssets)
        {
            BasePath = NormalizePath(basePath);

            Library = NormalizePath(BasePath + "/libraries");
            Versions = NormalizePath(BasePath + "/versions");
            Resource = NormalizePath(BasePath + "/resources");

            Runtime = NormalizePath(BasePath + "/runtime");
            Assets = NormalizePath(basePathForAssets + "/assets");

            CreateDirs();
        }

        public void CreateDirs()
        {
            Dir(BasePath);
            Dir(Library);
            Dir(Versions);
            Dir(Resource);
            Dir(Runtime);
            Dir(Assets);
        }

        public string BasePath { get; set; }
        public string Library { get; set; }
        public string Versions { get; set; }
        public string Resource { get; set; }
        public string Assets { get; set; }
        public string Runtime { get; set; }

        public virtual string GetIndexFilePath(string assetId)
            => NormalizePath($"{Assets}/indexes/{assetId}.json");

        public virtual string GetAssetObjectPath(string assetId)
            => NormalizePath($"{Assets}/objects");

        public virtual string GetAssetLegacyPath(string assetId)
            => NormalizePath($"{Assets}/virtual/legacy");

        public virtual string GetVersionJarPath(string id)
            => NormalizePath($"{Versions}/{id}/{id}.jar");

        public virtual string GetVersionJsonPath(string id)
            => NormalizePath($"{Versions}/{id}/{id}.json");

        public virtual string GetNativePath(string id)
            => NormalizePath($"{Versions}/{id}/natives");

        public override string ToString()
        {
            return BasePath;
        }

        protected static string Dir(string path)
        {
            string p = NormalizePath(path);
            if (!Directory.Exists(p))
                Directory.CreateDirectory(p);

            return p;
        }

        protected static string NormalizePath(string path)
        {
            return Path.GetFullPath(path)
                .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
                .TrimEnd(Path.DirectorySeparatorChar);
        }
    }
}
