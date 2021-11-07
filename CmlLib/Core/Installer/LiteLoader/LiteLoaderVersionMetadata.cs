using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CmlLib.Core.Version;
using CmlLib.Core.VersionMetadata;
using CmlLib.Utils;
using Newtonsoft.Json.Linq;

namespace CmlLib.Core.Installer.LiteLoader
{
    public class LiteLoaderVersionMetadata : MVersionMetadata
    {
        private const string LiteLoaderDl = "http://dl.liteloader.com/versions/";
        
        public LiteLoaderVersionMetadata(string id, string vanillaVersion, string? tweakClass, JArray? libs, string? llnName) : base(id)
        {
            IsLocalVersion = false;
            
            this.VanillaVersionName = vanillaVersion;
            this.tweakClass = tweakClass;
            this.libraries = libs;
            this.llnName = llnName;
        }
        
        public string VanillaVersionName { get; set; }
        private readonly string? tweakClass;
        private readonly JArray? libraries;
        private readonly string? llnName;

        private JObject createVersion(string versionName, string baseVersionName, string? strArgs, string?[]? arrArgs)
        {
            // add libraries
            var libs = JArray.FromObject(new[]
            {
                new
                {
                    name = llnName,
                    url = LiteLoaderDl
                }
            });

            if (libraries != null)
            {
                foreach (var item in libraries)
                {
                    libs.Add(item);
                }
            }

            // create object
            var obj = new
            {
                id = versionName,
                type = "release",
                libraries = libs,
                mainClass = "net.minecraft.launchwrapper.Launch",
                inheritsFrom = baseVersionName,
                jar = baseVersionName
            };
            
            var job = JObject.FromObject(obj);

            // set arguments
            if (!string.IsNullOrEmpty(strArgs))
                job["minecraftArguments"] = strArgs;
            if (arrArgs != null)
            {
                job["arguments"] = JObject.FromObject(new
                {
                    game = arrArgs
                });
            }
            
            return job;
        }

        private string prepareWriteMetadata(MinecraftPath path, string name)
        {
            var metadataPath = path.GetVersionJsonPath(name);
            
            var directoryPath = System.IO.Path.GetDirectoryName(metadataPath);
            if (!string.IsNullOrEmpty(directoryPath))
                Directory.CreateDirectory(directoryPath);

            return metadataPath;
        }

        private void writeMetadata(string json, MinecraftPath path, string name)
        {
            var metadataPath = prepareWriteMetadata(path, name);
            File.WriteAllText(metadataPath, json);
        }
        
        private Task writeMetadataAsync(string json, MinecraftPath path, string name)
        {
            var metadataPath = prepareWriteMetadata(path, name);
            return IOUtil.WriteFileAsync(metadataPath, json);
        }

        public string Install(MinecraftPath path, MVersion baseVersion)
        {
            var versionName = LiteLoaderInstaller.GetVersionName(VanillaVersionName, baseVersion.Id);
            
            if (!string.IsNullOrEmpty(baseVersion.MinecraftArguments))
            {
                // com.mumfrey.liteloader.launch.LiteLoaderTweaker
                var newArguments = $"--tweakClass {tweakClass} {baseVersion.MinecraftArguments}";
                var json = createVersion(versionName, baseVersion.Id, newArguments, null).ToString();
                writeMetadata(json, path, versionName);
            }
            else if (baseVersion.GameArguments != null)
            {
                var tweakArg = new []
                {
                    "--tweakClass",
                    tweakClass
                };

                var newArguments = tweakArg.Concat(baseVersion.GameArguments).ToArray();
                var json = createVersion(versionName, baseVersion.Id, null, newArguments).ToString();
                writeMetadata(json, path, versionName);
            }

            return versionName;
        }
        
        public override MVersion GetVersion()
        {
            var json = createVersion(Name, VanillaVersionName, null, null).ToString();
            return MVersionParser.ParseFromJson(json);
        }

        public override MVersion GetVersion(MinecraftPath savePath)
        {
            var json = createVersion(Name, VanillaVersionName, null, null).ToString();
            writeMetadata(json, savePath, Name);
            return MVersionParser.ParseFromJson(json);
        }

        public override Task<MVersion> GetVersionAsync()
        {
            return Task.FromResult(GetVersion());
        }

        public override async Task<MVersion> GetVersionAsync(MinecraftPath savePath)
        {
            var json = createVersion(Name, VanillaVersionName, null, null).ToString();
            await writeMetadataAsync(json, savePath, Name).ConfigureAwait(false);
            return MVersionParser.ParseFromJson(json);
        }

        public override void Save(MinecraftPath path)
        {
            var json = createVersion(Name, VanillaVersionName, null, null).ToString();
            writeMetadata(json, path, Name);
        }

        public override async Task SaveAsync(MinecraftPath path)
        {
            var json = createVersion(Name, VanillaVersionName, null, null).ToString();
            await writeMetadataAsync(json, path, Name).ConfigureAwait(false);
        }
    }
}