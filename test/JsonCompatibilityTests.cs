using CmlLib.Core.Test.Fixtures;

namespace CmlLib.Core.Test;

public class JsonCompatibilityTests
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public Task parse_mod_loader_responses(bool quilt, bool empty) => AotJsonScenarios.ModLoaders(quilt, empty);

    [Fact]
    public Task parse_java_manifest() => AotJsonScenarios.JavaManifest();

    [Fact]
    public Task write_and_read_liteloader_profile() => AotJsonScenarios.LiteLoader();

    [Fact]
    public Task parse_and_sort_versions() => AotJsonScenarios.Versions();
}
