using System.IO;
using System.Linq;

namespace CmlLib.Core.Java
{
    public class MinecraftJavaPathResolver : IJavaPathResolver
    {
        public static readonly string JreLegacyVersionName = "jre-legacy";
        public static readonly string CmlLegacyVersionName = "m-legacy";
        
        public MinecraftJavaPathResolver(MinecraftPath path)
        {
            runtimeDirectory = path.Runtime;
        }

        public MinecraftJavaPathResolver(string path)
        {
            runtimeDirectory = path;
        }

        private readonly string runtimeDirectory;

        public string[] GetInstalledJavaVersions()
        {
            var dir = new DirectoryInfo(runtimeDirectory);
            if (!dir.Exists)
                return new string[] { };

            return dir.GetDirectories()
                .Select(x => x.Name)
                .ToArray();
        }

        public string? GetDefaultJavaBinaryPath()
        {
            var javaVersions = GetInstalledJavaVersions();
            string? javaPath = null;
            
            if (string.IsNullOrEmpty(javaPath) &&
                javaVersions.Contains(MinecraftJavaPathResolver.JreLegacyVersionName))
                javaPath = GetJavaBinaryPath(MinecraftJavaPathResolver.JreLegacyVersionName, MRule.OSName);
            
            if (string.IsNullOrEmpty(javaPath) &&
                javaVersions.Contains(MinecraftJavaPathResolver.CmlLegacyVersionName))
                javaPath = GetJavaBinaryPath(MinecraftJavaPathResolver.CmlLegacyVersionName, MRule.OSName);

            if (string.IsNullOrEmpty(javaPath) && 
                javaVersions.Length > 0)
                javaPath = GetJavaBinaryPath(javaVersions[0], MRule.OSName);

            return javaPath;
        }
        
        public string GetJavaBinaryPath(string javaVersionName)
            => GetJavaBinaryPath(javaVersionName, MRule.OSName);
        
        public string GetJavaBinaryPath(string javaVersionName, string osName)
        {
            return Path.Combine(
                GetJavaDirPath(javaVersionName), 
                "bin", 
                GetJavaBinaryName(osName));
        }
        
        public string GetJavaDirPath() 
            => GetJavaDirPath(JreLegacyVersionName);

        public string GetJavaDirPath(string javaVersionName) 
            => Path.Combine(runtimeDirectory, javaVersionName);

        public string GetJavaBinaryName(string osName)
        {
            string binaryName = "java";
            if (osName == MRule.Windows)
                binaryName = "javaw.exe";
            return binaryName;
        }
    }
}