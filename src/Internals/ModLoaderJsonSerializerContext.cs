using System.Text.Json.Serialization;
using CmlLib.Core.ModLoaders.FabricMC;
using CmlLib.Core.ModLoaders.QuiltMC;
using CmlLib.Core.ModLoaders.LiteLoader;

namespace CmlLib.Core.Internals;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(FabricLoader))]
[JsonSerializable(typeof(IReadOnlyCollection<FabricLoader>), TypeInfoPropertyName = "FabricLoaders")]
[JsonSerializable(typeof(QuiltLoader))]
[JsonSerializable(typeof(IReadOnlyCollection<QuiltLoader>), TypeInfoPropertyName = "QuiltLoaders")]
[JsonSerializable(typeof(LiteLoaderVersion))]
[JsonSerializable(typeof(LiteLoaderProfile))]
internal partial class ModLoaderJsonSerializerContext : JsonSerializerContext
{
}
