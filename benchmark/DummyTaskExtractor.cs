using CmlLib.Core.FileExtractors;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;

namespace CmlLib.Core.Benchmarks;

public class DummyTask : LinkedTask
{
    public static int Seed { get; set; }

    public DummyTask(TaskFile file, int seed) : base(file) => Seed = seed;

    protected override ValueTask<LinkedTask?> OnExecuted()
    {
        for (int j = 0; j < 1024*512; j++)
            Seed += j;
        return new ValueTask<LinkedTask?>(NextTask);
    }
}

public class DummyTaskExtractor : IFileExtractor
{
    private readonly int _count;

    public DummyTaskExtractor(int count) => _count = count;

    public ValueTask<IEnumerable<LinkedTaskHead>> Extract(MinecraftPath path, IVersion version)
    {
        var r = extract();
        return new ValueTask<IEnumerable<LinkedTaskHead>>(r);
    }

    private IEnumerable<LinkedTaskHead> extract()
    {
        for (int i = 0; i < _count; i++)
        {
            var file = new TaskFile(i.ToString());
            var task = new DummyTask(file, i);
            task.InsertNextTask(new DummyTask(file, i));
            task.InsertNextTask(new DummyTask(file, i));
            yield return new LinkedTaskHead(task, file);
        }
    }
}