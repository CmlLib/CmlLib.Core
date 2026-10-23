using CmlLib.Core.Files;
using CmlLib.Core.Installers;

namespace CmlLib.Core.Test.Installers;

public class ParallelGameInstallerTests
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

    [Fact]
    public async Task CancellationDuringEnqueueWaitsForAllDownloads()
    {
        using var client = new HttpClient();
        using var cancellation = new CancellationTokenSource();
        var installer = new ControlledInstaller(client);
        var installation = installer.Install(createFiles(50), null, null, cancellation.Token).AsTask();

        await installer.DownloadsStarted.Task.WaitAsync(Timeout);
        cancellation.Cancel();
        try
        {
            await assertWaitingForDownloads(installation);
        }
        finally
        {
            installer.ReleaseDownloads.TrySetResult();
        }

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => installation.WaitAsync(Timeout));
        Assert.Equal(2, installer.FinishedDownloads);
        await installer.Install(Array.Empty<GameFile>(), null, null, CancellationToken.None);
    }

    [Fact]
    public async Task DownloadFailureStopsBlockedEnqueueAndPreservesDownloadError()
    {
        using var client = new HttpClient();
        using var cancellation = new CancellationTokenSource();
        var installer = new ControlledInstaller(client);
        var installation = installer.Install(createFiles(50), null, null, cancellation.Token).AsTask();

        await installer.DownloadsStarted.Task.WaitAsync(Timeout);
        installer.ReleaseDownloads.TrySetResult();
        try
        {
            await Assert.ThrowsAsync<IOException>(() => installation.WaitAsync(Timeout));
            Assert.Equal(2, installer.FinishedDownloads);
        }
        finally
        {
            cancellation.Cancel();
        }
    }

    [Fact]
    public async Task EnumerationFailureWaitsForDownloadsAndPreservesOriginalError()
    {
        using var client = new HttpClient();
        var installer = new ControlledInstaller(client);
        var expected = new InvalidOperationException("File enumeration failed");
        var installation = installer.Install(
            enumerateWithFailure(installer, expected),
            null,
            null,
            CancellationToken.None).AsTask();

        try
        {
            await installer.DownloadsStarted.Task.WaitAsync(Timeout);
            await assertWaitingForDownloads(installation);
        }
        finally
        {
            installer.ReleaseDownloads.TrySetResult();
        }

        var actual = await Assert.ThrowsAsync<InvalidOperationException>(() => installation.WaitAsync(Timeout));
        Assert.Same(expected, actual);
        Assert.Equal(2, installer.FinishedDownloads);
    }

    [Fact]
    public async Task ProgressFailureWaitsForDownloadsAndPreservesOriginalError()
    {
        using var client = new HttpClient();
        var installer = new ControlledInstaller(client);
        var expected = new InvalidOperationException("Progress callback failed");
        var progress = new CallbackProgress<ByteProgress>(_ =>
        {
            installer.DownloadsStarted.Task.WaitAsync(Timeout).GetAwaiter().GetResult();
            throw expected;
        });
        var installation = installer.Install(createFiles(2), null, progress, CancellationToken.None).AsTask();

        try
        {
            await installer.DownloadsStarted.Task.WaitAsync(Timeout);
            await assertWaitingForDownloads(installation);
        }
        finally
        {
            installer.ReleaseDownloads.TrySetResult();
        }

        var actual = await Assert.ThrowsAsync<InvalidOperationException>(() => installation.WaitAsync(Timeout));
        Assert.Same(expected, actual);
        Assert.Equal(2, installer.FinishedDownloads);
    }

    [Fact]
    public async Task DownloadFailureWaitsForAllSiblingDownloads()
    {
        using var client = new HttpClient();
        var installer = new ControlledInstaller(client);
        var installation = installer.Install(createFiles(2), null, null, CancellationToken.None).AsTask();

        await installer.DownloadsStarted.Task.WaitAsync(Timeout);
        installer.ReleaseDownloads.TrySetResult();

        await Assert.ThrowsAsync<IOException>(() => installation.WaitAsync(Timeout));
        Assert.Equal(2, installer.FinishedDownloads);
    }

    [Fact]
    public async Task SuccessfulInstallationCanBeReused()
    {
        using var client = new HttpClient();
        var installer = new ControlledInstaller(client) { FailDownloads = false };
        var installation = installer.Install(createFiles(2), null, null, CancellationToken.None).AsTask();

        await installer.DownloadsStarted.Task.WaitAsync(Timeout);
        installer.ReleaseDownloads.TrySetResult();
        await installation.WaitAsync(Timeout);

        Assert.Equal(2, installer.FinishedDownloads);
        await installer.Install(Array.Empty<GameFile>(), null, null, CancellationToken.None);
    }

    private static async Task assertWaitingForDownloads(Task installation)
    {
        // 취소 토큰을 무시하는 진행 중 작업이 끝나기 전에 설치 호출이 반환되면 안 된다.
        var completed = await Task.WhenAny(installation, Task.Delay(100));
        Assert.NotSame(installation, completed);
    }

    private static IEnumerable<GameFile> enumerateWithFailure(
        ControlledInstaller installer,
        Exception exception)
    {
        foreach (var file in createFiles(2))
            yield return file;

        installer.DownloadsStarted.Task.WaitAsync(Timeout).GetAwaiter().GetResult();
        throw exception;
    }

    private static IEnumerable<GameFile> createFiles(int count)
    {
        return Enumerable.Range(0, count)
            .Select(index => new GameFile(index.ToString())
            {
                Path = Path.Combine(Path.GetTempPath(), "cmllib-installer-test", Guid.NewGuid().ToString("N")),
                Url = "https://example.invalid/not-requested",
                Size = 1
            });
    }

    private sealed class CallbackProgress<T>(Action<T> callback) : IProgress<T>
    {
        public void Report(T value)
        {
            callback(value);
        }
    }

    private sealed class ControlledInstaller(HttpClient client) : ParallelGameInstaller(1, 2, 2, client)
    {
        private int startedDownloads;
        private int finishedDownloads;

        public TaskCompletionSource DownloadsStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource ReleaseDownloads { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public int FinishedDownloads => Volatile.Read(ref finishedDownloads);
        public bool FailDownloads { get; init; } = true;

        protected override async Task Download(
            GameFile file,
            IProgress<ByteProgress>? progress,
            CancellationToken cancellationToken)
        {
            if (Interlocked.Increment(ref startedDownloads) == 2)
                DownloadsStarted.TrySetResult();

            try
            {
                // 취소 이후에도 남아 있는 스트림 읽기의 실패를 네트워크 없이 재현한다.
                await ReleaseDownloads.Task;
                if (FailDownloads)
                    throw new IOException($"Download failed: {file.Name}");
            }
            finally
            {
                Interlocked.Increment(ref finishedDownloads);
            }
        }
    }
}
