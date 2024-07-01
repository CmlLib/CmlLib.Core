using CmlLib.Core.Version;

namespace CmlLib.Core.Test.Version;

public class JsonVersionParserTests
{
    private readonly static JsonVersionParserOptions options = new JsonVersionParserOptions
    {
        Side = JsonVersionParserOptions.ClientSide,
        SkipError = false
    };

    [Fact]
    public void parse_basic_properties_from_vanilla()
    {
        // release 1.0 ~     : same
        // release 1.16.5 ~  : complianceLevel 1
        // release 1.17 ~    : java-runtime-alpha, 16
        // release 1.18 ~    : java-runtime-beta, 17
        // release 1.19 ~    : java-runtime-gamma, 17
        var json = """
{
    "id": "1.0",
    "javaVersion": {
        "component": "jre-legacy",
        "majorVersion": 8
    },
    "complianceLevel": 0,
    "mainClass": "net.minecraft.launchwrapper.Launch",
    "minimumLauncherVersion": 4,
    "releaseTime": "2011-11-17T22:00:00+00:00",
    "time": "2011-11-17T22:00:00+00:00",
    "type": "release"
}
""";
        var version = JsonVersionParser.ParseFromJsonString(json, options);

        Assert.Equal("1.0", version.Id);
        Assert.Null(version.InheritsFrom);
        Assert.Equal("jre-legacy", version.JavaVersion?.Component);
        Assert.Equal("8", version.JavaVersion?.MajorVersion);
        Assert.Null(version.Jar);
        Assert.Equal("net.minecraft.launchwrapper.Launch", version.MainClass);
        Assert.Equal("release", version.Type);
        Assert.Equal(DateTime.Parse("2011-11-17T22:00:00+00:00"), version.ReleaseTime);
        Assert.Equal("4", version.GetProperty("minimumLauncherVersion"));
    }

    [Fact]
    public void parse_asset_from_vanilla()
    {
        // assetId
        // release 1.0 ~ 1.5.2   : pre-1.6
        // release 1.6.1 ~ 1.7.2 : legacy
        // release 1.7.3 ~       : inconsistent
        var json = """
{
    "id": "1.0",
    "assetIndex": {
        "id": "pre-1.6",
        "sha1": "3d8e55480977e32acd9844e545177e69a52f594b",
        "size": 74091,
        "totalSize": 49505710,
        "url": "https://launchermeta.mojang.com/v1/packages/3d8e55480977e32acd9844e545177e69a52f594b/pre-1.6.json"
    },
    "assets": "pre-1.6"
}
""";

        var version = JsonVersionParser.ParseFromJsonString(json, options);

        Assert.Equal("pre-1.6", version.AssetIndex?.Id);
        Assert.Equal("3d8e55480977e32acd9844e545177e69a52f594b", version.AssetIndex?.Sha1);
        Assert.Equal(74091, version.AssetIndex?.Size);
        Assert.Equal(49505710, version.AssetIndex?.TotalSize);
        Assert.Equal("https://launchermeta.mojang.com/v1/packages/3d8e55480977e32acd9844e545177e69a52f594b/pre-1.6.json", version.AssetIndex?.Url);
    }

    [Fact]
    public void parse_client_from_vanilla()
    {
        // release 1.0 ~     : only client
        // release 1.2.5 ~   : client, server, some windows_server
        // release 1.14.4 ~  : client, client_mappings, server, server_mapping
        var json = """
{
    "id": "1.0",
    "downloads": {
        "client": {
            "sha1": "b679fea27f2284836202e9365e13a82552092e5d",
            "size": 2362837,
            "url": "https://launcher.mojang.com/v1/objects/b679fea27f2284836202e9365e13a82552092e5d/client.jar"
        },
        "server": {
            "sha1": "0252918a5f9d47e3c6eb1dfec02134d1374a89b4",
            "size": 6132004,
            "url": "https://launcher.mojang.com/v1/objects/0252918a5f9d47e3c6eb1dfec02134d1374a89b4/server.jar"
        },
        "windows_server": {
            "sha1": "f495386d0eded7346e7e77a1c6d7dfc5a5dae068",
            "size": 6527780,
            "url": "https://launcher.mojang.com/v1/objects/f495386d0eded7346e7e77a1c6d7dfc5a5dae068/windows_server.exe"
        }
    }
}        
""";

        var version = JsonVersionParser.ParseFromJsonString(json, options);

        Assert.Equal("b679fea27f2284836202e9365e13a82552092e5d", version.Client?.Sha1);
        Assert.Equal(2362837, version.Client?.Size);
        Assert.Equal("https://launcher.mojang.com/v1/objects/b679fea27f2284836202e9365e13a82552092e5d/client.jar", version.Client?.Url);
    }

    [Fact]
    public void parse_logging_from_vanilla()
    {
        // release before 1.7.2 : null
        // release 1.7.2 ~      : 1.7
        // release 1.12 ~       : 1.12
        var json = """
{
    "id": "1.7.2",
    "logging": {
        "client": {
            "argument": "-Dlog4j.configurationFile=${path}",
            "file": {
                "id": "client-1.7.xml",
                "sha1": "50c9cc4af6d853d9fc137c84bcd153e2bd3a9a82",
                "size": 966,
                "url": "https://launcher.mojang.com/v1/objects/50c9cc4af6d853d9fc137c84bcd153e2bd3a9a82/client-1.7.xml"
            },
            "type": "log4j2-xml"
        }
    }
}
""";

        var version = JsonVersionParser.ParseFromJsonString(json, options);

        Assert.Equal("-Dlog4j.configurationFile=${path}", version.Logging?.Argument);
        Assert.Equal("client-1.7.xml", version.Logging?.LogFile?.Id);
        Assert.Equal("50c9cc4af6d853d9fc137c84bcd153e2bd3a9a82", version.Logging?.LogFile?.Sha1);
        Assert.Equal(966, version.Logging?.LogFile?.Size);
        Assert.Equal("https://launcher.mojang.com/v1/objects/50c9cc4af6d853d9fc137c84bcd153e2bd3a9a82/client-1.7.xml", version.Logging?.LogFile?.Url);
        Assert.Equal("log4j2-xml", version.Logging?.Type);
    }

    [Fact]
    public void parse_from_optifine()
    {
        // OptiFine 1.7.10 from third-party launcher

        var json = """
{
    "id": "OptiFine 1.7.10",
    "time": "2014-05-14T21:29:07+04:00",
    "releaseTime": "2014-05-14T21:29:23+04:00",
    "type": "modified",
    "minecraftArguments": "--username ${auth_player_name} --version ${version_name} --gameDir ${game_directory} --assetsDir ${assets_root} --assetIndex ${assets_index_name} --uuid ${auth_uuid} --accessToken ${auth_access_token} --userProperties ${user_properties} --userType ${user_type} --tweakClass optifine.OptiFineTweaker",
    "mainClass": "net.minecraft.launchwrapper.Launch",
    "minimumLauncherVersion": 13,
    "assets": "1.7.10",
    "complianceLevel": 0.0,
    "javaVersion": {
        "component": "jre-legacy",
        "majorVersion": 8.0
    }
}
""";

        var version = JsonVersionParser.ParseFromJsonString(json, options);
        Assert.Equal("OptiFine 1.7.10", version.Id);
        Assert.Equal("modified", version.Type);
        Assert.Equal("8.0", version.JavaVersion?.MajorVersion);
    }
}