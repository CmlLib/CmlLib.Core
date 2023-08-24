using CmlLib.Core.Rules;

namespace CmlLib.Core.Java;

public interface IJavaPathResolver
{
    string[] GetInstalledJavaVersions(MinecraftPath path);
    string? GetDefaultJavaBinaryPath(MinecraftPath path, RulesEvaluatorContext rules);
    string GetJavaBinaryPath(MinecraftPath path, JavaVersion javaVersion, RulesEvaluatorContext rules);
    string GetJavaDirPath(MinecraftPath path, JavaVersion javaVersion, RulesEvaluatorContext rules);
}