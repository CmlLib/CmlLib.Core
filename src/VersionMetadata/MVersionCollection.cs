using System.Collections;
using System.Collections.Specialized;
using CmlLib.Core.Version;

namespace CmlLib.Core.VersionMetadata;

// Collection for IVersionMetadata
// return IVersion object from IVersionMetadata
public class VersionCollection : IEnumerable<IVersionMetadata>
{
    public VersionCollection()
        : this(Enumerable.Empty<IVersionMetadata>(), null, null)
    {

    }

    public VersionCollection(
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
        var sorter = new MVersionMetadataSorter(option);
        return sorter.Sort(this);
    }

    public Task<IVersion> GetVersionAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));

        var versionMetadata = GetVersionMetadata(name);
        return GetVersionAsync(versionMetadata);
    }

    public Task<IVersion> GetAndSaveVersionAsync(string name, MinecraftPath path)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));

        var versionMetadata = GetVersionMetadata(name);
        return GetAndSaveVersionAsync(versionMetadata, path);
    }

    public Task<IVersion> GetVersionAsync(IVersionMetadata versionMetadata) =>
        getVersionInternal(versionMetadata, null);

    public Task<IVersion> GetAndSaveVersionAsync(IVersionMetadata versionMetadata, MinecraftPath path) =>
        getVersionInternal(versionMetadata, path);

    private async Task<IVersion> getVersionInternal(IVersionMetadata versionMetadata, MinecraftPath? path)
    {
        if (versionMetadata == null)
            throw new ArgumentNullException(nameof(versionMetadata));

        IVersion version;
        if (path == null)
            version = await versionMetadata.GetVersionAsync();
        else
            version = await versionMetadata.GetAndSaveVersionAsync(path);

        await inheritIfRequired(version, path);
        return version;
    }

    private async Task inheritIfRequired(IVersion version, MinecraftPath? path)
    {
        if (!string.IsNullOrEmpty(version.InheritsFrom))
        {
            if (version.InheritsFrom == version.Id) // prevent StackOverFlowException
                throw new InvalidDataException(
                    "Invalid version json file : inheritFrom property is equal to id property.");

            var baseVersionMetadata = GetVersionMetadata(version.InheritsFrom);
            var baseVersion = await getVersionInternal(baseVersionMetadata, path);
            version.ParentVersion = baseVersion;
        }
    }

    public void AddVersion(IVersionMetadata version)
    {
        Versions[version.Name] = version;
    }

    public bool Contains(string? versionName)
        => !string.IsNullOrEmpty(versionName) && Versions.Contains(versionName);

    public void Merge(VersionCollection from)
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
