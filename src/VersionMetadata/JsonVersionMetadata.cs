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
    protected abstract ValueTask<Stream> GetVersionJsonStream(CancellationToken cancellationToken);

    public async Task<IVersion> GetVersionAsync(CancellationToken cancellationToken = default)
    {
        using var versionStream = await GetVersionJsonStream(cancellationToken).ConfigureAwait(false);
        return JsonVersionParser.ParseFromJsonStream(versionStream, new JsonVersionParserOptions());
    }

    public async Task<IVersion> GetAndSaveVersionAsync(MinecraftPath path, CancellationToken cancellationToken = default)
    {
        using var versionStream = await GetVersionJsonStream(cancellationToken).ConfigureAwait(false);
        if (IsSaved)
        {
            return JsonVersionParser.ParseFromJsonStream(versionStream, new JsonVersionParserOptions());
        }
        else
        {
            using var fs = createVersionJsonFileStream(path);
            using var piped = new PipedStream(versionStream, fs, writeToEndOnClose: true);
            return JsonVersionParser.ParseFromJsonStream(piped, new JsonVersionParserOptions());
        }
    }

    public async Task SaveVersionAsync(MinecraftPath path, CancellationToken cancellationToken = default)
    {
        using var versionStream = await GetVersionJsonStream(cancellationToken).ConfigureAwait(false);
        if (!IsSaved)
        {
            using var fs = createVersionJsonFileStream(path);
            await versionStream.CopyToAsync(fs, StreamHelper.GetCopyBufferSize(versionStream), cancellationToken);
        }
    }

    private Stream createVersionJsonFileStream(MinecraftPath path)
    {
        var metadataPath = path.GetVersionJsonPath(Name);
        IOUtil.CreateParentDirectory(metadataPath);
        return File.Create(metadataPath);
    }
}