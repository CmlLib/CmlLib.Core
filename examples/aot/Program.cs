using System.Text.Json;
using CmlLib.Core.Test.Fixtures;

if (JsonSerializer.IsReflectionEnabledByDefault)
    throw new InvalidOperationException("JSON reflection must be disabled for this smoke test.");

await AotJsonScenarios.Versions();
await AotJsonScenarios.JavaManifest();
await AotJsonScenarios.ModLoaders(quilt: false);
await AotJsonScenarios.ModLoaders(quilt: true);
await AotJsonScenarios.ModLoaders(quilt: false, empty: true);
await AotJsonScenarios.ModLoaders(quilt: true, empty: true);
await AotJsonScenarios.LiteLoader();
Console.WriteLine("Native AOT JSON smoke tests passed.");
