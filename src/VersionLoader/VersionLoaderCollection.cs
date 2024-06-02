using System.Collections;
using CmlLib.Core.VersionMetadata;

namespace CmlLib.Core.VersionLoader;

public class VersionLoaderCollection : IVersionLoader, ICollection<IVersionLoader>
{
    private readonly List<IVersionLoader> _collection = new();

    public async ValueTask<VersionMetadataCollection> GetVersionMetadatasAsync(CancellationToken cancellationToken = default)
    {
        var versions = new VersionMetadataCollection();
        foreach (var loader in _collection)
        {
            var newVersions = await loader.GetVersionMetadatasAsync(cancellationToken);
            versions.Merge(newVersions);
        }
        return versions;
    }

    // ICollection implementation
    public int Count => _collection.Count();
    public bool IsReadOnly => false;
    public void Add(IVersionLoader item) => _collection.Add(item);
    public void Clear() => _collection.Clear();
    public bool Contains(IVersionLoader item) => _collection.Contains(item);
    public void CopyTo(IVersionLoader[] array, int arrayIndex) => _collection.CopyTo(array, arrayIndex);
    public IEnumerator<IVersionLoader> GetEnumerator() => _collection.GetEnumerator();
    public bool Remove(IVersionLoader item) => _collection.Remove(item);
    IEnumerator IEnumerable.GetEnumerator() => _collection.GetEnumerator();
}
