using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CmlLib.Core.Version;

namespace CmlLib.Core.VersionMetadata
{
    /// <summary>
    /// Represent version metadata
    /// It does not contains actual version data, but contains some metadata and the way to get version data (MVersion)
    /// </summary>
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

        /// <summary>
        /// Get version data
        /// </summary>
        /// <returns>MVersion object containing actual version data</returns>
        public abstract Task<MVersion> GetVersionAsync();
        
        /// <summary>
        /// Get version data and save version data into file
        /// </summary>
        /// <param name="savePath">Game directory</param>
        /// <returns>MVersion object containing actual version data</returns>
        public abstract Task<MVersion> GetVersionAsync(MinecraftPath savePath);

        /// <summary>
        /// Get version data and save version data into file. This method may not make MVersion object
        /// </summary>
        /// <param name="path">Game directory</param>
        /// <returns></returns>
        public abstract Task SaveAsync(MinecraftPath path);
    }
}
