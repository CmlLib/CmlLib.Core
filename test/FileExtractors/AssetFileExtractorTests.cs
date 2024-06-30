using CmlLib.Core.FileExtractors;
using CmlLib.Core.Files;
using CmlLib.Core.Internals;
using CmlLib.Core.Tasks;

namespace CmlLib.Core.Test.FileExtractors;

public class AssetFileExtractorTests
{
    public MinecraftPath TestPath = new()
    {
        Assets = "assets",
        Resource = "resources",
    };

    public string TestAssetServer = "https://assetserver";

    [Fact]
    public void create_common_asset_file()
    {
        var index = new MockAssetIndex()
        {
            IsVirtual = false,
            MapToResources = false,
            AssetObjects = 
            [
                new AssetObject("icons/icon_128x128.png", "b62ca8ec10d07e6bf5ac8dae0c8c1d2e6a1e3356", 9101)  
            ]
        };
        var result = AssetFileExtractor.Extractor.ExtractTasksFromAssetIndex(index, TestPath, TestAssetServer, false).Single();
        var expected = new GameFile("icons/icon_128x128.png")
        {
            Hash = "b62ca8ec10d07e6bf5ac8dae0c8c1d2e6a1e3356",
            Size = 9101,
            Path = IOUtil.NormalizePath("assets/objects/b6/b62ca8ec10d07e6bf5ac8dae0c8c1d2e6a1e3356"),
            Url = "https://assetserver/b6/b62ca8ec10d07e6bf5ac8dae0c8c1d2e6a1e3356",
            UpdateTask = result.UpdateTask
        };
        Assert.Equal(expected, result);
        Assert.Empty(result.UpdateTask);
    }

    [Fact]
    public void create_virtual_asset_file()
    {
        var index = new MockAssetIndex()
        {
            IsVirtual = true,
            MapToResources = false,
            AssetObjects = 
            [
                new AssetObject("icons/icon_128x128.png", "b62ca8ec10d07e6bf5ac8dae0c8c1d2e6a1e3356", 9101)
            ]
        };
        var result = AssetFileExtractor.Extractor.ExtractTasksFromAssetIndex(index, TestPath, TestAssetServer, false).ToArray();
        var expected = new GameFile("icons/icon_128x128.png")
        {
            Hash = "b62ca8ec10d07e6bf5ac8dae0c8c1d2e6a1e3356",
            Size = 9101,
            Path = IOUtil.NormalizePath("assets/objects/b6/b62ca8ec10d07e6bf5ac8dae0c8c1d2e6a1e3356"),
            Url = "https://assetserver/b6/b62ca8ec10d07e6bf5ac8dae0c8c1d2e6a1e3356",
            UpdateTask = result.First().UpdateTask
        };
        Assert.Equal([expected], result);
        
        var task = result.First().UpdateTask.First() as FileCopyTask;
        Assert.NotNull(task);
        Assert.Equal(IOUtil.NormalizePath("assets/virtual/legacy/icons/icon_128x128.png"), task!.DestinationPath);
    }

    [Fact]
    public void create_map_to_resources_asset_file()
    {
        var index = new MockAssetIndex()
        {
            IsVirtual = false,
            MapToResources = true,
            AssetObjects = 
            [
                new AssetObject("icons/icon_128x128.png", "b62ca8ec10d07e6bf5ac8dae0c8c1d2e6a1e3356", 9101)
            ]
        };
        var result = AssetFileExtractor.Extractor.ExtractTasksFromAssetIndex(index, TestPath, TestAssetServer, false).ToArray();
        var expected = new GameFile("icons/icon_128x128.png")
        {
            Hash = "b62ca8ec10d07e6bf5ac8dae0c8c1d2e6a1e3356",
            Size = 9101,
            Path = IOUtil.NormalizePath("assets/objects/b6/b62ca8ec10d07e6bf5ac8dae0c8c1d2e6a1e3356"),
            Url = "https://assetserver/b6/b62ca8ec10d07e6bf5ac8dae0c8c1d2e6a1e3356",
            UpdateTask = result.First().UpdateTask
        };
        Assert.Equal([expected], result);
        
        var task = result.First().UpdateTask.First() as FileCopyTask;
        Assert.NotNull(task);
        Assert.Equal(IOUtil.NormalizePath("resources/icons/icon_128x128.png"), task!.DestinationPath);
    }
}