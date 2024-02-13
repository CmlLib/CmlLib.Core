using CmlLib.Core.ProcessBuilder;
using CmlLib.Core.Test.Version;
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

    public async Task get_inherited_collection()
    {
        var (collection, parent, child) = createMocks();
        var version = await collection.GetVersionAsync("child");
        var concatArguments = version.ConcatInheritedCollection(v => v.GameArguments)
            .SelectMany(args => args.Values ?? Enumerable.Empty<string>())
            .ToArray();

        // should keep the order
        Assert.Equal(new []{ "parent_arg", "child_arg" }, concatArguments);
    }

    [Fact]
    public void throw_circular_inheritance()
    {
        var (collection, parent, child) = createMocks();
        parent.InheritsFrom = child.Id;

        Assert.ThrowsAsync<InvalidDataException>(async () =>
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
    public void throw_non_existent_parent_id()
    {
        var (collection, parent, child) = createMocks();
        child.InheritsFrom = "NON_EXISTENT_VERSION_ID";

        Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await collection.GetVersionAsync("child");
        });
    }

    [Fact]
    public void throw_too_deep_inheritance()
    {
        var v1 = new DummyVersion("v1");
        var v2 = new DummyVersion("v2");
        var v3 = new DummyVersion("v3");
        var v4 = new DummyVersion("v4");

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

        Assert.ThrowsAsync<InvalidDataException>(async () => 
        {
            await collection.GetVersionAsync("v4");
        });
    }

    private (VersionMetadataCollection, VersionMetadataCollection) createTestCollections()
    {
        var v1 = createMockVersionMetadata(new DummyVersion("v1"));
        var v2 = createMockVersionMetadata(new DummyVersion("v2"));
        var v3 = createMockVersionMetadata(new DummyVersion("v3"));
        var v4 = createMockVersionMetadata(new DummyVersion("v4"));

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

    private (VersionMetadataCollection, DummyVersion, DummyVersion) createMocks()
    {
        var parent = new DummyVersion("parent");
        parent.MainClass = "parent_mainclass";
        parent.AssetIndex = new Files.AssetMetadata
        {
            Id = "parent_assetindex"
        };
        parent.GameArguments = new MArgument[]
        {
            new MArgument("parent_arg")
        };

        var child = new DummyVersion("child");
        child.AssetIndex = new Files.AssetMetadata
        {
            Id = "child_assetindex"
        };
        child.GameArguments = new MArgument[]
        {
            new MArgument("child_arg")
        };

        child.InheritsFrom = parent.Id;        

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

        mock.Setup(v => v.GetVersionAsync().Result)
            .Returns(returnValue);

        return mock.Object;
    }
}