using CmlLib.Core.VersionMetadata;

namespace CmlLib.Core.Test.VersionMetadata;

public class MVersionTypeConverterTests
{
    [Fact]
    public void convert_to_string()
    {
        Assert.Equal("release", MVersionType.Release.ConvertToTypeString());
        Assert.Equal("snapshot", MVersionType.Snapshot.ConvertToTypeString());
        Assert.Equal("old_alpha", MVersionType.OldAlpha.ConvertToTypeString());
        Assert.Equal("old_beta", MVersionType.OldBeta.ConvertToTypeString());
        Assert.Equal("custom", MVersionType.Custom.ConvertToTypeString());
        Assert.Equal("custom", ((MVersionType)14).ConvertToTypeString());
    }

    [Fact]
    public void parse_from_string()
    {
        Assert.Equal(MVersionType.Release, MVersionTypeConverter.Parse("release"));
        Assert.Equal(MVersionType.Snapshot, MVersionTypeConverter.Parse("snapshot"));
        Assert.Equal(MVersionType.OldAlpha, MVersionTypeConverter.Parse("old_alpha"));
        Assert.Equal(MVersionType.OldBeta, MVersionTypeConverter.Parse("old_beta"));
        Assert.Equal(MVersionType.Custom, MVersionTypeConverter.Parse("custom"));
        Assert.Equal(MVersionType.Custom, MVersionTypeConverter.Parse(""));
        Assert.Equal(MVersionType.Custom, MVersionTypeConverter.Parse("1"));
    }
}