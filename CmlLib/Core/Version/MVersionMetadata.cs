using Newtonsoft.Json;
using System;

namespace CmlLib.Core.Version
{
    public class MVersionMetadata
    {

        // Will be replaced by IsLocalVersion
        [Obsolete("Use IsLocalVersion")]
        public bool IsWeb = true;

        public bool IsLocalVersion { get => !IsWeb; }

        [JsonProperty("id")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        public MVersionType MType { get; set; }

        [JsonProperty("releaseTime")]
        public string ReleaseTime { get; set; }

        [JsonProperty("url")]
        public string Path { get; set; }

        public override bool Equals(object obj)
        {
            var info = obj as MVersionMetadata;

            if (info != null)
                return info.Name.Equals(Name);
            else if (obj is string)
                return info.Name.Equals(obj.ToString());
            else
                return base.Equals(obj);
        }

        public override string ToString()
        {
            return Type + " " + Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
