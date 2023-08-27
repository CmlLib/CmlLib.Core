using System.Text.Json;
using CmlLib.Core.Files;
using CmlLib.Core.Java;
using CmlLib.Core.ProcessBuilder;

namespace CmlLib.Core.Version;

// JsonElement 의존성 빼고 parse 과정을 전부 JsonVersionParser 으로 이동

public class JsonVersion : IVersion
{
    private readonly JsonVersionParserOptions _options;
    private readonly JsonElement _json;
    private readonly VersionJsonModel _model;

    public JsonVersion(JsonElement json, JsonVersionParserOptions options)
    {
        _options = options;
        _json = json;
        _model = json.Deserialize<VersionJsonModel>() ?? throw new ArgumentNullException();
        Id = _model.Id ?? throw new ArgumentException("Null Id");
    }

    public string Id { get; }
    public string? JarId => getJarId();

    public string? InheritsFrom => _model.InheritsFrom;

    public IVersion? ParentVersion { get; set; }

    private AssetMetadata? _assetIndex;
    public AssetMetadata? AssetIndex => _assetIndex ??= getAssetIndex();

    private MFileMetadata? _client = null;
    public MFileMetadata? Client => _client ??= getClient();

    public JavaVersion? JavaVersion => _model.JavaVersion;

    private MLibrary[]? _libs = null;
    public MLibrary[] Libraries => _libs ??= getLibraries();

    public string? Jar => _model.Jar;

    private MLogFileMetadata? _logging;
    public MLogFileMetadata? Logging => _logging ??= getLogging();

    public string? MainClass => _model.MainClass;

    public string? MinecraftArguments => _model.MinecraftArguments;

    public DateTime ReleaseTime => _model.ReleaseTime;

    public string? Type => _model.Type;

    private MArgument[]? _gameArgs = null;
    public MArgument[] GameArguments => _gameArgs ??= getGameArguments();

    private MArgument[]? _jvmArgs = null;
    public MArgument[] JvmArguments => _jvmArgs ??= getJvmArguments();

    private string? getJarId()
    {
        var jar = this.GetInheritedProperty(v => v.Jar);
        if (string.IsNullOrEmpty(jar))
            return Id;
        return jar;
    }

    private AssetMetadata? getAssetIndex()
    {
        if (string.IsNullOrEmpty(_model.AssetIndex?.Id))
        {
            if (string.IsNullOrEmpty(_model.Assets))
                return null;
            else
                return new AssetMetadata() { Id = _model.Assets };
        }
        else
            return _model.AssetIndex;
    }

    private MFileMetadata? getClient()
    {
        try
        {
            return _json
                .GetProperty("downloads")
                .GetProperty(_options.Side)
                .Deserialize<MFileMetadata>();
        }
        catch (Exception)
        {
            if (!_options.SkipError)
                throw;
            return null;
        }
    }

    private MLibrary[] getLibraries()
    {
        try
        {
            var libProp = _json.GetProperty("libraries");
            var libList = new List<MLibrary>();
            foreach (var libJson in libProp.EnumerateArray())
            {
                var lib = JsonLibraryParser.Parse(libJson);
                if (lib != null)
                    libList.Add(lib);
            }
            return libList.ToArray();
        }
        catch (Exception)
        {
            if (!_options.SkipError)
                throw;
            return Array.Empty<MLibrary>();
        }
    }

    private MLogFileMetadata? getLogging()
    {
        try
        {
            return _json
                .GetProperty("logging")
                .GetProperty(_options.Side)
                .Deserialize<MLogFileMetadata>();
        }
        catch (Exception)
        {
            if (!_options.SkipError)
                throw;
            return null;
        }
    }

    private MArgument[] getGameArguments()
    {
        try
        {
            var prop = _json
                .GetProperty("arguments")
                .GetProperty("game");
            return JsonArgumentParser.Parse(prop);
        }
        catch (KeyNotFoundException)
        {
            var args = GetProperty("minecraftArguments");
            if (string.IsNullOrEmpty(args))
            {
                return Array.Empty<MArgument>();
            }
            else
            {
                return new MArgument[]
                {
                    new MArgument()
                    {
                        Values = new string[] { args }
                    }
                };
            }
        }
        catch (Exception)
        {
            if (!_options.SkipError)
                throw;
            return Array.Empty<MArgument>();
        }
    }

    private MArgument[] getJvmArguments()
    {
        try
        {
            var prop = _json
                .GetProperty("arguments")
                .GetProperty("jvm");
            return JsonArgumentParser.Parse(prop);
        }
        catch (KeyNotFoundException)
        {
            return Array.Empty<MArgument>();
        }
        catch (Exception)
        {
            if (!_options.SkipError)
                throw;
            return Array.Empty<MArgument>();
        }
    }

    public string? GetProperty(string key)
    {
        if (_json.TryGetProperty(key, out var prop))
            return prop.GetString();
        else
            return null;
    }
}