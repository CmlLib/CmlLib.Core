using System.Text.Json;
using CmlLib.Core.ProcessBuilder;
using CmlLib.Core.Version;
using CmlLib.Core.VersionMetadata;
using CmlLib.Core.Internals;

namespace CmlLib.Core.ModLoaders.LiteLoader;

public class LiteLoaderVersionMetadata : JsonVersionMetadata
{
    private const string LiteLoaderDl = "http://dl.liteloader.com/versions/"; // should be https

    public LiteLoaderVersionMetadata(
        JsonVersionMetadataModel model, 
        string vanillaVersionName, 
        JsonElement element) : base(model)
    {
        IsSaved = false;
        VanillaVersionName = vanillaVersionName;
        _element = element;
    }

    private readonly JsonElement? _element;
    public string VanillaVersionName { get; private set; }

    private void writeVersion(Utf8JsonWriter writer,
        string versionName, string baseVersionName, string? strArgs, IEnumerable<MArgument>? arrArgs)
    {
        var llVersion = _element?.GetPropertyValue("version");
        var libraries = _element?.GetPropertyOrNull("libraries");
        
        writer.WriteStartObject();
        writer.WriteString("id", versionName);
        writer.WriteString("type", "release");
        writer.WriteString("mainClass", "net.minecraft.launchwrapper.Launch");
        writer.WriteString("inheritsFrom", baseVersionName);
        writer.WriteString("jar", baseVersionName);
        
        writer.WriteStartArray("libraries");
        writer.WriteStartObject();
        writer.WriteString("name", $"{LiteLoaderVersionLoader.LiteLoaderLibName}:{llVersion}");
        writer.WriteString("url", LiteLoaderDl);
        writer.WriteEndObject();
        
        if (libraries != null)
        {
            foreach (var lib in libraries.Value.EnumerateArray())
            {
                // asm-all:5.2 is only available on LiteLoader server
                var libName = lib.GetPropertyValue("name");
                var libUrl = lib.GetPropertyValue("url");
                if (libName == "org.ow2.asm:asm-all:5.2")
                    libUrl = "http://repo.liteloader.com/";

                writer.WriteStartObject();
                writer.WriteString("name", libName);
                writer.WriteString("url", libUrl);
                writer.WriteEndObject();
            }
        }

        writer.WriteEndArray();
        
        if (!string.IsNullOrEmpty(strArgs))
            writer.WriteString("minecraftArguments", strArgs);
        if (arrArgs != null)
        {
            writer.WriteStartObject("arguments");
            writer.WriteStartArray();
            foreach (var item in arrArgs)
                JsonSerializer.Serialize(writer, item);
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
        
        writer.WriteEndObject();
    }
    
    private Stream createVersionWriteStream(MinecraftPath path, string name)
    {
        var metadataPath = path.GetVersionJsonPath(name);
        
        var directoryPath = System.IO.Path.GetDirectoryName(metadataPath);
        if (!string.IsNullOrEmpty(directoryPath))
            Directory.CreateDirectory(directoryPath);
        return File.Create(metadataPath);
    }

    public async Task<string> InstallAsync(MinecraftPath path, IVersion baseVersion)
    {
        var versionName = LiteLoaderInstaller.GetVersionName(VanillaVersionName, baseVersion.Id);
        var tweakClass = _element?.GetPropertyValue("tweakClass");

        using var fs = createVersionWriteStream(path, versionName);
        using var writer = new Utf8JsonWriter(fs);
        
        var minecraftArguments = baseVersion.GetProperty("minecraftArguments");
        if (!string.IsNullOrEmpty(minecraftArguments))
        {
            // com.mumfrey.liteloader.launch.LiteLoaderTweaker
            var newArguments = $"--tweakClass {tweakClass} {minecraftArguments}";
            writeVersion(writer, versionName, baseVersion.Id, newArguments, null);
        }
        else if (baseVersion.GameArguments.Any())
        {
            var tweakArg = new MArgument[]
            {
                new MArgument("--tweakClass"),
                new MArgument(tweakClass!)
            };

            var newArguments = tweakArg.Concat(baseVersion.GameArguments);
            writeVersion(writer, versionName, baseVersion.Id, null, newArguments);
        }

        await fs.FlushAsync();
        await writer.FlushAsync();
        return versionName;
    }

    protected override ValueTask<string> GetVersionJsonString()
    {
        using var ms = new MemoryStream();
        using var writer = new Utf8JsonWriter(ms);
        var json = System.Text.Encoding.UTF8.GetString(ms.ToArray());
        return new ValueTask<string>(json);
    }
}