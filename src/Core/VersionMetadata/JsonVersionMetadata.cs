using CmlLib.Core.Version;
using CmlLib.Utils;

namespace CmlLib.Core.VersionMetadata;

/// <summary>
/// Represent JSON text based version metadata
/// </summary>
public abstract class JsonVersionMetadata : IVersionMetadata
{
    public JsonVersionMetadata(JsonVersionMetadataModel model)
    {
        if (string.IsNullOrEmpty(model.Name))
            throw new ArgumentNullException(nameof(model.Name));
        Name = model.Name;
        Type = model.Type;
        ReleaseTime = model.ReleaseTime;
    }

    public bool IsSaved { get; set; }
    public string Name { get; }
    public string? Type { get; }
    public DateTime? ReleaseTime { get; }


    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;

        var info = obj as IVersionMetadata;

        if (info?.Name != null) // obj is MVersionMetadata
            return info.Name.Equals(Name);
        if (obj is string)
            return Name.Equals(obj.ToString());

        return false;
    }

    public override string ToString()
    {
        return Type + " " + Name;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
    
    /// <summary>
    /// Get actual version data as string
    /// </summary>
    /// <returns>Version metadata</returns>
    protected abstract ValueTask<string> GetVersionJsonString();

    public async Task<MVersion> GetVersionAsync()
    {
        var metadataJson = await GetVersionJsonString().ConfigureAwait(false);
        return new JsonVersionParser().ParseFromJsonString(metadataJson);
    }

    public async Task<MVersion> GetAndSaveVersionAsync(MinecraftPath path)
    {
        var metadataJson = await GetVersionJsonString().ConfigureAwait(false);
        var version = new JsonVersionParser().ParseFromJsonString(metadataJson);
        await saveMetdataJson(path, metadataJson);
        return version;
    }

    public async Task SaveVersionAsync(MinecraftPath path)
    {
        var metadataJson = await GetVersionJsonString().ConfigureAwait(false);
        await saveMetdataJson(path, metadataJson);
    }

    private async Task saveMetdataJson(MinecraftPath path, string json)
    {
        if (IsSaved)
            return;
        var metadataPath = path.GetVersionJsonPath(Name);
        IOUtil.CreateParentDirectory(metadataPath);
        await IOUtil.WriteFileAsync(metadataPath, json).ConfigureAwait(false);
    }
}