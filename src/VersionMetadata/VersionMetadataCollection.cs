using System.Collections;
using System.Collections.Specialized;
using CmlLib.Core.Version;

namespace CmlLib.Core.VersionMetadata;

// Collection for IVersionMetadata
// return IVersion object from IVersionMetadata
public class VersionMetadataCollection : IEnumerable<IVersionMetadata>
{
    public VersionMetadataCollection()
        : this(Enumerable.Empty<IVersionMetadata>(), null, null)
    {

    }

    public VersionMetadataCollection(
        IEnumerable<IVersionMetadata> versions,
        string? latestRelease,
        string? latestSnapshot)
    {
        if (versions == null)
            throw new ArgumentNullException(nameof(versions));

        Versions = new OrderedDictionary();
        foreach (var item in versions)
        {
            Versions.Add(item.Name, item);
        }

        LatestReleaseName = latestRelease;
        LatestSnapshotName = latestSnapshot;
    }

    // Use OrderedDictionary to keep version order
    protected OrderedDictionary Versions;

    public int MaxDepth { get; set; } = 10;
    public string? LatestReleaseName { get; private set; }
    public string? LatestSnapshotName { get; private set; }

    public IVersionMetadata this[int index] => (IVersionMetadata)Versions[index]!;

    public IVersionMetadata GetVersionMetadata(string name)
    {
        if (TryGetVersionMetadata(name, out var version))
            return version;
        else
            throw new KeyNotFoundException("Cannot find " + name);
    }

    public bool TryGetVersionMetadata(string name, out IVersionMetadata version)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        var metadata = Versions[name] as IVersionMetadata;
        version = metadata!;
        return metadata != null;
    }

    public IVersionMetadata[] ToArray(MVersionSortOption option)
    {
        var sorter = new VersionMetadataSorter(option);
        return sorter.Sort(this);
    }

    public Task<IVersion> GetVersionAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));

        var versionMetadata = GetVersionMetadata(name);
        return GetVersionAsync(versionMetadata, cancellationToken);
    }

    public Task<IVersion> GetAndSaveVersionAsync(
        string name, 
        MinecraftPath path, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));

        var versionMetadata = GetVersionMetadata(name);
        return GetAndSaveVersionAsync(versionMetadata, path, cancellationToken);
    }

    public Task<IVersion> GetVersionAsync(
        IVersionMetadata versionMetadata, 
        CancellationToken cancellationToken = default) =>
        getVersionInternal(versionMetadata, null, new List<string>(), cancellationToken);

    public Task<IVersion> GetAndSaveVersionAsync(
        IVersionMetadata versionMetadata, 
        MinecraftPath path, 
        CancellationToken cancellationToken = default) =>
        getVersionInternal(versionMetadata, path, new List<string>(), cancellationToken);

    private async Task<IVersion> getVersionInternal(
        IVersionMetadata versionMetadata, 
        MinecraftPath? path, 
        List<string> visited, 
        CancellationToken cancellationToken = default)
    {
        if (versionMetadata == null)
            throw new ArgumentNullException(nameof(versionMetadata));

        IVersion version;
        if (path == null)
            version = await versionMetadata.GetVersionAsync(cancellationToken);
        else
            version = await versionMetadata.GetAndSaveVersionAsync(path, cancellationToken);

        await inheritIfRequired(version, path, visited, cancellationToken);
        return version;
    }

    private async Task inheritIfRequired(
        IVersion version, 
        MinecraftPath? path, 
        List<string> visited, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(version.InheritsFrom))
            return;
        if (version.InheritsFrom == version.Id)
            return;

        if (visited.Contains(version.InheritsFrom))
            throw VersionDependencyException.CreateCircularDependencyException(version.Id);
        visited.Add(version.Id);
        if (visited.Count >= MaxDepth)
            throw VersionDependencyException.CreateExcessiveDepthMessage(MaxDepth, version.Id);

        var baseVersionMetadata = GetVersionMetadata(version.InheritsFrom);
        var baseVersion = await getVersionInternal(baseVersionMetadata, path, visited, cancellationToken);
        version.ParentVersion = baseVersion;
    }

    public void AddVersion(IVersionMetadata version)
    {
        Versions[version.Name] = version;
    }

    public bool Contains(string? versionName)
        => !string.IsNullOrEmpty(versionName) && Versions.Contains(versionName);

    public void Merge(VersionMetadataCollection from)
    {
        foreach (var item in from)
        {
            if (!Contains(item.Name))
            {
                Versions[item.Name] = item;
            }
        }

        if (string.IsNullOrEmpty(this.LatestReleaseName))
            this.LatestReleaseName = from.LatestReleaseName;
        if (string.IsNullOrEmpty(this.LatestSnapshotName))
            this.LatestSnapshotName = from.LatestSnapshotName;
    }

    public IEnumerator<IVersionMetadata> GetEnumerator()
    {
        foreach (DictionaryEntry? item in Versions)
        {
            var entry = item.Value;

            var version = (IVersionMetadata)entry.Value!;
            yield return version;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        foreach (DictionaryEntry? item in Versions)
        {
            yield return item.Value;
        }
    }
}
