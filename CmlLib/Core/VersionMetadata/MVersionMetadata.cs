using System;
using System.Threading.Tasks;
using CmlLib.Core.Version;
using Newtonsoft.Json;

namespace CmlLib.Core.VersionMetadata
{
    public abstract class MVersionMetadata
    {
        protected MVersionMetadata(string id)
        {
            this.Name = id;
        }

        public bool IsLocalVersion { get; set; }

        [JsonProperty("id")] public string Name { get; private set; }

        [JsonProperty("type")] public string? Type { get; set; }

        public MVersionType MType { get; set; }

        [JsonProperty("releaseTime")] public string? ReleaseTimeStr { get; set; }

        public DateTime? ReleaseTime
        {
            get
            {
                if (DateTime.TryParse(this.ReleaseTimeStr, out DateTime dt))
                    return dt;
                return null;
            }
        }
        
        [JsonProperty("url")] public string? Path { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            var info = obj as MVersionMetadata;

            if (info?.Name != null) // obj is MVersionMetadata
                return info.Name.Equals(Name);
            if (obj is string)
                return Name.Equals(obj.ToString());

            return false;
        }

        public override string ToString()
        {
            return Type + " " + Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public abstract MVersion GetVersion();
        public abstract MVersion GetVersion(MinecraftPath savePath);
        public abstract Task<MVersion> GetVersionAsync();
        public abstract Task<MVersion> GetVersionAsync(MinecraftPath savePath);
        public abstract void Save(MinecraftPath path);
        public abstract Task SaveAsync(MinecraftPath path);
    }
}
