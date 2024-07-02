using CmlLib.Core.ProcessBuilder;
using CmlLib.Core.Version;
using CmlLib.Core.VersionMetadata;
using Moq;

namespace CmlLib.Core.Test.VersionMetadata;

public class VersionMetadataCollectionTest
{
    [Fact]
    public void get_version_by_id()
    {
        var (collection, parent, child) = createMocks();
        var result = collection.GetVersionMetadata(parent.Id);
        Assert.Equal("parent", result.Name);
    }

    [Fact]
    public void get_version_by_not_existing_id()
    {
        var (collection, parent, child) = createMocks();

        Assert.Throws<KeyNotFoundException>(() =>
        {
            collection.GetVersionMetadata("1234");
        });
    }

    [Fact]
    public void keep_enumeration_order()
    {
        var (collection, parent, child) = createMocks();
        var names = collection.Select(v => v.Name);
        Assert.Equal(new [] { "parent", "child" }, names);
    }

    [Fact]
    public async Task get_inherited_property()
    {
        var (collection, parent, child) = createMocks();
        var version = await collection.GetVersionAsync("child");
        Assert.Equal("child_assetindex", version.GetInheritedProperty(v => v.AssetIndex)?.Id);
        Assert.Equal("parent_mainclass", version.GetInheritedProperty(v => v.MainClass));
    }

    [Fact]
    public async Task get_inherited_collection()
    {
        var (collection, parent, child) = createMocks();
        var version = await collection.GetVersionAsync("child");
        var concatLibraries = version.ConcatInheritedCollection(v => v.Libraries).Select(lib => lib.Name);

        // should keep the order
        Assert.Equal(["parent_lib", "child_lib"], concatLibraries);
    }

    [Fact]
    public async Task get_game_arguments_with_base_arguments()
    {
        var (collection, parent, child) = createMocks();
        var version = await collection.GetVersionAsync("child");
        var gameArgs = version.ConcatInheritedGameArguments()
            .SelectMany(arg => arg.Values)
            .ToArray();

        // should keep the order
        Assert.Equal(["parent_base_arg", "child_arg"], gameArgs);
    }

    [Fact]
    public async Task get_jvm_arguments_with_base_arguments()
    {
        var (collection, parent, child) = createMocks();
        var version = await collection.GetVersionAsync("child");
        var gameArgs = version.ConcatInheritedJvmArguments()
            .SelectMany(arg => arg.Values)
            .ToArray();

        // should keep the order
        Assert.Equal(["parent_base_arg", "child_arg"], gameArgs);
    }

    [Fact]
    public async Task throw_circular_inheritance()
    {
        var (collection, parent, child) = createMocks();
        parent.InheritsFrom = child.Id;

        await Assert.ThrowsAsync<VersionDependencyException>(async () =>
        {
            await collection.GetVersionAsync("parent");
        });
    }

    [Fact]
    public async Task handle_self_inheritance()
    {
        // Given
        var (collection, parent, child) = createMocks();
        child.InheritsFrom = child.Id;

        // When
        var selfInherited = await collection.GetVersionAsync("child");

        // Then
        Assert.Equal("child", selfInherited.InheritsFrom);
        Assert.Null(selfInherited.ParentVersion);
    }

    [Fact]
    public async Task throw_non_existent_parent_id()
    {
        var (collection, parent, child) = createMocks();
        child.InheritsFrom = "NON_EXISTENT_VERSION_ID";

        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await collection.GetVersionAsync("child");
        });
    }

    [Fact]
    public async Task throw_too_deep_inheritance()
    {
        var v1 = new MinecraftVersion("v1");
        var v2 = new MinecraftVersion("v2");
        var v3 = new MinecraftVersion("v3");
        var v4 = new MinecraftVersion("v4");

        v4.InheritsFrom = v3.Id;
        v3.InheritsFrom = v2.Id;
        v2.InheritsFrom = v1.Id;

        var collection = new VersionMetadataCollection(new IVersionMetadata[]
        {
            createMockVersionMetadata(v1),
            createMockVersionMetadata(v2),
            createMockVersionMetadata(v3),
            createMockVersionMetadata(v4)
        }, null, null);

        collection.MaxDepth = 3;

        await Assert.ThrowsAsync<VersionDependencyException>(async () => 
        {
            await collection.GetVersionAsync("v4");
        });
    }

    private (VersionMetadataCollection, VersionMetadataCollection) createTestCollections()
    {
        var v1 = createMockVersionMetadata(new MinecraftVersion("v1"));
        var v2 = createMockVersionMetadata(new MinecraftVersion("v2"));
        var v3 = createMockVersionMetadata(new MinecraftVersion("v3"));
        var v4 = createMockVersionMetadata(new MinecraftVersion("v4"));

        var collection1 = new VersionMetadataCollection(new IVersionMetadata[]
        {
            v1, v2, v3
        }, "v1", null);

        var collection2 = new VersionMetadataCollection(new IVersionMetadata[]
        {
            v2, v3, v4
        }, "v2", "v4");

        return (collection1, collection2);
    }

    [Fact]
    public void keep_enumeration_order_after_merging()
    {
        var (collection1, collection2) = createTestCollections();
        collection1.Merge(collection2);
        Assert.Equal(new [] { "v1", "v2", "v3", "v4" }, collection1.Select(v => v.Name));
    }

    [Fact]
    public void merge_latest_version_names()
    {
        var (collection1, collection2) = createTestCollections();
        collection1.Merge(collection2);
        Assert.Equal("v1", collection1.LatestReleaseName);
        Assert.Equal("v4", collection1.LatestSnapshotName);
    }

    private (VersionMetadataCollection, MinecraftVersion, MinecraftVersion) createMocks()
    {
        var parent = new MinecraftVersion("parent")
        {
            MainClass = "parent_mainclass",
            AssetIndex = new Files.AssetMetadata
            {
                Id = "parent_assetindex"
            },
            GameArguments = [new MArgument("parent_arg")],
            GameArgumentsForBaseVersion = [new MArgument("parent_base_arg")],
            JvmArguments = [new MArgument("parent_arg")],
            JvmArgumentsForBaseVersion = [new MArgument("parent_base_arg")],
            LibraryList = [new MLibrary("parent_lib")]
        };

        var child = new MinecraftVersion("child")
        {
            AssetIndex = new Files.AssetMetadata
            {
                Id = "child_assetindex"
            },
            GameArguments = [new MArgument("child_arg")],
            GameArgumentsForBaseVersion = [new MArgument("child_base_arg")],
            JvmArguments = [new MArgument("child_arg")],
            JvmArgumentsForBaseVersion = [new MArgument("child_base_arg")],
            LibraryList = [new MLibrary("child_lib")],

            InheritsFrom = parent.Id
        };

        var collection = new VersionMetadataCollection(
            new IVersionMetadata[]
            {
                createMockVersionMetadata(parent),
                createMockVersionMetadata(child)
            }, 
            null,
            null
        );

        return (collection, parent, child);
    }

    private IVersionMetadata createMockVersionMetadata(IVersion returnValue)
    {
        var mock = new Mock<IVersionMetadata>();

        mock.Setup(v => v.Name)
            .Returns(returnValue.Id);

        mock.Setup(v => v.GetVersionAsync(default).Result)
            .Returns(returnValue);

        return mock.Object;
    }
}