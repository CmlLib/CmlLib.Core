using System.Text.Json;
using CmlLib.Core.Files;
using CmlLib.Core.Java;
using CmlLib.Core.ProcessBuilder;

namespace CmlLib.Core.Version;

public class JsonVersion : IVersion, IDisposable
{
    private readonly JsonVersionParserOptions _options;
    private readonly JsonDocument _json;
    private readonly JsonVersionDTO _model;

    public JsonVersion(JsonDocument jsonDocument, JsonVersionParserOptions options)
    {
        _options = options;
        _json = jsonDocument;
        _model = jsonDocument.RootElement.Deserialize<JsonVersionDTO>() ?? 
            throw new ArgumentNullException();
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

    private IReadOnlyCollection<MLibrary>? _libs = null;
    public IReadOnlyCollection<MLibrary> Libraries => _libs ??= getLibraries();

    public string? Jar => _model.Jar;

    private MLogFileMetadata? _logging;
    public MLogFileMetadata? Logging => _logging ??= getLogging();

    public string? MainClass => _model.MainClass;

    public string? MinecraftArguments => _model.MinecraftArguments;

    public DateTime ReleaseTime => _model.ReleaseTime;

    public string? Type => _model.Type;

    private IReadOnlyCollection<MArgument>? _gameArgs = null;
    public IReadOnlyCollection<MArgument> GameArguments => _gameArgs ??= getGameArguments();

    private IReadOnlyCollection<MArgument>? _jvmArgs = null;

    public IReadOnlyCollection<MArgument> JvmArguments => _jvmArgs ??= getJvmArguments();

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
            return _json.RootElement
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

    private IReadOnlyCollection<MLibrary> getLibraries()
    {
        try
        {
            var libProp = _json.RootElement.GetProperty("libraries");
            var libList = new List<MLibrary>();
            foreach (var libJson in libProp.EnumerateArray())
            {
                var lib = JsonLibraryParser.Parse(libJson);
                if (lib != null)
                    libList.Add(lib);
            }
            return libList;
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
            return _json.RootElement
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

    private IReadOnlyCollection<MArgument> getGameArguments()
    {
        try
        {
            var prop = _json.RootElement
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
                return args
                    .Split(' ')
                    .Select(arg => new MArgument(arg))
                    .ToArray();
            }
        }
        catch (Exception)
        {
            if (!_options.SkipError)
                throw;
            return Array.Empty<MArgument>();
        }
    }

    private IReadOnlyCollection<MArgument> getJvmArguments()
    {
        try
        {
            var prop = _json.RootElement
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
        if (_json.RootElement.TryGetProperty(key, out var prop))
            return prop.ToString();
        else
            return null;
    }

    public void Dispose()
    {
        _json.Dispose();
    }
}