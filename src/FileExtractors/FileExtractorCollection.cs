using System.Collections;

namespace CmlLib.Core.FileExtractors;

public class FileExtractorCollection : IEnumerable<IFileExtractor>
{
    public IFileExtractor this[int index] => checkers[index];

    private readonly List<IFileExtractor> checkers;

    public FileExtractorCollection()
    {
        checkers = new List<IFileExtractor>(5);
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
