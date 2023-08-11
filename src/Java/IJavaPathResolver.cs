using CmlLib.Core.Rules;

namespace CmlLib.Core.Java;

public interface IJavaPathResolver
{
    string[] GetInstalledJavaVersions();
    string? GetDefaultJavaBinaryPath(LauncherOSRule os);
    string GetJavaBinaryPath(JavaVersion javaVersionName, LauncherOSRule os);
    string GetJavaDirPath(JavaVersion javaVersionName);
}