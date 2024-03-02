using System.Text.Json;
using CmlLib.Core.FileExtractors;
using CmlLib.Core.Files;
using CmlLib.Core.Internals;
using CmlLib.Core.Rules;
using CmlLib.Core.Test.Version;
using CmlLib.Core.Version;

namespace CmlLib.Core.Test.FileExtractors;

public class LibraryFileExtractorTests
{
    MinecraftPath TestPath = new()
    {
        Library = "libraries"
    };
    string TestServer = "https://libraryserver";
    RulesEvaluatorContext TestWindows = new RulesEvaluatorContext(new LauncherOSRule(LauncherOSRule.Windows, LauncherOSRule.X64));
    RulesEvaluatorContext TestOSX = new RulesEvaluatorContext(new LauncherOSRule(LauncherOSRule.OSX, LauncherOSRule.X64));
    RulesEvaluatorContext TestLinux = new RulesEvaluatorContext(new LauncherOSRule(LauncherOSRule.Linux, LauncherOSRule.X64));

    [Fact]
    public void extract_simple_java_library()
    {
        var library = parseLibrary(JsonLibraryParserTests.simple_java_library);
        var result = LibraryFileExtractor.Extractor.ExtractTasks(TestServer, TestPath, library, TestWindows);
        var expected = new GameFile("net.minecraft:launchwrapper:1.5")
        {
            Path = IOUtil.NormalizePath("libraries/net/minecraft/launchwrapper/1.5/launchwrapper-1.5.jar"),
            Hash = "5150b9c2951f0fde987ce9c33496e26add1de224",
            Size = 27787,
            Url = "https://libraries.minecraft.net/net/minecraft/launchwrapper/1.5/launchwrapper-1.5.jar"
        };
        Assert.Equal([expected], result);
    }

    [Fact]
    public void extract_native_library_on_linux()
    {
        var library = parseLibrary(JsonLibraryParserTests.native_library);
        var result = LibraryFileExtractor.Extractor.ExtractTasks(TestServer, TestPath, library, TestLinux);
        var expected = new GameFile("org.lwjgl.lwjgl:lwjgl-platform:2.9.0")
        {
            Path = IOUtil.NormalizePath("libraries/org/lwjgl/lwjgl/lwjgl-platform/2.9.0/lwjgl-platform-2.9.0-natives-linux.jar"),
            Hash = "2ba5dcb11048147f1a74eff2deb192c001321f77",
            Size = 569061,
            Url = "https://libraries.minecraft.net/org/lwjgl/lwjgl/lwjgl-platform/2.9.0/lwjgl-platform-2.9.0-natives-linux.jar"
        };
        Assert.Equal([expected], result);
    }

    [Fact]
    public void extract_native_library_on_windows()
    {
        var library = parseLibrary(JsonLibraryParserTests.native_library);
        var result = LibraryFileExtractor.Extractor.ExtractTasks(TestServer, TestPath, library, TestWindows);
        var expected = new GameFile("org.lwjgl.lwjgl:lwjgl-platform:2.9.0")
        {
            Path = IOUtil.NormalizePath("libraries/org/lwjgl/lwjgl/lwjgl-platform/2.9.0/lwjgl-platform-2.9.0-natives-windows.jar"),
            Hash = "3f11873dc8e84c854ec7c5a8fd2e869f8aaef764",
            Size = 609967,
            Url = "https://libraries.minecraft.net/org/lwjgl/lwjgl/lwjgl-platform/2.9.0/lwjgl-platform-2.9.0-natives-windows.jar"
        };
        Assert.Equal([expected], result);
    }

    [Fact]
    public void extract_java_library_and_native_library()
    {
        var library = parseLibrary(JsonLibraryParserTests.java_library_and_native_library);
        var result = LibraryFileExtractor.Extractor.ExtractTasks(TestServer, TestPath, library, TestWindows);
        var javaLibrary = new GameFile("org.lwjgl.lwjgl:lwjgl-platform:2.9.4-nightly-20150209")
        {
            Path = IOUtil.NormalizePath("libraries/org/lwjgl/lwjgl/lwjgl-platform/2.9.4-nightly-20150209/lwjgl-platform-2.9.4-nightly-20150209.jar"),
            Hash = "b04f3ee8f5e43fa3b162981b50bb72fe1acabb33",
            Size = 22,
            Url = "https://libraries.minecraft.net/org/lwjgl/lwjgl/lwjgl-platform/2.9.4-nightly-20150209/lwjgl-platform-2.9.4-nightly-20150209.jar"
        };
        var nativeLibrary = new GameFile("org.lwjgl.lwjgl:lwjgl-platform:2.9.4-nightly-20150209")
        {
            Path = IOUtil.NormalizePath("libraries/org/lwjgl/lwjgl/lwjgl-platform/2.9.4-nightly-20150209/lwjgl-platform-2.9.4-nightly-20150209-natives-windows.jar"),
            Hash = "b84d5102b9dbfabfeb5e43c7e2828d98a7fc80e0",
            Size = 613748,
            Url = "https://libraries.minecraft.net/org/lwjgl/lwjgl/lwjgl-platform/2.9.4-nightly-20150209/lwjgl-platform-2.9.4-nightly-20150209-natives-windows.jar"
        };
        Assert.Equal([javaLibrary, nativeLibrary], result.ToHashSet());
    }

    private static MLibrary parseLibrary(string json)
    {
        using var jsonDocument = JsonDocument.Parse(json);
        var result = JsonLibraryParser.Parse(jsonDocument.RootElement);
        if (result == null)
            throw new InvalidOperationException("null json");
        return result;
    }
}