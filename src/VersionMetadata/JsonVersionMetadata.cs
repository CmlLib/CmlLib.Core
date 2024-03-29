using CmlLib.Core.Version;
using CmlLib.Core.Internals;

namespace CmlLib.Core.VersionMetadata;

/// <summary>
/// Represent JSON text based version metadata
/// </summary>
public abstract class JsonVersionMetadata : IVersionMetadata
{
    public JsonVersionMetadata(JsonVersionMetadataModel model)
    {
        if (!string.IsNullOrEmpty(model.Id))
            Name = model.Id;
        else if (!string.IsNullOrEmpty(model.Name))
            Name = model.Name;
        else
            throw new ArgumentNullException(nameof(model.Id));

        Type = model.Type;
        ReleaseTime = model.ReleaseTime;
    }

    public bool IsSaved { get; set; }
    public string Name { get; }
    public string? Type { get; }
    public DateTimeOffset ReleaseTime { get; }

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
    protected abstract ValueTask<Stream> GetVersionJsonStream();

    public async Task<IVersion> GetVersionAsync()
    {
        using var versionStream = await GetVersionJsonStream().ConfigureAwait(false);
        return JsonVersionParser.ParseFromJsonStream(versionStream, new JsonVersionParserOptions());
    }

    public async Task<IVersion> GetAndSaveVersionAsync(MinecraftPath path)
    {
        using var versionStream = await GetVersionJsonStream().ConfigureAwait(false);
        var version = JsonVersionParser.ParseFromJsonStream(versionStream, new JsonVersionParserOptions());
        await saveMetdataJson(path, versionStream);
        return version;
    }

    public async Task SaveVersionAsync(MinecraftPath path)
    {
        using var versionStream = await GetVersionJsonStream().ConfigureAwait(false);
        await saveMetdataJson(path, versionStream);
    }

    private async Task saveMetdataJson(MinecraftPath path, Stream stream)
    {
        if (IsSaved) return;
        var metadataPath = path.GetVersionJsonPath(Name);
        IOUtil.CreateParentDirectory(metadataPath);
        await AsyncIO.WriteFileAsync(metadataPath, stream);
    }
}