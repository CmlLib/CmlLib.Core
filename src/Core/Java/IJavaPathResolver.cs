namespace CmlLib.Core.Java
{
    public interface IJavaPathResolver
    {
        string[] GetInstalledJavaVersions();
        string? GetDefaultJavaBinaryPath();
        string GetJavaBinaryPath(string javaVersionName);
        string GetJavaBinaryPath(string javaVersionName, string osName);
        string GetJavaDirPath(string javaVersionName);
    }
}