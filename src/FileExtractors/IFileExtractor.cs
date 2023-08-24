using CmlLib.Core.Rules;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;

namespace CmlLib.Core.FileExtractors;

public interface IFileExtractor
{
    ValueTask<IEnumerable<LinkedTaskHead>> Extract(
        MinecraftPath path, 
        IVersion version,
        RulesEvaluatorContext rulesContext,
        CancellationToken cancellationToken);
}