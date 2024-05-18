using CmlLib.Core.ProcessBuilder;
using CmlLib.Core.Rules;
using CmlLib.Core.Version;

namespace CmlLib.Core.Test.Version;

public class JsonArgumentParserTests
{
    private static readonly JsonVersionParserOptions options = new JsonVersionParserOptions
    {
        Side = JsonVersionParserOptions.ClientSide,
        SkipError = true
    };

    private static string[] getArguments(IEnumerable<MArgument> args)
    {
        var dict = new Dictionary<string, string?>();
        return args
            .SelectMany(arg => arg.InterpolateValues(dict))
            .ToArray();
    }

    public static readonly string vanilla_arg_string = """
{
    "id": "1.7.10",
    "minecraftArguments": "--username ${auth_player_name} --version ${version_name} --gameDir ${game_directory} --assetsDir ${assets_root} --assetIndex ${assets_index_name} --uuid ${auth_uuid} --accessToken ${auth_access_token} --userProperties ${user_properties} --userType ${user_type}"
}
""";

    [Fact]
    public void parse_from_vanilla_arg_string()
    {
        // release 1.0 ~    : ${auth_player_name} ${auth_session} --gameDir ${game_directory} --assetsDir ${game_assets}
        // release 1.6.1 ~  : from --username to --assetDir
        // release 1.7.2 ~  : add --uuid, --accessToken
        // release 1.7.10 ~ : add --assetIndex, --userProperties, --userType

        var version = JsonVersionParser.ParseFromJsonString(vanilla_arg_string, options);
        var parsedArgs = getArguments(version.GetGameArguments(false));
        Assert.Equal(
        [
            "--username",
            "${auth_player_name}",
            "--version",
            "${version_name}",
            "--gameDir",
            "${game_directory}",
            "--assetsDir",
            "${assets_root}",
            "--assetIndex",
            "${assets_index_name}",
            "--uuid",
            "${auth_uuid}",
            "--accessToken",
            "${auth_access_token}",
            "--userProperties",
            "${user_properties}",
            "--userType",
            "${user_type}"
        ], parsedArgs);
    }

    public static readonly string vanilla_game_arg_array = """
{
    "id": "1.13",
    "arguments": {
        "game": [
            "--username",
            "${auth_player_name}",
            "--version",
            "${version_name}",
            "--gameDir",
            "${game_directory}",
            "--assetsDir",
            "${assets_root}",
            "--assetIndex",
            "${assets_index_name}",
            "--uuid",
            "${auth_uuid}",
            "--accessToken",
            "${auth_access_token}",
            "--clientId",
            "${clientid}",
            "--xuid",
            "${auth_xuid}",
            "--userType",
            "${user_type}",
            "--versionType",
            "${version_type}",
            {
                "rules": [
                    {
                        "action": "allow",
                        "features": {
                            "is_demo_user": true
                        }
                    }
                ],
                "value": "--demo"
            },
            {
                "rules": [
                    {
                        "action": "allow",
                        "features": {
                            "has_custom_resolution": true
                        }
                    }
                ],
                "value": [
                    "--width",
                    "${resolution_width}",
                    "--height",
                    "${resolution_height}"
                ]
            },
            {
                "rules": [
                    {
                        "action": "allow",
                        "features": {
                            "has_quick_plays_support": true
                        }
                    }
                ],
                "value": [
                    "--quickPlayPath",
                    "${quickPlayPath}"
                ]
            },
            {
                "rules": [
                    {
                        "action": "allow",
                        "features": {
                            "is_quick_play_singleplayer": true
                        }
                    }
                ],
                "value": [
                    "--quickPlaySingleplayer",
                    "${quickPlaySingleplayer}"
                ]
            },
            {
                "rules": [
                    {
                        "action": "allow",
                        "features": {
                            "is_quick_play_multiplayer": true
                        }
                    }
                ],
                "value": [
                    "--quickPlayMultiplayer",
                    "${quickPlayMultiplayer}"
                ]
            },
            {
                "rules": [
                    {
                        "action": "allow",
                        "features": {
                            "is_quick_play_realms": true
                        }
                    }
                ],
                "value": [
                    "--quickPlayRealms",
                    "${quickPlayRealms}"
                ]
            }
        ]
    }
}
""";

    [Fact]
    public void parse_from_vanilla_game_arg_array() // 1.13 ~
    {
        // release 1.13 ~
        // release 1.19 ~ : add --clientId ${clientid} --xuid ${auth_xuid}
        // release 1.20 ~ : add --quickPlayPath, --quickPlaySingleplayer, --quickPlayMultiplayer, --quickPlayRealms

        var version = JsonVersionParser.ParseFromJsonString(vanilla_game_arg_array, options);
        var parsedArgs = getArguments(version.GetGameArguments(false));
        Assert.Equal(
        [
            "--username",
            "${auth_player_name}",
            "--version",
            "${version_name}",
            "--gameDir",
            "${game_directory}",
            "--assetsDir",
            "${assets_root}",
            "--assetIndex",
            "${assets_index_name}",
            "--uuid",
            "${auth_uuid}",
            "--accessToken",
            "${auth_access_token}",
            "--clientId",
            "${clientid}",
            "--xuid",
            "${auth_xuid}",
            "--userType",
            "${user_type}",
            "--versionType",
            "${version_type}",
            "--demo",
            "--width",
            "${resolution_width}",
            "--height",
            "${resolution_height}",
            "--quickPlayPath",
            "${quickPlayPath}",
            "--quickPlaySingleplayer",
            "${quickPlaySingleplayer}",
            "--quickPlayMultiplayer",
            "${quickPlayMultiplayer}",
            "--quickPlayRealms",
            "${quickPlayRealms}"
        ], parsedArgs);
    }

    public static readonly string vanilla_jvm_arg_array = """
{
    "id": "1.13",
    "arguments": {
        "jvm": [
            {
                "rules": [
                    {
                        "action": "allow",
                        "os": {
                            "name": "osx"
                        }
                    }
                ],
                "value": [
                    "-XstartOnFirstThread"
                ]
            },
            {
                "rules": [
                    {
                        "action": "allow",
                        "os": {
                            "name": "windows"
                        }
                    }
                ],
                "value": "-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump"
            },
            {
                "rules": [
                    {
                        "action": "allow",
                        "os": {
                            "name": "windows",
                            "version": "^10\\."
                        }
                    }
                ],
                "value": [
                    "-Dos.name=Windows 10",
                    "-Dos.version=10.0"
                ]
            },
            {
                "rules": [
                    {
                        "action": "allow",
                        "os": {
                            "arch": "x86"
                        }
                    }
                ],
                "value": "-Xss1M"
            },
            "-Djava.library.path=${natives_directory}",
            "-Djna.tmpdir=${natives_directory}",
            "-Dorg.lwjgl.system.SharedLibraryExtractPath=${natives_directory}",
            "-Dio.netty.native.workdir=${natives_directory}",
            "-Dminecraft.launcher.brand=${launcher_name}",
            "-Dminecraft.launcher.version=${launcher_version}",
            "-cp",
            "${classpath}"
        ]
    }
}
""";

    [Fact]
    public void parse_from_vanilla_jvm_arg_array()
    {
        // release 1.13 ~
        // release 1.20 ~ : add -Djna.tmpdir=, -Dorg.lwjgl.system.SharedLibraryExtractPath, -Dio.netty.native.workdir

        var version = JsonVersionParser.ParseFromJsonString(vanilla_jvm_arg_array, options);

        var parsedArgs = getArguments(version.GetJvmArguments(false));
        Assert.Equal(
        [
            "-XstartOnFirstThread",
            "-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump",
            "-Dos.name=Windows 10",
            "-Dos.version=10.0",
            "-Xss1M",
            "-Djava.library.path=${natives_directory}",
            "-Djna.tmpdir=${natives_directory}",
            "-Dorg.lwjgl.system.SharedLibraryExtractPath=${natives_directory}",
            "-Dio.netty.native.workdir=${natives_directory}",
            "-Dminecraft.launcher.brand=${launcher_name}",
            "-Dminecraft.launcher.version=${launcher_version}",
            "-cp",
            "${classpath}",
        ], parsedArgs);
    }

    public readonly static string forge_arguments = """
{
    "id": "1.16.5-forge-36.2.0",
    "arguments": {
        "game": [
            "--launchTarget",
            "fmlclient",
            "--fml.forgeVersion",
            "36.2.0",
            "--fml.mcVersion",
            "1.16.5",
            "--fml.forgeGroup",
            "net.minecraftforge",
            "--fml.mcpVersion",
            "20210115.111550"
        ],
        "jvm": [
            "-XX:+IgnoreUnrecognizedVMOptions",
            "--add-exports=java.base/sun.security.util=ALL-UNNAMED",
            "--add-exports=jdk.naming.dns/com.sun.jndi.dns=java.naming",
            "--add-opens=java.base/java.util.jar=ALL-UNNAMED"
        ]
    }
}
""";

    [Fact]
    public void parse_forge_arguments()
    {
        var version = JsonVersionParser.ParseFromJsonString(forge_arguments, options);
        Assert.Equal(
        [
            "--launchTarget",
            "fmlclient",
            "--fml.forgeVersion",
            "36.2.0",
            "--fml.mcVersion",
            "1.16.5",
            "--fml.forgeGroup",
            "net.minecraftforge",
            "--fml.mcpVersion",
            "20210115.111550"
        ], getArguments(version.GetGameArguments(false)));
        Assert.Equal(
        [
            "-XX:+IgnoreUnrecognizedVMOptions",
            "--add-exports=java.base/sun.security.util=ALL-UNNAMED",
            "--add-exports=jdk.naming.dns/com.sun.jndi.dns=java.naming",
            "--add-opens=java.base/java.util.jar=ALL-UNNAMED"
        ], getArguments(version.GetJvmArguments(false)));
    }

    public readonly static string forge_arguments_2 = """
{
    "id": "1.17.1-forge-37.0.48",
    "arguments": {
        "game": [
            "--launchTarget",
            "forgeclient",
            "--fml.forgeVersion",
            "37.0.48",
            "--fml.mcVersion",
            "1.17.1",
            "--fml.forgeGroup",
            "net.minecraftforge",
            "--fml.mcpVersion",
            "20210706.113038"
        ],
        "jvm": [
            "-DignoreList=bootstraplauncher,securejarhandler,asm-commons,asm-util,asm-analysis,asm-tree,asm,client-extra,fmlcore,javafmllanguage,mclanguage,forge-,${version_name}.jar",
            "-DmergeModules=jna-5.8.0.jar,jna-platform-58.0.jar,java-objc-bridge-1.0.0.jar",
            "-DlibraryDirectory=${library_directory}",
            "-p",
            "${library_directory}/cpw/mods/bootstraplauncher/0.1.17/bootstraplauncher-0.1.17.jar${classpath_separator}${library_directory}/cpw/mods/securejarhandler/0.9.46/securejarhandler-0.9.46.jar${classpath_separator}${library_directory}/org/ow2/asm/asm-commons/9.1/asm-commons-9.1.jar${classpath_separator}${library_directory}/org/ow2/asm/asm-util/9.1/asm-util-9.1.jar${classpath_separator}${library_directory}/org/ow2/asm/asm-analysis/9.1/asm-analysis-9.1.jar${classpath_separator}${library_directory}/org/ow2/asm/asm-tree/9.1/asm-tree-9.1.jar${classpath_separator}${library_directory}/org/ow2/asm/asm/9.1/asm-9.1.jar",
            "--add-modules",
            "ALL-MODULE-PATH",
            "--add-opens",
            "java.base/java.util.jar=cpw.mods.securejarhandler",
            "--add-exports",
            "java.base/sun.security.util=cpw.mods.securejarhandler",
            "--add-exports",
            "jdk.naming.dns/com.sun.jndi.dns=java.naming"
        ]
    }
}
""";

    [Fact]
    public void parse_forge_arguments_2()
    {
        var version = JsonVersionParser.ParseFromJsonString(forge_arguments_2, options);
        Assert.Equal(
        [
            "--launchTarget",
            "forgeclient",
            "--fml.forgeVersion",
            "37.0.48",
            "--fml.mcVersion",
            "1.17.1",
            "--fml.forgeGroup",
            "net.minecraftforge",
            "--fml.mcpVersion",
            "20210706.113038"
        ], getArguments(version.GetGameArguments(false)));
        Assert.Equal(
        [
            "-DignoreList=bootstraplauncher,securejarhandler,asm-commons,asm-util,asm-analysis,asm-tree,asm,client-extra,fmlcore,javafmllanguage,mclanguage,forge-,${version_name}.jar",
            "-DmergeModules=jna-5.8.0.jar,jna-platform-58.0.jar,java-objc-bridge-1.0.0.jar",
            "-DlibraryDirectory=${library_directory}",
            "-p",
            "${library_directory}/cpw/mods/bootstraplauncher/0.1.17/bootstraplauncher-0.1.17.jar${classpath_separator}${library_directory}/cpw/mods/securejarhandler/0.9.46/securejarhandler-0.9.46.jar${classpath_separator}${library_directory}/org/ow2/asm/asm-commons/9.1/asm-commons-9.1.jar${classpath_separator}${library_directory}/org/ow2/asm/asm-util/9.1/asm-util-9.1.jar${classpath_separator}${library_directory}/org/ow2/asm/asm-analysis/9.1/asm-analysis-9.1.jar${classpath_separator}${library_directory}/org/ow2/asm/asm-tree/9.1/asm-tree-9.1.jar${classpath_separator}${library_directory}/org/ow2/asm/asm/9.1/asm-9.1.jar",
            "--add-modules",
            "ALL-MODULE-PATH",
            "--add-opens",
            "java.base/java.util.jar=cpw.mods.securejarhandler",
            "--add-exports",
            "java.base/sun.security.util=cpw.mods.securejarhandler",
            "--add-exports",
            "jdk.naming.dns/com.sun.jndi.dns=java.naming"
        ], getArguments(version.GetJvmArguments(false)));
    }

    public readonly static string fabric_loader_arguments = """
{
    "id": "fabric-loader-0.13.3-1.18.2",
    "arguments": {
        "game": [],
        "jvm": [
            "-DFabricMcEmu= net.minecraft.client.main.Main "
        ]
    }
}
""";

    [Fact]
    public void parse_fabric_loader_arguments()
    {
        var version = JsonVersionParser.ParseFromJsonString(fabric_loader_arguments, options);
        Assert.Empty(getArguments(version.GetGameArguments(false)));
        Assert.Equal(["-DFabricMcEmu= net.minecraft.client.main.Main "], getArguments(version.GetJvmArguments(false)));
    }
}