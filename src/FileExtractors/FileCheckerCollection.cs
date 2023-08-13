using System.Collections;
using CmlLib.Core.Java;
using CmlLib.Core.Rules;

namespace CmlLib.Core.FileExtractors;

public class FileExtractorCollection : IEnumerable<IFileExtractor>
{
    public static FileExtractorCollection CreateDefault(
        HttpClient httpClient,
        IJavaPathResolver javaPathResolver,
        IRulesEvaluator rulesEvaluator,
        RulesEvaluatorContext context)
    {
        var extractors = new FileExtractorCollection();

        var library = new LibraryFileExtractor(rulesEvaluator, context, httpClient);
        var asset = new AssetFileExtractor(httpClient);
        var client = new ClientFileExtractor(httpClient);
        var java = new JavaFileExtractor(httpClient, javaPathResolver, context.OS);
        var log = new LogFileExtractor(httpClient);

        extractors.Add(library);
        extractors.Add(asset);
        extractors.Add(client);
        extractors.Add(log);
        extractors.Add(java);

        return extractors;
    }

    public IFileExtractor this[int index] => checkers[index];

    private readonly List<IFileExtractor> checkers;

    public FileExtractorCollection()
    {
        checkers = new List<IFileExtractor>(4);
    }

    public void Add(IFileExtractor item)
    {
        checkers.Add(item);
    }

    public void AddRange(IEnumerable<IFileExtractor?> items)
    {
        foreach (IFileExtractor? item in items)
        {
            if (item != null)
                Add(item);
        }
    }

    public void Remove(IFileExtractor item)
    {
        checkers.Remove(item);
    }

    public void RemoveAt(int index)
    {
        IFileExtractor item = checkers[index];
        Remove(item);
    }

    public void Insert(int index, IFileExtractor item)
    {
        checkers.Insert(index, item);
    }

    public IEnumerator<IFileExtractor> GetEnumerator()
    {
        return checkers.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return checkers.GetEnumerator();
    }
}
