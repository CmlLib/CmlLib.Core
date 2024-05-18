using CmlLib.Core.Internals;
using CmlLib.Core.Files;
using System.Diagnostics;

namespace CmlLib.Core.Installers;

public abstract class GameInstallerBase : IGameInstaller
{
    private readonly HttpClient _httpClient;

    public GameInstallerBase(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private volatile bool IsRunning = false;
    public bool CheckFileSize { get; set; } = false;
    public bool CheckFileChecksum { get; set; } = true;
    public IEnumerable<string> ExcludeFiles { get; set; } = Enumerable.Empty<string>();

    private HashSet<string> excludeSet = new();
    private IProgress<InstallerProgressChangedEventArgs>? FileProgress;
    private IProgress<ByteProgress>? ByteProgress;

    public async ValueTask Install(
        IEnumerable<GameFile> gameFiles,
        IProgress<InstallerProgressChangedEventArgs>? fileProgress,
        IProgress<ByteProgress>? byteProgress,
        CancellationToken cancellationToken)
    {
        if (IsRunning)
            throw new InvalidOperationException("Already installing");

        IsRunning = true;
        this.FileProgress = fileProgress;
        this.ByteProgress = byteProgress;

        excludeSet.Clear();
        foreach (var excludeFile in ExcludeFiles)
        {
            excludeSet.Add(excludeFile);
        }

        try
        {
            await Install(gameFiles, cancellationToken);
        }
        finally
        { 
            IsRunning = false;
        }
    }

    protected abstract ValueTask Install(IEnumerable<GameFile> gameFiles, CancellationToken cancellationToken);

    protected bool NeedUpdate(GameFile file)
    {
        if (string.IsNullOrEmpty(file.Path))
            return false;

        if (!File.Exists(file.Path))
            return true;

        if (CheckFileSize && file.Size > 0)
        {
            var realSize = new FileInfo(file.Path).Length;
            if (realSize != file.Size)
                return true;
        }

        if (CheckFileChecksum && !string.IsNullOrEmpty(file.Hash))
        {
            var realHash = IOUtil.ComputeFileSHA1(file.Path);
            if (realHash != file.Hash)
                return true;
        }

        return false;
    }

    protected virtual async Task Download(
        GameFile file,
        IProgress<ByteProgress>? progress,
        CancellationToken cancellationToken)
    {
        Debug.Assert(!string.IsNullOrEmpty(file.Path));
        if (string.IsNullOrEmpty(file.Url))
            return;

        await HttpClientDownloadHelper.DownloadFileAsync(
            _httpClient,
            file.Url,
            file.Size,
            file.Path,
            progress,
            cancellationToken);
    }

    protected bool IsExcludedPath(string path)
    {
        return excludeSet.Contains(path);
    }

    protected void FireFileProgress(
        int totalTasks,
        int progressedTasks,
        string? name,
        InstallerEventType type)
    {
        FileProgress?.Report(new InstallerProgressChangedEventArgs(
            totalTasks: totalTasks,
            progressedTasks: progressedTasks,
            name: name,
            type: type));
    }

    protected void FireByteProgress(ByteProgress progress)
    {
        ByteProgress?.Report(progress);
    }
}