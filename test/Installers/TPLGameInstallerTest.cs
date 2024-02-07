using CmlLib.Core.FileExtractors;
using CmlLib.Core.Installers;
using CmlLib.Core.Rules;
using CmlLib.Core.Tasks;
using CmlLib.Core.Test.Tasks;
using CmlLib.Core.Version;
using Moq;
using NUnit.Framework;

namespace CmlLib.Core.Test.Installers;

public class TPLGameInstallerTest
{
    private const int TaskCount = 128;
    private const int ExtractorCount = 4;
    private const long TaskSize = 128 * 32;
    private readonly ITaskFactory TestTaskFactory = new CmlLib.Core.Tasks.TaskFactory(new HttpClient());
    private readonly RulesEvaluatorContext TestRulesContext = new RulesEvaluatorContext(LauncherOSRule.Current);
    private readonly MinecraftPath TestMinecraftPath = new Mock<MinecraftPath>().Object;
    private readonly IVersion TestVersion = new Mock<IVersion>().Object;

    [Test]
    public async Task TestAllTaskExecuted()
    {
        var mocks = createMockTasks();
        var extractors = createMockExtractors(mocks);

        var installer = createInstaller();
        await installer.Install(
            TestTaskFactory,
            extractors,
            TestMinecraftPath,
            TestVersion,
            TestRulesContext,
            null,
            null,
            default);

        assertAllMockTasks(mocks);
    }

    [Test]
    public async Task TestFileProgressReachesTo100()
    {
        var mocks = createMockTasks();
        var extractors = createMockExtractors(mocks);

        int totalTasks = 0;
        int progressedTasks = 0;
        var fileProgress = new SyncProgress<InstallerProgressChangedEventArgs>(e =>
        {
            totalTasks = Math.Max(totalTasks, e.TotalTasks);
            progressedTasks = Math.Max(progressedTasks, e.ProgressedTasks);
        });

        var installer = createInstaller();
        await installer.Install(
            TestTaskFactory,
            extractors,
            TestMinecraftPath,
            TestVersion,
            TestRulesContext,
            fileProgress,
            null,
            default
        );

        assertAllMockTasks(mocks);
        Assert.That(totalTasks, Is.EqualTo(totalTasks), "TotalTasks");
        Assert.That(progressedTasks, Is.EqualTo(totalTasks), "ProgressedTasks");
    }

    [Test]
    public async Task TestByteProgressReachesTo100()
    {
        var mocks = createMockDownloadTasks();
        var extractors = createMockExtractors(mocks);

        long totalBytes = 0;
        long progressedBytes = 0;
        var byteProgress = new SyncProgress<ByteProgress>(e => 
        {
            totalBytes = Math.Max(totalBytes, e.TotalBytes);
            progressedBytes = Math.Max(progressedBytes, e.ProgressedBytes);
        });

        var installer = createInstaller();
        await installer.Install(
            TestTaskFactory,
            extractors,
            TestMinecraftPath,
            TestVersion,
            TestRulesContext,
            null,
            byteProgress,
            default
        );

        assertAllMockTasks(mocks);
        Assert.That(totalBytes, Is.EqualTo(TaskCount * TaskSize), "TotalBytes");
        Assert.That(progressedBytes, Is.EqualTo(totalBytes), 
            "ProgressedBytes, d: " + (totalBytes - progressedBytes) / (double)TaskSize);
    }

    [Test]
    public async Task TestFilterDuplicatedFile()
    {
        var divide = 2;
        var mockTasks = Enumerable.Range(0, TaskCount)
            .Select(i =>
            {
                var setId = i / (TaskCount / divide);
                return new MockTask(new TaskFile($"{i} {setId}")
                {
                    Path = setId.ToString()
                });
            })
            .ToArray();
        
        var extractors = createMockExtractors(mockTasks);

        var installer = createInstaller();
        await installer.Install(
            TestTaskFactory,
            extractors,
            TestMinecraftPath,
            TestVersion,
            TestRulesContext,
            null,
            null,
            default
        );

        var set = new HashSet<string>();
        foreach (var task in mockTasks)
        {
            var setId = task.Name.Split()[1];
            if (task.IsExecuted && !set.Add(setId))
                Assert.Fail("duplicated tasks are executed: " + setId);
        }

        Assert.That(set.Count, Is.EqualTo(divide), "not all tasks are executed");
    }

    [Test]
    public void TestExceptionMode()
    {

    }

    private MockTask[] createMockTasks() => 
        Enumerable.Range(0, TaskCount)
            .Select(i => new MockTask(new TaskFile(i.ToString())))
            .ToArray();

    private MockDownloadTask[] createMockDownloadTasks() =>
        Enumerable.Range(0, TaskCount)
            .Select(i => new MockDownloadTask(new TaskFile(i.ToString())
            {
                Size = TaskSize,
            }))
            .ToArray();

    private IEnumerable<IFileExtractor> createMockExtractors(IEnumerable<MockTask> tasks) => tasks
        .Select(task => new LinkedTaskHead(task, task.File))
        .Chunk(TaskCount / ExtractorCount)
        .Select(chunkedTasks =>
        {
            var mockExtractor = new Mock<IFileExtractor>();
            mockExtractor.Setup(e => e.Extract(
                It.IsAny<ITaskFactory>(),
                It.IsAny<MinecraftPath>(),
                It.IsAny<IVersion>(),
                It.IsAny<RulesEvaluatorContext>(),
                default).Result)
                .Returns(chunkedTasks);
            return mockExtractor.Object;
        });

    private void assertAllMockTasks(IEnumerable<MockTask> mocks)
    {
        var list = new List<string>();
        foreach (var mock in mocks)
        {
            if (!mock.IsExecuted)
                list.Add(mock.Name);
        }

        if (list.Any())
            Assert.Fail("Find tasks not executed: " + string.Join(',', list));
    }

    public IGameInstaller createInstaller()
    {
        return new TPLGameInstaller(1);
    }
}