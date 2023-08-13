using CmlLib.Core.FileExtractors;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;

namespace CmlLib.Core.Benchmarks;

public class DummyDownloaderExtractor : IFileExtractor
{
    private readonly int _count;
    private readonly string _prefix;
    public DummyDownloaderExtractor(string prefix, int count) => 
        (_prefix, _count) = (prefix, count);

    public ValueTask<IEnumerable<LinkedTaskHead>> Extract(MinecraftPath path, IVersion version)
    {
        var r = extract();
        return new ValueTask<IEnumerable<LinkedTaskHead>>(r);
    }

    private IEnumerable<LinkedTaskHead> extract()
    {
        for (int i = 0; i < _count; i++)
        {
            var file = new TaskFile(_prefix + "-" + i.ToString())
            {
                Size = 1024 * 256,
                Path = "a.dat",
                Url = "a.dat"
            };
            var task = LinkedTask.LinkTasks(
                new DummyTask(file, i),
                new DummyDownloadTask(file, null!)
            );
            yield return new LinkedTaskHead(task, file);
        }
    }
}