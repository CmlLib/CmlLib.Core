using CmlLib.Core.Rules;

namespace CmlLib.Core.Java;

public interface IJavaPathResolver
{
    string[] GetInstalledJavaVersions();
    string? GetDefaultJavaBinaryPath(RulesEvaluatorContext rules);
    string GetJavaBinaryPath(JavaVersion javaVersion, RulesEvaluatorContext rules);
    string GetJavaDirPath(JavaVersion javaVersion, RulesEvaluatorContext rules);
}