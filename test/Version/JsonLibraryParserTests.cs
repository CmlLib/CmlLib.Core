using System.Text.Json;
using CmlLib.Core.Version;

namespace CmlLib.Core.Test.Version;

public class JsonLibraryParserTests
{
    [Fact]
    public void parse_simple_java_library()
    {
        // release 1.0
        var json = """
{
    "downloads": {
        "artifact": {
            "path": "net/minecraft/launchwrapper/1.5/launchwrapper-1.5.jar",
            "sha1": "5150b9c2951f0fde987ce9c33496e26add1de224",
            "size": 27787,
            "url": "https://libraries.minecraft.net/net/minecraft/launchwrapper/1.5/launchwrapper-1.5.jar"
        }
    },
    "name": "net.minecraft:launchwrapper:1.5"
}
""";

        using var jsonDocument = JsonDocument.Parse(json);
        var lib = JsonLibraryParser.Parse(jsonDocument.RootElement);

        Assert.Equal("net.minecraft:launchwrapper:1.5", lib?.Name);
        Assert.Equal("net/minecraft/launchwrapper/1.5/launchwrapper-1.5.jar", lib?.Artifact?.Path);
        Assert.Equal("5150b9c2951f0fde987ce9c33496e26add1de224", lib?.Artifact?.Sha1);
        Assert.Equal(27787, lib?.Artifact?.Size);
        Assert.Equal("https://libraries.minecraft.net/net/minecraft/launchwrapper/1.5/launchwrapper-1.5.jar", lib?.Artifact?.Url);
    }

    [Fact]
    public void parse_java_library_with_rules()
    {
        // release 1.0
        var json = """
        {
            "downloads": {
                "artifact": {
                    "path": "org/lwjgl/lwjgl/lwjgl/2.9.0/lwjgl-2.9.0.jar",
                    "sha1": "5654d06e61a1bba7ae1e7f5233e1106be64c91cd",
                    "size": 994633,
                    "url": "https://libraries.minecraft.net/org/lwjgl/lwjgl/lwjgl/2.9.0/lwjgl-2.9.0.jar"
                }
            },
            "name": "org.lwjgl.lwjgl:lwjgl:2.9.0",
            "rules": [
                {
                    "action": "allow"
                },
                {
                    "action": "disallow",
                    "os": {
                        "name": "osx",
                        "version": "^10\\.5\\.\\d$"
                    }
                }
            ]
        }
        """;

        using var jsonDocument = JsonDocument.Parse(json);
        var lib = JsonLibraryParser.Parse(jsonDocument.RootElement);

        Assert.Equal(2, lib?.Rules?.Count);
    }

    [Fact]
    public void parse_native_library()
    {
        // release 1.2.5
        var json = """
        {
            "downloads": {
                "classifiers": {
                    "natives-linux": {
                        "path": "org/lwjgl/lwjgl/lwjgl-platform/2.9.0/lwjgl-platform-2.9.0-natives-linux.jar",
                        "sha1": "2ba5dcb11048147f1a74eff2deb192c001321f77",
                        "size": 569061,
                        "url": "https://libraries.minecraft.net/org/lwjgl/lwjgl/lwjgl-platform/2.9.0/lwjgl-platform-2.9.0-natives-linux.jar"
                    },
                    "natives-osx": {
                        "path": "org/lwjgl/lwjgl/lwjgl-platform/2.9.0/lwjgl-platform-2.9.0-natives-osx.jar",
                        "sha1": "6621b382cb14cc409b041d8d72829156a87c31aa",
                        "size": 518924,
                        "url": "https://libraries.minecraft.net/org/lwjgl/lwjgl/lwjgl-platform/2.9.0/lwjgl-platform-2.9.0-natives-osx.jar"
                    },
                    "natives-windows": {
                        "path": "org/lwjgl/lwjgl/lwjgl-platform/2.9.0/lwjgl-platform-2.9.0-natives-windows.jar",
                        "sha1": "3f11873dc8e84c854ec7c5a8fd2e869f8aaef764",
                        "size": 609967,
                        "url": "https://libraries.minecraft.net/org/lwjgl/lwjgl/lwjgl-platform/2.9.0/lwjgl-platform-2.9.0-natives-windows.jar"
                    }
                }
            },
            "extract": {
                "exclude": [
                    "META-INF/"
                ]
            },
            "name": "org.lwjgl.lwjgl:lwjgl-platform:2.9.0",
            "natives": {
                "linux": "natives-linux",
                "osx": "natives-osx",
                "windows": "natives-windows"
            },
            "rules": [
                {
                    "action": "allow"
                },
                {
                    "action": "disallow",
                    "os": {
                        "name": "osx",
                        "version": "^10\\.5\\.\\d$"
                    }
                }
            ]
        }
        """;

        using var jsonDocument = JsonDocument.Parse(json);
        var lib = JsonLibraryParser.Parse(jsonDocument.RootElement);
        var nativeLib = lib?.GetNativeLibrary(new Core.Rules.LauncherOSRule("windows", "64"));

        Assert.Equal("org/lwjgl/lwjgl/lwjgl-platform/2.9.0/lwjgl-platform-2.9.0-natives-windows.jar", nativeLib?.Path);
        Assert.Equal("3f11873dc8e84c854ec7c5a8fd2e869f8aaef764", nativeLib?.Sha1);
        Assert.Equal(609967, nativeLib?.Size);
        Assert.Equal("https://libraries.minecraft.net/org/lwjgl/lwjgl/lwjgl-platform/2.9.0/lwjgl-platform-2.9.0-natives-windows.jar", nativeLib?.Url);
    }

    [Fact]
    public void parse_native_library_with_arch()
    {
        // release 1.7.10
        var json = """
        {
            "downloads": {
                "classifiers": {
                    "natives-osx": {
                        "path": "tv/twitch/twitch-platform/5.16/twitch-platform-5.16-natives-osx.jar",
                        "sha1": "62503ee712766cf77f97252e5902786fd834b8c5",
                        "size": 418331,
                        "url": "https://libraries.minecraft.net/tv/twitch/twitch-platform/5.16/twitch-platform-5.16-natives-osx.jar"
                    },
                    "natives-windows-32": {
                        "path": "tv/twitch/twitch-platform/5.16/twitch-platform-5.16-natives-windows-32.jar",
                        "sha1": "7c6affe439099806a4f552da14c42f9d643d8b23",
                        "size": 386792,
                        "url": "https://libraries.minecraft.net/tv/twitch/twitch-platform/5.16/twitch-platform-5.16-natives-windows-32.jar"
                    },
                    "natives-windows-64": {
                        "path": "tv/twitch/twitch-platform/5.16/twitch-platform-5.16-natives-windows-64.jar",
                        "sha1": "39d0c3d363735b4785598e0e7fbf8297c706a9f9",
                        "size": 463390,
                        "url": "https://libraries.minecraft.net/tv/twitch/twitch-platform/5.16/twitch-platform-5.16-natives-windows-64.jar"
                    }
                }
            },
            "extract": {
                "exclude": [
                    "META-INF/"
                ]
            },
            "name": "tv.twitch:twitch-platform:5.16",
            "natives": {
                "linux": "natives-linux",
                "osx": "natives-osx",
                "windows": "natives-windows-${arch}"
            },
            "rules": [
                {
                    "action": "allow"
                },
                {
                    "action": "disallow",
                    "os": {
                        "name": "linux"
                    }
                }
            ]
        }
        """;

        using var jsonDocument = JsonDocument.Parse(json);
        var lib = JsonLibraryParser.Parse(jsonDocument.RootElement);

        var nativeLib = lib?.GetNativeLibrary(new Core.Rules.LauncherOSRule("windows", "32"));
        Assert.Equal("tv/twitch/twitch-platform/5.16/twitch-platform-5.16-natives-windows-32.jar", nativeLib?.Path);
    }

    [Fact]
    public void parse_java_library_and_native_library()
    {
        // release 1.8.9
        var json = """
        {
            "downloads": {
                "artifact": {
                    "path": "org/lwjgl/lwjgl/lwjgl-platform/2.9.4-nightly-20150209/lwjgl-platform-2.9.4-nightly-20150209.jar",
                    "sha1": "b04f3ee8f5e43fa3b162981b50bb72fe1acabb33",
                    "size": 22,
                    "url": "https://libraries.minecraft.net/org/lwjgl/lwjgl/lwjgl-platform/2.9.4-nightly-20150209/lwjgl-platform-2.9.4-nightly-20150209.jar"
                },
                "classifiers": {
                    "natives-linux": {
                        "path": "org/lwjgl/lwjgl/lwjgl-platform/2.9.4-nightly-20150209/lwjgl-platform-2.9.4-nightly-20150209-natives-linux.jar",
                        "sha1": "931074f46c795d2f7b30ed6395df5715cfd7675b",
                        "size": 578680,
                        "url": "https://libraries.minecraft.net/org/lwjgl/lwjgl/lwjgl-platform/2.9.4-nightly-20150209/lwjgl-platform-2.9.4-nightly-20150209-natives-linux.jar"
                    },
                    "natives-osx": {
                        "path": "org/lwjgl/lwjgl/lwjgl-platform/2.9.4-nightly-20150209/lwjgl-platform-2.9.4-nightly-20150209-natives-osx.jar",
                        "sha1": "bcab850f8f487c3f4c4dbabde778bb82bd1a40ed",
                        "size": 426822,
                        "url": "https://libraries.minecraft.net/org/lwjgl/lwjgl/lwjgl-platform/2.9.4-nightly-20150209/lwjgl-platform-2.9.4-nightly-20150209-natives-osx.jar"
                    },
                    "natives-windows": {
                        "path": "org/lwjgl/lwjgl/lwjgl-platform/2.9.4-nightly-20150209/lwjgl-platform-2.9.4-nightly-20150209-natives-windows.jar",
                        "sha1": "b84d5102b9dbfabfeb5e43c7e2828d98a7fc80e0",
                        "size": 613748,
                        "url": "https://libraries.minecraft.net/org/lwjgl/lwjgl/lwjgl-platform/2.9.4-nightly-20150209/lwjgl-platform-2.9.4-nightly-20150209-natives-windows.jar"
                    }
                }
            },
            "extract": {
                "exclude": [
                    "META-INF/"
                ]
            },
            "name": "org.lwjgl.lwjgl:lwjgl-platform:2.9.4-nightly-20150209",
            "natives": {
                "linux": "natives-linux",
                "osx": "natives-osx",
                "windows": "natives-windows"
            },
            "rules": [
                {
                    "action": "allow"
                },
                {
                    "action": "disallow",
                    "os": {
                        "name": "osx"
                    }
                }
            ]
        }
        """;

        using var jsonDocument = JsonDocument.Parse(json);
        var lib = JsonLibraryParser.Parse(jsonDocument.RootElement);

        var nativeLib = lib?.GetNativeLibrary(new Core.Rules.LauncherOSRule("windows", "32"));
        Assert.Equal("org/lwjgl/lwjgl/lwjgl-platform/2.9.4-nightly-20150209/lwjgl-platform-2.9.4-nightly-20150209-natives-windows.jar", nativeLib?.Path);

        var javaLib = lib?.Artifact;
        Assert.Equal("org/lwjgl/lwjgl/lwjgl-platform/2.9.4-nightly-20150209/lwjgl-platform-2.9.4-nightly-20150209.jar", javaLib?.Path);
    }
}