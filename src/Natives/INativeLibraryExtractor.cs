using CmlLib.Core.Rules;
using CmlLib.Core.Version;

namespace CmlLib.Core.Natives;

public interface INativeLibraryExtractor
{
    string Extract(MinecraftPath path, IVersion version, RulesEvaluatorContext rulesContext);
    void Clean(MinecraftPath path, IVersion version);
}