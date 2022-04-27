using System.Text.Json.Serialization;

namespace CmlLib.Core.Files
{
    // Represent common file metadata. most files in version.json file follow this form
    public class MFileMetadata
    {
        public MFileMetadata()
        {

        }

        public MFileMetadata(string? id)
        {
            this.Id = id;
        }

        [JsonPropertyName("id")]
        public string? Id { get; set; }
        
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        
        [JsonPropertyName("sha1")]
        public string? Sha1 { get; set; }

        [JsonPropertyName("size")]
        public long Size { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }
}
