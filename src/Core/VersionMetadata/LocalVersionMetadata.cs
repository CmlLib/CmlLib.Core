using CmlLib.Utils;

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

    protected override async ValueTask<string> GetVersionJsonString()
    {
        if (string.IsNullOrEmpty(Path))
            throw new InvalidOperationException("Path property was null");
        
        // FileNotFoundException will be thrown if Path does not exist.
        return await IOUtil.ReadFileAsync(Path);
    }
}