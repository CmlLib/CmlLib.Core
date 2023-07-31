using System.Collections;
using System.Collections.Specialized;
using CmlLib.Core.Version;

namespace CmlLib.Core.VersionMetadata;

// Collection for IVersionMetadata
// return MVersion object from IVersionMetadata
public class MVersionCollection : IEnumerable<IVersionMetadata>
{
    public MVersionCollection()
        : this(Enumerable.Empty<IVersionMetadata>(), null, null)
    {

    }

    public MVersionCollection(
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
        if (name == null)
            throw new ArgumentNullException(nameof(name));
        
        var versionMetadata = Versions[name] as IVersionMetadata;
        if (versionMetadata == null)
            throw new KeyNotFoundException("Cannot find " + name);

        return versionMetadata;
    }

    public IVersionMetadata[] ToArray(MVersionSortOption option)
    {
        var sorter = new MVersionMetadataSorter(option);
        return sorter.Sort(this);
    }

    public Task<MVersion> GetVersionAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));

        var versionMetadata = GetVersionMetadata(name);
        return GetVersionAsync(versionMetadata);
    }

    public Task<MVersion> GetAndSaveVersionAsync(string name, MinecraftPath path)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));

        var versionMetadata = GetVersionMetadata(name);
        return GetAndSaveVersionAsync(versionMetadata, path);
    }

    public Task<MVersion> GetVersionAsync(IVersionMetadata versionMetadata) =>
        getVersionInternal(versionMetadata, null);

    public Task<MVersion> GetAndSaveVersionAsync(IVersionMetadata versionMetadata, MinecraftPath path) =>
        getVersionInternal(versionMetadata, path);

    private async Task<MVersion> getVersionInternal(IVersionMetadata versionMetadata, MinecraftPath? path)
    {
        if (versionMetadata == null)
            throw new ArgumentNullException(nameof(versionMetadata));

        MVersion version;
        if (path == null)
            version = await versionMetadata.GetVersionAsync();
        else
            version = await versionMetadata.GetAndSaveVersionAsync(path);

        await inheritIfRequired(version, path);
        return version;
    }

    private async Task inheritIfRequired(MVersion version, MinecraftPath? path)
    {
        if (version.IsInherited && !string.IsNullOrEmpty(version.ParentVersionId))
        {
            if (version.ParentVersionId == version.Id) // prevent StackOverFlowException
                throw new InvalidDataException(
                    "Invalid version json file : inheritFrom property is equal to id property.");

            var baseVersionMetadata = GetVersionMetadata(version.ParentVersionId);
            var baseVersion = await getVersionInternal(baseVersionMetadata, path);
            version.InheritFrom(baseVersion);
        }
    }

    public void AddVersion(IVersionMetadata version)
    {
        Versions[version.Name] = version;
    }

    public bool Contains(string? versionName)
        => !string.IsNullOrEmpty(versionName) && Versions.Contains(versionName);

    public void Merge(MVersionCollection from)
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
