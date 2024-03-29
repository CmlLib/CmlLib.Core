using CmlLib.Core.Rules;

namespace CmlLib.Core.Java;

public class MinecraftJavaPathResolver : IJavaPathResolver
{
    public static readonly JavaVersion JreLegacyVersion = new JavaVersion("jre-legacy");
    public static readonly JavaVersion CmlLegacyVersion = new JavaVersion("m-legacy");

    private readonly MinecraftPath _path;

    public MinecraftJavaPathResolver(MinecraftPath path)
    {
        this._path = path;
    }

    public string[] GetInstalledJavaVersions()
    {
        var dir = new DirectoryInfo(_path.Runtime);
        if (!dir.Exists)
            return Array.Empty<string>();

        return dir.GetDirectories()
            .Select(x => x.Name)
            .ToArray();
    }

    public string? GetDefaultJavaBinaryPath(RulesEvaluatorContext rules)
    {
        var javaVersions = GetInstalledJavaVersions();
        string? javaPath = null;
        
        if (string.IsNullOrEmpty(javaPath) &&
            javaVersions.Contains(MinecraftJavaPathResolver.JreLegacyVersion.Component))
            javaPath = GetJavaBinaryPath(MinecraftJavaPathResolver.JreLegacyVersion, rules);
        
        if (string.IsNullOrEmpty(javaPath) &&
            javaVersions.Contains(MinecraftJavaPathResolver.CmlLegacyVersion.Component))
            javaPath = GetJavaBinaryPath(MinecraftJavaPathResolver.CmlLegacyVersion, rules);

        if (string.IsNullOrEmpty(javaPath) && 
            javaVersions.Length > 0)
            javaPath = GetJavaBinaryPath(new JavaVersion(javaVersions[0]), rules);

        return javaPath;
    }
    
    public string GetJavaBinaryPath(JavaVersion javaVersionName, RulesEvaluatorContext rules)
    {
        return Path.Combine(
            GetJavaDirPath(javaVersionName, rules), 
            "bin", 
            GetJavaBinaryName(rules.OS));
    }

    public string GetJavaDirPath(JavaVersion javaVersionName, RulesEvaluatorContext rules) 
        => Path.Combine(_path.Runtime, javaVersionName.Component);

    public string GetJavaBinaryName(LauncherOSRule os)
    {
        string binaryName = "java";
        if (os.Name == LauncherOSRule.Windows)
            binaryName = "javaw.exe";
        return binaryName;
    }
}