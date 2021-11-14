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
        
        //public string GetJavaBinaryPath(string osName)
        //    => GetJavaBinaryPath(JreLegacyVersionName, osName);
        
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