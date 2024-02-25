using CmlLib.Core.Rules;
using CmlLib.Core.Version;
using CmlLib.Core.Internals;
using System.Diagnostics;

namespace CmlLib.Core.ProcessBuilder;

public class MinecraftProcessBuilder
{
    private const int DefaultServerPort = 25565;

    public static readonly string SupportVersion = "1.20.1";
    public readonly static string[] DefaultJavaParameter = 
        {
            "-XX:+UnlockExperimentalVMOptions",
            "-XX:+UseG1GC",
            "-XX:G1NewSizePercent=20",
            "-XX:G1ReservePercent=20",
            "-XX:MaxGCPauseMillis=50",
            "-XX:G1HeapRegionSize=16M",
            "-Dlog4j2.formatMsgNoLookups=true"
            // "-Xss1M"
        };

    public MinecraftProcessBuilder(
        IRulesEvaluator evaluator, 
        MLaunchOption option)
    {
        option.CheckValid();

        Debug.Assert(option.StartVersion != null);
        Debug.Assert(option.Path != null);
        Debug.Assert(option.RulesContext != null);

        launchOption = option;
        version = option.StartVersion;
        minecraftPath = option.Path;
        rulesContext = option.RulesContext;
        rulesEvaluator = evaluator;
    }

    private readonly IVersion version;
    private readonly RulesEvaluatorContext rulesContext;
    private readonly IRulesEvaluator rulesEvaluator;
    private readonly MinecraftPath minecraftPath;
    private readonly MLaunchOption launchOption;
    
    public Process CreateProcess()
    {
        var arg = string.Join(" ", BuildArguments());
        var mc = new Process();
        mc.StartInfo.FileName = launchOption.JavaPath!;
        mc.StartInfo.Arguments = arg;
        mc.StartInfo.WorkingDirectory = minecraftPath.BasePath;

        return mc;
    }

    public IEnumerable<string> BuildArguments()
    {
        var argDict = buildArgumentDictionary();

        var jvmArgs = buildJvmArguments(argDict);
        foreach (var item in jvmArgs)
            yield return item;
        
        var gameArgs = buildGameArguments(argDict);
        foreach (var item in gameArgs)
            yield return item;
    }

    private Dictionary<string, string?> buildArgumentDictionary()
    {
        Debug.Assert(launchOption.Session != null);

        var classpaths = getClasspaths();
        var classpath = IOUtil.CombinePath(classpaths);
        var assetId = version.GetInheritedProperty(version => version.AssetIndex?.Id) ?? "legacy";
        
        var argDict = new Dictionary<string, string?>
        {
            { "library_directory"  , minecraftPath.Library },
            { "natives_directory"  , launchOption.NativesDirectory },
            { "launcher_name"      , useNotNull(launchOption.GameLauncherName, "minecraft-launcher") },
            { "launcher_version"   , useNotNull(launchOption.GameLauncherVersion, "2") },
            { "classpath_separator", Path.PathSeparator.ToString() },
            { "classpath"          , classpath },

            { "auth_player_name" , launchOption.Session.Username },
            { "version_name"     , version.Id },
            { "game_directory"   , minecraftPath.BasePath },
            { "assets_root"      , minecraftPath.Assets },
            { "assets_index_name", assetId },
            { "auth_uuid"        , launchOption.Session.UUID },
            { "auth_access_token", launchOption.Session.AccessToken },
            { "user_properties"  , "{}" },
            { "auth_xuid"        , launchOption.Session.Xuid ?? "xuid" },
            { "clientid"         , launchOption.ClientId ?? "clientId" },
            { "user_type"        , launchOption.Session.UserType ?? "Mojang" },
            { "game_assets"      , minecraftPath.GetAssetLegacyPath(assetId) },
            { "auth_session"     , launchOption.Session.AccessToken },
            { "version_type"     , useNotNull(launchOption.VersionType, version.Type) },
        };

        if (launchOption.ArgumentDictionary != null)
        {
            foreach (var argument in launchOption.ArgumentDictionary)
            {
                argDict[argument.Key] = argument.Value;
            }
        }

        return argDict;
    }

    private IEnumerable<string> buildJvmArguments(Dictionary<string, string?> argDict)
    {
        var builder = new ProcessArgumentBuilder();

        if (launchOption.JvmArgumentOverrides != null)
        {
            // override all jvm arguments
            // even if necessary arguments are missing (-cp, -Djava.library.path),
            // the builder will still add the necessary arguments
            builder.AddRange(mapArguments(launchOption.JvmArgumentOverrides, argDict));
        }
        else
        {
            // version-specific jvm arguments
            var jvmArgs = version.ConcatInheritedCollection(v => v.JvmArguments);
            builder.AddRange(mapArguments(jvmArgs, argDict));

            // default jvm arguments
            builder.AddRange(DefaultJavaParameter);
        }

        // add extra jvm arguments
        builder.AddRange(mapArguments(launchOption.ExtraJvmArguments, argDict));

        // libraries
        builder.TryAddKeyValue("-Djava.library.path", argDict["natives_directory"]);
        if (!builder.CheckKeyAdded("-cp"))
        {
            builder.Add("-cp");
            builder.AddRaw(argDict["classpath"]);
        }

        // -Xmx, -Xms
        if (!checkXmxAdded(builder) && launchOption.MaximumRamMb > 0)
            builder.Add("-Xmx" + launchOption.MaximumRamMb + "m");
        if (!checkXmsAdded(builder) && launchOption.MinimumRamMb > 0)
            builder.Add("-Xms" + launchOption.MinimumRamMb + "m");
            
        // for macOS
        if (!string.IsNullOrEmpty(launchOption.DockName))
            builder.TryAddKeyValue("-Xdock:name", launchOption.DockName);
        if (!string.IsNullOrEmpty(launchOption.DockIcon))
            builder.TryAddKeyValue("-Xdock:icon", launchOption.DockIcon);

        // logging
        var logging = version.GetInheritedProperty(v => v.Logging);
        if (!string.IsNullOrEmpty(logging?.Argument))
        {
            builder.AddRange(mapArgument(new MArgument(logging.Argument), new Dictionary<string, string?>()
            {
                { "path", minecraftPath.GetLogConfigFilePath(logging.LogFile?.Id ?? version.Id) }
            }));
        }

        // main class
        var mainClass = version.GetInheritedProperty(v => v.MainClass);
        if (!string.IsNullOrEmpty(mainClass))
            builder.Add(mainClass);

        return builder.Build();
    }

    private bool checkXmxAdded(ProcessArgumentBuilder builder)
    {
        return builder.Keys.Any(k => k.StartsWith("-Xmx"));
    }

    private bool checkXmsAdded(ProcessArgumentBuilder builder)
    {
        return builder.Keys.Any(k => k.StartsWith("-Xms"));
    }

    private IEnumerable<string> buildGameArguments(Dictionary<string, string?> argDict)
    {
        var builder = new ProcessArgumentBuilder();

        // game arguments
        var gameArgs = version.ConcatInheritedCollection(v => v.GameArguments);
        builder.AddRange(mapArguments(gameArgs, argDict));

        // add extra game arguments
        builder.AddRange(mapArguments(launchOption.ExtraGameArguments, argDict));

        // server
        if (!string.IsNullOrEmpty(launchOption.ServerIp))
        {
            if (!builder.CheckKeyAdded("--server"))
                builder.AddRange("--server", launchOption.ServerIp);

            if (launchOption.ServerPort != DefaultServerPort && !builder.CheckKeyAdded("--port"))
                builder.AddRange("--port", launchOption.ServerPort.ToString());
        }

        // screen size
        if (launchOption.ScreenWidth > 0 && launchOption.ScreenHeight > 0)
        {
            if (!builder.CheckKeyAdded("--width"))
                builder.AddRange("--width", launchOption.ScreenWidth.ToString());
            if (!builder.CheckKeyAdded("--height"))
                builder.AddRange("--height", launchOption.ScreenHeight.ToString());
        }

        // fullscreen
        if (!builder.CheckKeyAdded("--fullscreen") && launchOption.FullScreen)
            builder.Add("--fullscreen");

        return builder.Build();
    }

    // make library files into jvm classpath string
    private IEnumerable<string> getClasspaths()
    {
        // libraries
        var libPaths = version
            .ConcatInheritedCollection(v => v.Libraries)
            .Where(lib => lib.CheckIsRequired(JsonVersionParserOptions.ClientSide))
            .Where(lib => lib.Rules == null || rulesEvaluator.Match(lib.Rules, rulesContext))
            .Where(lib => lib.Artifact != null)
            .Select(lib => Path.Combine(minecraftPath.Library, lib.GetLibraryPath()));

        foreach (var item in libPaths)
            yield return item;
            
        // <version>.jar file
        // TODO: decide what Jar file should be used. current jar or parent jar
        var jar = version.GetInheritedProperty(v => v.Jar);
        if (string.IsNullOrEmpty(jar))
            jar = version.Id; 
        yield return (minecraftPath.GetVersionJarPath(jar));
    }

    // if input1 is null, return input2
    private string? useNotNull(string? input1, string? input2)
    {
        if (string.IsNullOrEmpty(input1))
            return input2;
        else
            return input1;
    }

    private IEnumerable<string> mapArguments(IEnumerable<MArgument> arguments, Dictionary<string, string?> mapper)
    {
        foreach (var arg in arguments)
        {
            foreach (var mappedArg in mapArgument(arg, mapper))
            {
                yield return mappedArg;
            }
        }
    }

    private IEnumerable<string> mapArgument(MArgument arg, Dictionary<string, string?> mapper)
    {
        if (arg.Values == null)
            yield break;

        if (arg.Rules != null)
        {
            var isMatch = rulesEvaluator.Match(arg.Rules, rulesContext);
            if (!isMatch)
                yield break;
        }

        foreach (var value in arg.Values)
        {
            var mappedValue = Mapper.Interpolation(value, mapper);
            if (!string.IsNullOrEmpty(mappedValue))
                yield return mappedValue;
        }
    }
}
