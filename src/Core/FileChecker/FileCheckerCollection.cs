using System.Collections;

namespace CmlLib.Core.FileChecker;

public class FileCheckerCollection : IEnumerable<IFileChecker>
{
    public static FileCheckerCollection CreateDefault(HttpClient httpClient)
    {
        var checkers = new FileCheckerCollection();

        var library = new LibraryChecker();
        var asset = new AssetChecker(httpClient);
        var client = new ClientChecker();
        var java = new JavaChecker(httpClient);
        var log = new LogChecker();

        checkers.Add(library);
        checkers.Add(asset);
        checkers.Add(client);
        checkers.Add(log);
        checkers.Add(java);

        return checkers;
    }

    public IFileChecker this[int index] => checkers[index];

    private readonly List<IFileChecker> checkers;

    public FileCheckerCollection()
    {
        checkers = new List<IFileChecker>(4);
    }

    public void Add(IFileChecker item)
    {
        checkers.Add(item);
    }

    public void AddRange(IEnumerable<IFileChecker?> items)
    {
        foreach (IFileChecker? item in items)
        {
            if (item != null)
                Add(item);
        }
    }

    public void Remove(IFileChecker item)
    {
        checkers.Remove(item);
    }

    public void RemoveAt(int index)
    {
        IFileChecker item = checkers[index];
        Remove(item);
    }

    public void Insert(int index, IFileChecker item)
    {
        checkers.Insert(index, item);
    }

    public IEnumerator<IFileChecker> GetEnumerator()
    {
        return checkers.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return checkers.GetEnumerator();
    }
}
