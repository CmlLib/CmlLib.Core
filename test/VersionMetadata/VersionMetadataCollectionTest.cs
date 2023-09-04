using CmlLib.Core.ProcessBuilder;
using CmlLib.Core.Test.Version;
using CmlLib.Core.Version;
using CmlLib.Core.VersionMetadata;
using Moq;
using NUnit.Framework;

namespace CmlLib.Core.Test.VersionMetadata;

public class VersionMetadataCollectionTest
{
    [Test]
    public void TestGetVersionMetadata()
    {
        var (collection, parent, child) = createMocks();

        var result = collection.GetVersionMetadata(parent.Id);
        Assert.That(result.Name, Is.EqualTo("parent"));
    }

    [Test]
    public void TestGetVersionMetadataException()
    {
        var (collection, parent, child) = createMocks();

        Assert.Throws<KeyNotFoundException>(() =>
        {
            collection.GetVersionMetadata("1234");
        });
    }

    [Test]
    public void TestEnumerationOrder()
    {
        var (collection, parent, child) = createMocks();

        var names = collection.Select(v => v.Name);
        Assert.That(names, Is.EqualTo(new string[] { "parent", "child" }));
    }

    [Test]
    public async Task TestInheritance()
    {
        var (collection, parent, child) = createMocks();

        var result = await collection.GetVersionAsync("child");
        Assert.That(result, Is.EqualTo(child));
        Assert.That(result.ParentVersion, Is.EqualTo(parent));
        Assert.That(result.GetInheritedProperty(v => v.AssetIndex)?.Id, Is.EqualTo("child_assetindex"));
        Assert.That(result.GetInheritedProperty(v => v.MainClass), Is.EqualTo("parent_mainclass"));

        var concatArguments = result.ConcatInheritedCollection(v => v.GameArguments)
            .SelectMany(args => args.Values ?? Enumerable.Empty<string>())
            .ToArray();
        Assert.That(concatArguments, Is.EqualTo(new string[] { "parent_arg", "child_arg" }));
    }

    [Test]
    public void TestCircularInheritanceException()
    {
        var (collection, parent, child) = createMocks();
        parent.InheritsFrom = child.Id;

        Assert.ThrowsAsync<InvalidDataException>(async () =>
        {
            await collection.GetVersionAsync("parent");
        });
    }

    [Test]
    public async Task TestSelfInheritance()
    {
        var (collection, parent, child) = createMocks();
        child.InheritsFrom = child.Id;

        var actual = await collection.GetVersionAsync("child");
        Assert.That(actual.InheritsFrom, Is.EqualTo(child.Id));
        Assert.Null(actual.ParentVersion);
    }

    [Test]
    public void TestNonExistInheritanceException()
    {
        var (collection, parent, child) = createMocks();
        child.InheritsFrom = "NON_EXISTENCE_VERSION_ID";

        Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await collection.GetVersionAsync("child");
        });
    }

    [Test]
    public void TestInheritanceOverflow()
    {
        var v1 = new MockVersion("v1");
        var v2 = new MockVersion("v2");
        var v3 = new MockVersion("v3");
        var v4 = new MockVersion("v4");

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

    [Test]
    public void TestMerge()
    {
        var v1 = createMockVersionMetadata(new MockVersion("v1"));
        var v2 = createMockVersionMetadata(new MockVersion("v2"));
        var v3 = createMockVersionMetadata(new MockVersion("v3"));
        var v4 = createMockVersionMetadata(new MockVersion("v4"));

        var collection1 = new VersionMetadataCollection(new IVersionMetadata[]
        {
            v1, v2, v3
        }, "v1", null);

        var collection2 = new VersionMetadataCollection(new IVersionMetadata[]
        {
            v2, v3, v4
        }, "v2", "v4");

        collection1.Merge(collection2);

        Assert.That(
            collection1.Select(v => v.Name), Is.EqualTo(
            new string[] { "v1", "v2", "v3", "v4" }),
            "Wrong order");
        
        Assert.That(collection1.LatestReleaseName, Is.EqualTo("v1"), "Wrong LatestReleaseName");
        Assert.That(collection1.LatestSnapshotName, Is.EqualTo("v4"), "Wrong LatestSnapshotName");
    }

    private (VersionMetadataCollection, MockVersion, MockVersion) createMocks()
    {
        var parent = new MockVersion("parent");
        parent.MainClass = "parent_mainclass";
        parent.AssetIndex = new Files.AssetMetadata
        {
            Id = "parent_assetindex"
        };
        parent.GameArguments = new MArgument[]
        {
            new MArgument("parent_arg")
        };

        var child = new MockVersion("child");
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