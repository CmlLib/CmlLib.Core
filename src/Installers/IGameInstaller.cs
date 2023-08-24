using CmlLib.Core.FileExtractors;
using CmlLib.Core.Rules;
using CmlLib.Core.Version;

namespace CmlLib.Core.Installers;

public interface IGameInstaller
{
    ValueTask Install(
        IEnumerable<IFileExtractor> extractors,
        MinecraftPath path, 
        IVersion version,
        RulesEvaluatorContext rulesContext,
        IProgress<InstallerProgressChangedEventArgs>? fileProgress,
        IProgress<ByteProgress>? byteProgress,
        CancellationToken cancellationToken);
}