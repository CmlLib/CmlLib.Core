using System.Text.Json;
using CmlLib.Core.Files;
using CmlLib.Core.Rules;
using CmlLib.Core.Version;

namespace CmlLib.Core.Test.Version;

public class JsonLibraryParserTests
{
    // release 1.0
    public static readonly string simple_java_library = """
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

    [Fact]
    public void parse_simple_java_library()
    {
        using var jsonDocument = JsonDocument.Parse(simple_java_library);
        var result = JsonLibraryParser.Parse(jsonDocument.RootElement);
        var expected = new MLibrary("net.minecraft:launchwrapper:1.5")
        {
            Artifact = new MFileMetadata
            {
                Path = "net/minecraft/launchwrapper/1.5/launchwrapper-1.5.jar",
                Sha1 = "5150b9c2951f0fde987ce9c33496e26add1de224",
                Size = 27787,
                Url = "https://libraries.minecraft.net/net/minecraft/launchwrapper/1.5/launchwrapper-1.5.jar",
            }
        };
        Assert.Equal(expected, result);
    }

    // release 1.0
    public static readonly string java_library_with_rules = """
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

    [Fact]
    public void parse_java_library_with_rules()
    {
        using var jsonDocument = JsonDocument.Parse(java_library_with_rules);
        var lib = JsonLibraryParser.Parse(jsonDocument.RootElement);
        Assert.Equal(2, lib?.Rules?.Count);
    }

// release 1.2.5
    public readonly static string native_library = """
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

    [Fact]
    public void parse_native_library()
    {
        using var jsonDocument = JsonDocument.Parse(native_library);
        var lib = JsonLibraryParser.Parse(jsonDocument.RootElement);
        var nativeLib = lib?.GetNativeLibrary(new LauncherOSRule(LauncherOSRule.Windows, LauncherOSRule.X64, ""));
        var expected = new MFileMetadata
        {
            Path = "org/lwjgl/lwjgl/lwjgl-platform/2.9.0/lwjgl-platform-2.9.0-natives-windows.jar",
            Sha1 = "3f11873dc8e84c854ec7c5a8fd2e869f8aaef764",
            Size = 609967,
            Url = "https://libraries.minecraft.net/org/lwjgl/lwjgl/lwjgl-platform/2.9.0/lwjgl-platform-2.9.0-natives-windows.jar"
        };
        Assert.Null(lib?.Artifact);
        Assert.Equal(["META-INF/"], lib?.ExtractExcludes.ToHashSet());
        Assert.Equal(expected, nativeLib);
    }

    // release 1.7.10
    public readonly static string native_library_with_arch = """
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

    [Fact]
    public void parse_native_library_with_arch()
    {
        using var jsonDocument = JsonDocument.Parse(native_library_with_arch);
        var lib = JsonLibraryParser.Parse(jsonDocument.RootElement);
        Assert.Null(lib?.Artifact);

        var nativeLib = lib?.GetNativeLibrary(new LauncherOSRule(LauncherOSRule.Windows, LauncherOSRule.X86, ""));
        Assert.Equal("tv/twitch/twitch-platform/5.16/twitch-platform-5.16-natives-windows-32.jar", nativeLib?.Path);
    }

    // release 1.8.9
    public readonly static string java_library_and_native_library = """
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

    [Fact]
    public void parse_java_library_and_native_library()
    {
        using var jsonDocument = JsonDocument.Parse(java_library_and_native_library);
        var lib = JsonLibraryParser.Parse(jsonDocument.RootElement);

        var nativeLib = lib?.GetNativeLibrary(new LauncherOSRule("windows", "32", ""));
        Assert.Equal("org/lwjgl/lwjgl/lwjgl-platform/2.9.4-nightly-20150209/lwjgl-platform-2.9.4-nightly-20150209-natives-windows.jar", nativeLib?.Path);

        var javaLib = lib?.Artifact;
        Assert.Equal("org/lwjgl/lwjgl/lwjgl-platform/2.9.4-nightly-20150209/lwjgl-platform-2.9.4-nightly-20150209.jar", javaLib?.Path);
    }

    // 1.7.10-Forge10.13.4.1558-1.7.10
    public static readonly string java_library_with_url = """
{
    "name": "net.minecraftforge:forge:1.7.10-10.13.4.1558-1.7.10",
	"url": "http://files.minecraftforge.net/maven/"
}
""";

    [Fact]
    public void parse_java_library_with_url()
    {
        using var jsonDocument = JsonDocument.Parse(java_library_with_url);
        var lib = JsonLibraryParser.Parse(jsonDocument.RootElement);
        Assert.Null(lib?.Classifiers);
        Assert.Equal("net.minecraftforge:forge:1.7.10-10.13.4.1558-1.7.10", lib?.Name);
        Assert.Equal("http://files.minecraftforge.net/maven/", lib?.Artifact?.Url);
        Assert.Null(lib?.Artifact?.Path);
        Assert.Null(lib?.Artifact?.GetSha1());
    }

    // 1.7.10-Forge10.13.4.1558-1.7.10
    public static readonly string java_library_with_url_checksums = """
{
    "name": "com.typesafe.akka:akka-actor_2.11:2.3.3",
    "url": "http://files.minecraftforge.net/maven/",
    "checksums": [
        "ed62e9fc709ca0f2ff1a3220daa8b70a2870078e",
        "25a86ccfdb6f6dfe08971f4825d0a01be83a6f2e"
    ],
    "serverreq": true,
    "clientreq": true
}
""";

    public static readonly string java_library_with_url_checksums_2 = """
{
	"name": "org.ow2.asm:asm-all:5.2",
	"url": "https://maven.minecraftforge.net/",
	"checksums": [
	    "2ea49e08b876bbd33e0a7ce75c8f371d29e1f10a"
	],
	"serverreq": true,
	"clientreq": true
}
""";

    [Fact]
    public void parse_java_library_with_url_checksums()
    {
        using var jsonDocument = JsonDocument.Parse(java_library_with_url_checksums);
        var lib = JsonLibraryParser.Parse(jsonDocument.RootElement);
        Assert.Equal("com.typesafe.akka:akka-actor_2.11:2.3.3", lib?.Name);
        Assert.Equal("http://files.minecraftforge.net/maven/", lib?.Artifact?.Url);
        Assert.Equal(["ed62e9fc709ca0f2ff1a3220daa8b70a2870078e", "25a86ccfdb6f6dfe08971f4825d0a01be83a6f2e"], lib?.Artifact?.Checksums);
        Assert.Null(lib?.Artifact?.Path);
        Assert.Null(lib?.Classifiers);
    }

    // 1.8.9-forge1.8.9-11.15.1.1722
    public static readonly string java_library_with_only_name = """
{
	"name": "java3d:vecmath:1.5.2",
	"clientreq": true,
    "serverreq": true
}
""";

    // 1.8.9-OptiFine_HD_U_M5
    public static readonly string java_library_with_only_name_2 = """
{
    "name": "optifine:OptiFine:1.8.9_HD_U_M5"
}
""";

    [Fact]
    public void parse_java_library_with_only_name()
    {
        using var jsonDocument = JsonDocument.Parse(java_library_with_only_name);
        var lib = JsonLibraryParser.Parse(jsonDocument.RootElement);
        Assert.Equal("java3d:vecmath:1.5.2", lib?.Name);
        Assert.Null(lib?.Artifact?.Url);
        Assert.Null(lib?.Artifact?.Path);
        Assert.Null(lib?.Classifiers);
    }

    // 1.12.2-forge-14.23.5.2854
    public static readonly string java_library_with_empty_url = """
{
    "name": "net.minecraftforge:forge:1.12.2-14.23.5.2854",
    "downloads": {
        "artifact": {
            "path": "net/minecraftforge/forge/1.12.2-14.23.5.2854/forge-1.12.2-14.23.5.2854.jar",
            "url": "",
            "sha1": "762f5cb227a8dd88cfd3949033c78f24030b36aa",
            "size": 4464068
        }
    }
}
""";

    [Fact]
    public void parse_java_library_with_empty_url()
    {
        using var jsonDocument = JsonDocument.Parse(java_library_with_empty_url);
        var lib = JsonLibraryParser.Parse(jsonDocument.RootElement);
        var expected = new MLibrary("net.minecraftforge:forge:1.12.2-14.23.5.2854")
        {
            Artifact = new MFileMetadata
            {
                Path = "net/minecraftforge/forge/1.12.2-14.23.5.2854/forge-1.12.2-14.23.5.2854.jar",
                Url = "",
                Sha1 = "762f5cb227a8dd88cfd3949033c78f24030b36aa",
                Size = 4464068
            }
        };
        Assert.Equal(expected, lib);
    }

    // OptiFine 1.7.10 from third-party launcher
    public static readonly string native_library_with_incorrect_classifier_id = """
{
  "name": "org.lwjgl.lwjgl:lwjgl-platform:2.9.1",
  "natives": {
    "linux": "natives-linux",
    "osx": "natives-osx",
    "windows": "natives-windows"
  },
  "extract": {
    "exclude": [
      "META-INF/"
    ]
  },
  "classifies": {
    "linux": {
      "sha1": "aa9aae879af8eb378e22cfc64db56ec2ca9a44d1",
      "size": 571424,
      "path": "org/lwjgl/lwjgl/lwjgl-platform/2.9.1/lwjgl-platform-2.9.1-natives-linux.jar",
      "url": "https://libraries.minecraft.net/org/lwjgl/lwjgl/lwjgl-platform/2.9.1/lwjgl-platform-2.9.1-natives-linux.jar"
    },
    "osx": {
      "sha1": "2d12c83fdfbc04ecabf02c7bc8cc54d034f0daac",
      "size": 527196,
      "path": "org/lwjgl/lwjgl/lwjgl-platform/2.9.1/lwjgl-platform-2.9.1-natives-osx.jar",
      "url": "https://libraries.minecraft.net/org/lwjgl/lwjgl/lwjgl-platform/2.9.1/lwjgl-platform-2.9.1-natives-osx.jar"
    },
    "windows": {
      "sha1": "4c517eca808522457dd95ee8fc1fbcdbb602efbe",
      "size": 611334,
      "path": "org/lwjgl/lwjgl/lwjgl-platform/2.9.1/lwjgl-platform-2.9.1-natives-windows.jar",
      "url": "https://libraries.minecraft.net/org/lwjgl/lwjgl/lwjgl-platform/2.9.1/lwjgl-platform-2.9.1-natives-windows.jar"
    }
  }
}
""";

    [Fact]
    public void get_native_library_with_incorrect_classifier_id()
    {
        using var jsonDocument = JsonDocument.Parse(native_library_with_incorrect_classifier_id);
        var lib = JsonLibraryParser.Parse(jsonDocument.RootElement);

        var windows = lib?.GetNativeLibrary(new LauncherOSRule(LauncherOSRule.Windows, LauncherOSRule.X64, ""));
        Assert.Equal("4c517eca808522457dd95ee8fc1fbcdbb602efbe", windows?.GetSha1());

        var linux = lib?.GetNativeLibrary(new LauncherOSRule(LauncherOSRule.Linux, LauncherOSRule.X64, ""));
        Assert.Equal("aa9aae879af8eb378e22cfc64db56ec2ca9a44d1", linux?.GetSha1());

        var osx = lib?.GetNativeLibrary(new LauncherOSRule(LauncherOSRule.OSX, LauncherOSRule.X64, ""));
        Assert.Equal("2d12c83fdfbc04ecabf02c7bc8cc54d034f0daac", osx?.GetSha1());
    }

    // Butteracking Client v4
    public static readonly string native_library_without_classifiers = """
{
  "name": "tv.twitch:twitch-external-platform:4.5",
  "rules": [
    {
      "action": "allow",
      "os": {
        "name": "windows"
      }
    }
  ],
  "natives": {
    "windows": "natives-windows-${arch}"
  },
  "extract": {
    "exclude": [
      "META-INF/"
    ]
  }
}
""";

    [Fact]
    public void get_native_library_without_classifiers()
    {
        using var jsonDocument = JsonDocument.Parse(native_library_without_classifiers);
        var lib = JsonLibraryParser.Parse(jsonDocument.RootElement);

        var win = lib?.GetNativeLibrary(new LauncherOSRule(LauncherOSRule.Windows, LauncherOSRule.X64, "10"));
        Assert.Null(win);

        var winPath = lib?.GetNativeLibraryPath(new LauncherOSRule(LauncherOSRule.Windows, LauncherOSRule.X64, "10"));
        Assert.NotNull(winPath);
    }


    // 1.7.2-Forge10.12.2.1161-mc172
    public static readonly string native_library_for_osx = """
{
    "name": "org.lwjgl.lwjgl:lwjgl-platform:2.9.1-nightly-20131017",
	"rules": [
	    {
		    "action": "allow",
			"os": {
			    "name": "osx"
			}
		}
	],
	"natives": {
		"osx": "natives-osx"
	},
	"extract": {
		"exclude": [
			"META-INF/"
		]
	}
}
""";

    [Fact]
    public void get_native_library_on_unsupported_os()
    {
        using var jsonDocument = JsonDocument.Parse(native_library_without_classifiers);
        var lib = JsonLibraryParser.Parse(jsonDocument.RootElement);

        var win = lib?.GetNativeLibrary(new LauncherOSRule(LauncherOSRule.Windows, LauncherOSRule.X64, "10"));
        Assert.Null(win); // return null instead of throwing an exception

        var winPath = lib?.GetNativeLibraryPath(new LauncherOSRule(LauncherOSRule.Windows, LauncherOSRule.X64, "10"));
        Assert.NotNull(winPath);
    }
}