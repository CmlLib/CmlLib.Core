namespace CmlLib.Core.VersionMetadata;

/// <summary>
/// Represent metadata where the actual version data is in local file
/// </summary>
public class LocalVersionMetadata : JsonVersionMetadata
{
    public string Path { get; }

    public LocalVersionMetadata(JsonVersionMetadataModel model, string path) : base(model)
    {
        IsSaved = true;
        Path = path;
    }

    protected override ValueTask<Stream> GetVersionJsonStream(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(Path))
            throw new InvalidOperationException("Path property was null");
        
        // FileNotFoundException will be thrown if Path does not exist.
        return new ValueTask<Stream>(File.OpenRead(Path));
    }
}