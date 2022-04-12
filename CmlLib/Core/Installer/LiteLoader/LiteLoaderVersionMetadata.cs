using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CmlLib.Core.Files;
using CmlLib.Core.Version;
using CmlLib.Core.VersionMetadata;
using CmlLib.Utils;

namespace CmlLib.Core.Installer.LiteLoader
{
    public class LiteLoaderVersionMetadata : MVersionMetadata
    {
        private const string LiteLoaderDl = "http://dl.liteloader.com/versions/"; // should be https

        public LiteLoaderVersionMetadata(string id, string vanillaVersionName) : base(id)
        {
            this.VanillaVersionName = vanillaVersionName;
        }
        
        public LiteLoaderVersionMetadata(string vanillaVersion, JsonElement element)
            : base($"LiteLoader-{vanillaVersion}")
        {
            IsLocalVersion = false;
            this.VanillaVersionName = vanillaVersion;
            this.element = element;
        }

        private readonly JsonElement? element;
        public string VanillaVersionName { get; private set; }

        private async Task writeVersion(Stream stream,
            string versionName, string baseVersionName, string? strArgs, string?[]? arrArgs)
        {
            await using var writer = new Utf8JsonWriter(stream);
            
            var llVersion = element?.GetPropertyValue("version");
            var libraries = element?.SafeGetProperty("libraries");
            
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
                    writer.WriteStringValue(item);
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

            return IOUtil.AsyncWriteStream(metadataPath, false);
        }

        public async Task<string> InstallAsync(MinecraftPath path, MVersion baseVersion)
        {
            var versionName = LiteLoaderInstaller.GetVersionName(VanillaVersionName, baseVersion.Id);
            var tweakClass = element?.GetPropertyValue("tweakClass");

            using var fs = createVersionWriteStream(path, versionName);
            
            if (!string.IsNullOrEmpty(baseVersion.MinecraftArguments))
            {
                // com.mumfrey.liteloader.launch.LiteLoaderTweaker
                var newArguments = $"--tweakClass {tweakClass} {baseVersion.MinecraftArguments}";
                await writeVersion(fs, versionName, baseVersion.Id, newArguments, null)
                    .ConfigureAwait(false);
            }
            else if (baseVersion.GameArguments != null)
            {
                var tweakArg = new []
                {
                    "--tweakClass",
                    tweakClass
                };

                var newArguments = tweakArg.Concat(baseVersion.GameArguments).ToArray();
                await writeVersion(fs, versionName, baseVersion.Id, null, newArguments)
                    .ConfigureAwait(false);
            }

            return versionName;
        }

        private async Task<Stream> writeVersionToMemory()
        {
            var ms = new MemoryStream();
            await writeVersion(ms, Name, VanillaVersionName, null, null)
                .ConfigureAwait(false);
            return ms;
        }
        
        public override async Task<MVersion> GetVersionAsync()
        {
            using var ms = await writeVersionToMemory()
                .ConfigureAwait(false);
            using var jsonDocument = await JsonDocument.ParseAsync(ms)
                .ConfigureAwait(false);
            return MVersionParser.ParseFromJson(jsonDocument);
        }

        public override async Task<MVersion> GetVersionAsync(MinecraftPath savePath)
        {
            using var ms = await writeVersionToMemory()
                .ConfigureAwait(false);
            using var fs = createVersionWriteStream(savePath, Name);

            using var jsonDocument = await JsonDocument.ParseAsync(ms);
            var copyTask = ms.CopyToAsync(fs)
                .ConfigureAwait(false);
            var version = MVersionParser.ParseFromJson(jsonDocument);
            await copyTask;
            return version;
        }

        public override async Task SaveAsync(MinecraftPath path)
        {
            using var fs = createVersionWriteStream(path, Name);
            await writeVersion(fs, Name, VanillaVersionName, null, null)
                .ConfigureAwait(false);
        }
    }
}