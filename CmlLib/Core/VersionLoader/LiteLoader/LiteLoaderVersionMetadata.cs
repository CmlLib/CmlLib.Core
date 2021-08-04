using System;
using System.IO;
using System.Linq;
using System.Xml.Schema;
using CmlLib.Core.Installer;
using CmlLib.Core.Version;
using CmlLib.Utils;
using Newtonsoft.Json.Linq;

namespace CmlLib.Core.VersionLoader.LiteLoader
{
    public class LiteLoaderVersionMetadata : MVersionMetadata
    {
        private const string LiteLoaderDl = "http://dl.liteloader.com/versions/";
        
        public LiteLoaderVersionMetadata(string id, string vanillaVersion, string? tweakClass, JArray? libs, string? llnName) : base(id)
        {
            IsLocalVersion = false;

            this.Name = id;
            
            this.vanillaVersionName = vanillaVersion;
            this.tweakClass = tweakClass;
            this.libraries = libs;
            this.llnName = llnName;
        }
        
        private readonly string vanillaVersionName;
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
            foreach (var item in libraries)
            {
                libs.Add(item);
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

        private void writeMetadata(string json, MinecraftPath path, string name)
        {
            var metadataPath = path.GetVersionJsonPath(name);
            
            var directoryPath = System.IO.Path.GetDirectoryName(metadataPath);
            if (!string.IsNullOrEmpty(directoryPath))
                Directory.CreateDirectory(directoryPath);
    
            File.WriteAllText(metadataPath, json);
        }

        public string Install(MinecraftPath path, MVersion baseVersion)
        {
            var versionName = LiteLoaderInstaller.GetVersionName(vanillaVersionName, baseVersion.Id);
            
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
            var json = createVersion(Name, vanillaVersionName, null, null).ToString();
            return MVersionParser.ParseFromJson(json);
        }

        public override MVersion GetVersion(MinecraftPath savePath)
        {
            var json = createVersion(Name, vanillaVersionName, null, null).ToString();
            writeMetadata(json, savePath, Name);
            return MVersionParser.ParseFromJson(json);
        }

        public override void Save(MinecraftPath path)
        {
            var json = createVersion(Name, vanillaVersionName, null, null).ToString();
            writeMetadata(json, path, Name);
        }
    }
}