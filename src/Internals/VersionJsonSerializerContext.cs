using System.Text.Json.Serialization;
using CmlLib.Core.Files;
using CmlLib.Core.Rules;
using CmlLib.Core.Version;
using CmlLib.Core.VersionMetadata;

namespace CmlLib.Core.Internals;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(JsonVersionManifestModel))]
[JsonSerializable(typeof(JsonVersionDTO))]
[JsonSerializable(typeof(MFileMetadata))]
[JsonSerializable(typeof(MLogFileMetadata))]
[JsonSerializable(typeof(LauncherRule))]
[JsonSerializable(typeof(Dictionary<string, MFileMetadata>), TypeInfoPropertyName = "FileMetadataDictionary")]
[JsonSerializable(typeof(Dictionary<string, string>), TypeInfoPropertyName = "StringDictionary")]
internal partial class VersionJsonSerializerContext : JsonSerializerContext
{
}
