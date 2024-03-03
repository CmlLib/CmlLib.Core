using CmlLib.Core.Files;

namespace CmlLib.Core.Installers;

public interface IGameInstaller
{
    ValueTask Install(
        IEnumerable<GameFile> gameFiles,
        IProgress<InstallerProgressChangedEventArgs>? fileProgress,
        IProgress<ByteProgress>? byteProgress,
        CancellationToken cancellationToken);
}