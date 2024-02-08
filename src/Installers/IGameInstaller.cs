using CmlLib.Core.FileExtractors;
using CmlLib.Core.Rules;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;

namespace CmlLib.Core.Installers;

public interface IGameInstaller
{
    ValueTask Install(
        IEnumerable<GameFile> gameFiles,
        IProgress<InstallerProgressChangedEventArgs>? fileProgress,
        IProgress<ByteProgress>? byteProgress,
        CancellationToken cancellationToken);
}