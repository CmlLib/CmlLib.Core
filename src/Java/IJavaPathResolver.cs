using CmlLib.Core.Rules;

namespace CmlLib.Core.Java;

public interface IJavaPathResolver
{
    IReadOnlyCollection<string> GetInstalledJavaVersions();
    IReadOnlyCollection<string> GetInstalledJavaVersions(RulesEvaluatorContext rules);
    string? GetDefaultJavaBinaryPath(RulesEvaluatorContext rules);
    string GetJavaBinaryPath(JavaVersion javaVersion, RulesEvaluatorContext rules);
    string GetJavaDirPath(JavaVersion javaVersion, RulesEvaluatorContext rules);
}