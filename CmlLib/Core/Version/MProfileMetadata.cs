using Newtonsoft.Json;
using System;

namespace CmlLib.Core.Version
{
    public class MProfileMetadata
    {

        // Will be replaced by IsLocalProfile
        [Obsolete("Use IsLocalProfile")]
        public bool IsWeb = true;

        public bool IsLocalProfile { get => !IsWeb; }

        [JsonProperty("id")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        public MProfileType MType { get; set; }

        [JsonProperty("releaseTime")]
        public string ReleaseTime { get; set; }

        [JsonProperty("url")]
        public string Path { get; set; }

        public override bool Equals(object obj)
        {
            var info = obj as MProfileMetadata;

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
