using CmlLib.Core.Rules;
using CmlLib.Core.Files;
using CmlLib.Core.Version;

namespace CmlLib.Core.FileExtractors;

public interface IFileExtractor
{
    ValueTask<IEnumerable<GameFile>> Extract(
        MinecraftPath path,
        IVersion version,
        RulesEvaluatorContext rulesContext,
        CancellationToken cancellationToken);
}