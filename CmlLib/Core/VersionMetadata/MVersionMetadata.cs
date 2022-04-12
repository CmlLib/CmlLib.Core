using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CmlLib.Core.Version;

namespace CmlLib.Core.VersionMetadata
{
    public abstract class MVersionMetadata
    {
        protected MVersionMetadata(string name)
        {
            this.Name = name;
        }

        public bool IsLocalVersion { get; set; }

        [JsonPropertyName("id")] 
        public string Name { get; private set; }

        [JsonPropertyName("type")] 
        public string? Type { get; set; }

        public MVersionType MType { get; set; }

        [JsonPropertyName("releaseTime")] 
        public string? ReleaseTimeStr { get; set; }
        [JsonIgnore]
        public DateTime? ReleaseTime
        {
            get
            {
                if (DateTime.TryParse(this.ReleaseTimeStr, out DateTime dt))
                    return dt;
                return null;
            }
        }
        
        [JsonPropertyName("url")] 
        public string? Path { get; set; }

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

        public abstract Task<MVersion> GetVersionAsync();
        public abstract Task<MVersion> GetVersionAsync(MinecraftPath savePath);
        public abstract Task SaveAsync(MinecraftPath path);
    }
}
