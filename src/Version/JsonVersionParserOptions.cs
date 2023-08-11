using CmlLib.Core.Rules;

namespace CmlLib.Core.Version;

public class JsonVersionParserOptions
{
    public const string ClientSide = "client";
    public const string ServerSide = "server";

    public bool SkipError { get; set; } = true;
    public string Side { get; set; } = "client";
    public IRulesEvaluator RulesEvaluator { get; set; } = new RulesEvaluator();
}