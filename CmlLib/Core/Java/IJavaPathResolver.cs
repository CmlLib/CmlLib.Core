namespace CmlLib.Core.Java
{
    public interface IJavaPathResolver
    {
        //string GetJavaBinaryPath(string osName);
        string GetJavaBinaryPath(string javaVersionName, string osName);
        string GetJavaDirPath();
        string GetJavaDirPath(string javaVersionName);
    }
}