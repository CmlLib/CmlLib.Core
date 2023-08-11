using CmlLib.Core.Rules;

namespace CmlLib.Core.Java;

public class MinecraftJavaPathResolver : IJavaPathResolver
{
    public static readonly JavaVersion JreLegacyVersion = new JavaVersion("jre-legacy");
    public static readonly JavaVersion CmlLegacyVersion = new JavaVersion("m-legacy");
    
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

    public string? GetDefaultJavaBinaryPath(LauncherOSRule os)
    {
        var javaVersions = GetInstalledJavaVersions();
        string? javaPath = null;
        
        if (string.IsNullOrEmpty(javaPath) &&
            javaVersions.Contains(MinecraftJavaPathResolver.JreLegacyVersion.Component))
            javaPath = GetJavaBinaryPath(MinecraftJavaPathResolver.JreLegacyVersion, os);
        
        if (string.IsNullOrEmpty(javaPath) &&
            javaVersions.Contains(MinecraftJavaPathResolver.CmlLegacyVersion.Component))
            javaPath = GetJavaBinaryPath(MinecraftJavaPathResolver.CmlLegacyVersion, os);

        if (string.IsNullOrEmpty(javaPath) && 
            javaVersions.Length > 0)
            javaPath = GetJavaBinaryPath(new JavaVersion(javaVersions[0]), os);

        return javaPath;
    }
    
    public string GetJavaBinaryPath(JavaVersion javaVersionName, LauncherOSRule os)
    {
        return Path.Combine(
            GetJavaDirPath(javaVersionName), 
            "bin", 
            GetJavaBinaryName(os));
    }
    
    public string GetJavaDirPath() 
        => GetJavaDirPath(JreLegacyVersion);

    public string GetJavaDirPath(JavaVersion javaVersionName) 
        => Path.Combine(runtimeDirectory, javaVersionName.Component);

    public string GetJavaBinaryName(LauncherOSRule os)
    {
        string binaryName = "java";
        if (os.Name == LauncherOSRule.Windows)
            binaryName = "javaw.exe";
        return binaryName;
    }
}