using CmlLib.Core.FileExtractors;
using CmlLib.Core.Rules;
using CmlLib.Core.Tasks;
using CmlLib.Core.Version;

namespace CmlLib.Core.Benchmarks;

public class RandomFileExtractor : IFileExtractor
{
    private readonly string _path;
    private readonly int _fileCount;
    private readonly int _fileSize;

    public RandomFileExtractor(string path, int fileCount, int fileSize)
    {
        _path = path;
        _fileCount = fileCount;
        _fileSize = fileSize;
    }

    public void Setup()
    {
        Directory.CreateDirectory(_path);

        for (int i = 0; i < _fileCount; i++)
        {
            var dummyFilePath = Path.Combine(_path, i + ".dat");
            if (TPLTaskExecutorWithRandomFileBenchmark.Verbose)
                Console.WriteLine(dummyFilePath);
            using var fs = File.Create(dummyFilePath);

            var bufferSize = 1024 * 8;
            var buffer = new byte[bufferSize];
            var size = 0;
            while (size < _fileSize)
            {
                Random.Shared.NextBytes(buffer);
                fs.Write(buffer, 0, bufferSize);
                size += bufferSize;
            }

        }
    }

    public void Cleanup()
    {
        foreach (var file in Directory.GetFiles(_path))
        {
            File.Delete(file);
        }
        Directory.Delete(_path);
    }

    public ValueTask<IEnumerable<LinkedTaskHead>> Extract(MinecraftPath path, IVersion version, RulesEvaluatorContext rulesContext, CancellationToken cancellationToken)
    {
        var result = extract();
        return new ValueTask<IEnumerable<LinkedTaskHead>>(result);
    }

    private IEnumerable<LinkedTaskHead> extract()
    {
        foreach (var filePath in Directory.GetFiles(_path))
        {
            var file = new TaskFile(filePath)
            {
                Path = filePath,
                Hash = "-"
            };


            var task = new FileCheckTask(file);
            yield return new LinkedTaskHead(task, file);
        }
    }
}