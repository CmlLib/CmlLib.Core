using CmlLib.Core.Internals;
using System.Text.Json;

namespace CmlLib.Core.Files;

public interface IAssetIndex
{
    string Id { get; }
    bool IsVirtual { get; }
    bool MapToResources { get; }
    IEnumerable<AssetObject> EnumerateAssetObjects();
}

public class JsonAssetIndex : IAssetIndex, IDisposable
{
    private readonly JsonDocument _json;
    private readonly Lazy<bool> isVirtualLoader;
    private readonly Lazy<bool> mapToResourcesLoader;

    public JsonAssetIndex(string id, JsonDocument json)
    {
        Id = id;
        _json = json;
        isVirtualLoader = new Lazy<bool>(() =>
            _json.RootElement.GetPropertyOrNull("virtual")?.GetBoolean() ?? false);
        mapToResourcesLoader = new Lazy<bool>(() =>
            _json.RootElement.GetPropertyOrNull("map_to_resources")?.GetBoolean() ?? false);
    }

    public string Id { get; }
    public bool IsVirtual => isVirtualLoader.Value;
    public bool MapToResources => mapToResourcesLoader.Value;

    public IEnumerable<AssetObject> EnumerateAssetObjects()
    {
        if (!_json.RootElement.TryGetProperty("objects", out var objectsProp))
            yield break;

        var objects = objectsProp.EnumerateObject();
        foreach (var prop in objects)
        {
            var name = prop.Name;
            var hash = prop.Value.GetPropertyValue("hash");
            var size = prop.Value.GetPropertyOrNull("size")?.GetInt64() ?? 0;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(hash))
                continue;

            yield return new AssetObject(name, hash, size);
        }
    }

    public void Dispose() => _json.Dispose();
}

public record AssetObject(
    string Name, 
    string Hash, 
    long Size);