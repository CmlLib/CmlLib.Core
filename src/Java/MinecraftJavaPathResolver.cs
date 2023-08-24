using CmlLib.Core.Rules;

namespace CmlLib.Core.Java;

public class MinecraftJavaPathResolver : IJavaPathResolver
{
    public static readonly JavaVersion JreLegacyVersion = new JavaVersion("jre-legacy");
    public static readonly JavaVersion CmlLegacyVersion = new JavaVersion("m-legacy");

    public string[] GetInstalledJavaVersions(MinecraftPath path)
    {
        var dir = new DirectoryInfo(path.Runtime);
        if (!dir.Exists)
            return Array.Empty<string>();

        return dir.GetDirectories()
            .Select(x => x.Name)
            .ToArray();
    }

    public string? GetDefaultJavaBinaryPath(MinecraftPath path, RulesEvaluatorContext rules)
    {
        var javaVersions = GetInstalledJavaVersions(path);
        string? javaPath = null;
        
        if (string.IsNullOrEmpty(javaPath) &&
            javaVersions.Contains(MinecraftJavaPathResolver.JreLegacyVersion.Component))
            javaPath = GetJavaBinaryPath(path, MinecraftJavaPathResolver.JreLegacyVersion, rules);
        
        if (string.IsNullOrEmpty(javaPath) &&
            javaVersions.Contains(MinecraftJavaPathResolver.CmlLegacyVersion.Component))
            javaPath = GetJavaBinaryPath(path, MinecraftJavaPathResolver.CmlLegacyVersion, rules);

        if (string.IsNullOrEmpty(javaPath) && 
            javaVersions.Length > 0)
            javaPath = GetJavaBinaryPath(path, new JavaVersion(javaVersions[0]), rules);

        return javaPath;
    }
    
    public string GetJavaBinaryPath(MinecraftPath path, JavaVersion javaVersionName, RulesEvaluatorContext rules)
    {
        return Path.Combine(
            GetJavaDirPath(path, javaVersionName, rules), 
            "bin", 
            GetJavaBinaryName(rules.OS));
    }

    public string GetJavaDirPath(MinecraftPath path, JavaVersion javaVersionName, RulesEvaluatorContext rules) 
        => Path.Combine(path.Runtime, javaVersionName.Component);

    public string GetJavaBinaryName(LauncherOSRule os)
    {
        string binaryName = "java";
        if (os.Name == LauncherOSRule.Windows)
            binaryName = "javaw.exe";
        return binaryName;
    }
}