using CmlLib.Core.Version;

namespace CmlLib.Core.Test.Version;

public class JsonArgumentParserTests
{
    private static readonly JsonVersionParserOptions options = new JsonVersionParserOptions
    {
        Side = JsonVersionParserOptions.ClientSide,
        SkipError = false
    };

    [Fact]
    public void parse_from_vanilla_arg_string()
    {
        // release 1.0 ~    : ${auth_player_name} ${auth_session} --gameDir ${game_directory} --assetsDir ${game_assets}
        // release 1.6.1 ~  : from --username to --assetDir
        // release 1.7.2 ~  : add --uuid, --accessToken
        // release 1.7.10 ~ : add --assetIndex, --userProperties, --userType

        var json = """
{
    "id": "1.7.10",
    "minecraftArguments": "--username ${auth_player_name} --version ${version_name} --gameDir ${game_directory} --assetsDir ${assets_root} --assetIndex ${assets_index_name} --uuid ${auth_uuid} --accessToken ${auth_access_token} --userProperties ${user_properties} --userType ${user_type}"
}
""";

        var version = JsonVersionParser.ParseFromJsonString(json, options);

        var parsedArgs = version.GameArguments
            .SelectMany(arg => arg.Values ?? Enumerable.Empty<string>())
            .ToArray();
        Assert.Equal(new[]
        {
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
        }, parsedArgs);
    }

    [Fact]
    public void parse_from_vanilla_game_arg_array() // 1.13 ~
    {
        // release 1.13 ~
        // release 1.19 ~ : add --clientId ${clientid} --xuid ${auth_xuid}
        // release 1.20 ~ : add --quickPlayPath, --quickPlaySingleplayer, --quickPlayMultiplayer, --quickPlayRealms

        var json = """
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

        var version = JsonVersionParser.ParseFromJsonString(json, options);
        
        var parsedArgs = version.JvmArguments
            .SelectMany(arg => arg.Values ?? Enumerable.Empty<string>())
            .ToArray();
        Assert.Equal(new [] 
        {
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
        }, parsedArgs);
    }

    [Fact]
    public void parse_from_vanilla_jvm_arg_array()
    {
        // release 1.13 ~
        // release 1.20 ~ : add -Djna.tmpdir=, -Dorg.lwjgl.system.SharedLibraryExtractPath, -Dio.netty.native.workdir

        var json = """
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

        var version = JsonVersionParser.ParseFromJsonString(json, options);

        var parsedArgs = version.JvmArguments
            .SelectMany(args => args.Values ?? Enumerable.Empty<string>())
            .ToArray();
        Assert.Equal(new []
        {
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
            "${classpath}"
        }, parsedArgs);
    }
}