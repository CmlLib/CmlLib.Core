using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CmlLib.Core.Version;
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

        private Task writeMetadata(string json, MinecraftPath path, string name, bool async)
        {
            var metadataPath = path.GetVersionJsonPath(name);
            
            var directoryPath = System.IO.Path.GetDirectoryName(metadataPath);
            if (!string.IsNullOrEmpty(directoryPath))
                Directory.CreateDirectory(directoryPath);

            if (async)
                return IOUtil.WriteFileAsync(metadataPath, json);
            
            File.WriteAllText(metadataPath, json);
            return Task.CompletedTask;
        }

        public string Install(MinecraftPath path, MVersion baseVersion)
        {
            var versionName = LiteLoaderInstaller.GetVersionName(VanillaVersionName, baseVersion.Id);
            
            if (!string.IsNullOrEmpty(baseVersion.MinecraftArguments))
            {
                // com.mumfrey.liteloader.launch.LiteLoaderTweaker
                var newArguments = $"--tweakClass {tweakClass} {baseVersion.MinecraftArguments}";
                var json = createVersion(versionName, baseVersion.Id, newArguments, null).ToString();
                writeMetadata(json, path, versionName, false).GetAwaiter().GetResult();
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
                writeMetadata(json, path, versionName, false).GetAwaiter().GetResult();
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
            writeMetadata(json, savePath, Name, false).GetAwaiter().GetResult();
            return MVersionParser.ParseFromJson(json);
        }

        public override Task<MVersion> GetVersionAsync()
        {
            return Task.FromResult(GetVersion());
        }

        public override async Task<MVersion> GetVersionAsync(MinecraftPath savePath)
        {
            var json = createVersion(Name, VanillaVersionName, null, null).ToString();
            await writeMetadata(json, savePath, Name, true);
            return MVersionParser.ParseFromJson(json);
        }

        public override void Save(MinecraftPath path)
        {
            var json = createVersion(Name, VanillaVersionName, null, null).ToString();
            writeMetadata(json, path, Name, false).GetAwaiter().GetResult();
        }

        public override async Task SaveAsync(MinecraftPath path)
        {
            var json = createVersion(Name, VanillaVersionName, null, null).ToString();
            await writeMetadata(json, path, Name, true);
        }
    }
}