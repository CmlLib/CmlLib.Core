using CmlLib.Core.Files;

namespace CmlLib.Core.Test.FileExtractors;

public class MockAssetIndex : IAssetIndex
{
    public string Id { get; set; } = "mock";
    public bool IsVirtual { get; set; }
    public bool MapToResources { get; set; }
    public IEnumerable<AssetObject> AssetObjects { get; set; } = Enumerable.Empty<AssetObject>();
    public IEnumerable<AssetObject> EnumerateAssetObjects() => AssetObjects;
}