using CmlLib.Core.Rules;
using CmlLib.Core.Version;
using CmlLib.Core.Internals;
using CmlLib.Core.CommandParser;
using System.Diagnostics;

namespace CmlLib.Core.ProcessBuilder;

public class MinecraftProcessBuilder
{
    private const int DefaultServerPort = 25565;

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
        Debug.Assert(!string.IsNullOrEmpty(launchOption.JavaPath));

        var mc = new Process();
        mc.StartInfo.FileName = launchOption.JavaPath;
        mc.StartInfo.Arguments = BuildArguments();
        mc.StartInfo.WorkingDirectory = minecraftPath.BasePath;
        return mc;
    }

    public string BuildArguments()
    {
        var builder = new CommandLineBuilder();
        var argDict = buildArgumentDictionary();
        addJvmArguments(builder, argDict);
        addGameArguments(builder, argDict);
        return builder.Build();
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
            { "launcher_name"      , launchOption.GameLauncherName },
            { "launcher_version"   , launchOption.GameLauncherVersion },
            { "classpath_separator", launchOption.PathSeparator },
            { "classpath"          , classpath },

            { "auth_player_name" , launchOption.Session.Username },
            { "version_name"     , version.Id },
            { "game_directory"   , minecraftPath.BasePath },
            { "assets_root"      , minecraftPath.Assets },
            { "assets_index_name", assetId },
            { "auth_uuid"        , launchOption.Session.UUID },
            { "auth_access_token", launchOption.Session.AccessToken },
            { "user_properties"  , launchOption.UserProperties },
            { "auth_xuid"        , launchOption.Session.Xuid ?? "xuid" },
            { "clientid"         , launchOption.ClientId ?? "clientId" },
            { "user_type"        , launchOption.Session.UserType ?? "Mojang" },
            { "game_assets"      , minecraftPath.GetAssetLegacyPath(assetId) },
            { "auth_session"     , launchOption.Session.AccessToken },
            { "version_type"     , launchOption.VersionType ?? version.Type },
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
        yield return minecraftPath.GetVersionJarPath(jar);
    }

    private void addJvmArguments(CommandLineBuilder builder, Dictionary<string, string?> argDict)
    {
        if (launchOption.JvmArgumentOverrides != null)
        {
            // override all jvm arguments
            // even if necessary arguments are missing (-cp, -Djava.library.path),
            // the builder will still add the necessary arguments
            builder.AddArguments(getArguments(launchOption.JvmArgumentOverrides, argDict));
        }
        else
        {
            // version-specific jvm arguments
            var jvmArgs = version.ConcatInheritedCollection(v => v.JvmArguments);
            builder.AddArguments(getArguments(jvmArgs, argDict));
        }

        // add extra jvm arguments
        builder.AddArguments(getArguments(launchOption.ExtraJvmArguments, argDict));

        // native library
        if (!builder.ContainsKey("-Djava.library.path"))
            builder.AddArguments(["-Djava.library.path", argDict["natives_directory"] ?? ""]);

        // classpath
        if (!builder.ContainsKey("-cp"))
            builder.AddArguments(["-cp", argDict["classpath"] ?? ""]);

        // -Xmx, -Xms
        if (!builder.ContainsXmx() && launchOption.MaximumRamMb > 0)
            builder.AddArguments(["-Xmx" + launchOption.MaximumRamMb + "m"]);
        if (!builder.ContainsXms() && launchOption.MinimumRamMb > 0)
            builder.AddArguments(["-Xms" + launchOption.MinimumRamMb + "m"]);
            
        // for macOS
        if (!string.IsNullOrEmpty(launchOption.DockName) && !builder.ContainsKey("-Xdock:name"))
            builder.AddArguments(["-Xdock:name", launchOption.DockName]);
        if (!string.IsNullOrEmpty(launchOption.DockIcon) && !builder.ContainsKey("-Xdock:icon"))
            builder.AddArguments(["-Xdock:icon", launchOption.DockIcon]);

        // logging
        var logging = version.GetInheritedProperty(v => v.Logging);
        if (!string.IsNullOrEmpty(logging?.Argument))
        {
            builder.AddArguments(getArguments([new MArgument(logging.Argument)], new Dictionary<string, string?>()
            {
                { "path", minecraftPath.GetLogConfigFilePath(logging.LogFile?.Id ?? version.Id) }
            }));
        }

        // main class
        var mainClass = version.GetInheritedProperty(v => v.MainClass);
        if (!string.IsNullOrEmpty(mainClass))
            builder.AddArguments([mainClass]);
    }

    private void addGameArguments(CommandLineBuilder builder, Dictionary<string, string?> argDict)
    {
        // game arguments
        var gameArgs = version.ConcatInheritedCollection(v => v.GameArguments);
        builder.AddArguments(getArguments(gameArgs, argDict));

        // add extra game arguments
        builder.AddArguments(getArguments(launchOption.ExtraGameArguments, argDict));

        // server
        if (!string.IsNullOrEmpty(launchOption.ServerIp))
        {
            if (!builder.ContainsKey("--server"))
                builder.AddArguments(["--server", launchOption.ServerIp]);

            if (launchOption.ServerPort != DefaultServerPort && !builder.ContainsKey("--port"))
                builder.AddArguments(["--port", launchOption.ServerPort.ToString()]);
        }

        // screen size
        if (launchOption.ScreenWidth > 0 && launchOption.ScreenHeight > 0)
        {
            if (!builder.ContainsKey("--width"))
                builder.AddArguments(["--width", launchOption.ScreenWidth.ToString()]);
            if (!builder.ContainsKey("--height"))
                builder.AddArguments(["--height", launchOption.ScreenHeight.ToString()]);
        }

        // fullscreen
        if (!builder.ContainsKey("--fullscreen") && launchOption.FullScreen)
            builder.AddArguments(["--fullscreen"]);
    }

    private IEnumerable<string> getArguments(IEnumerable<MArgument> args, IReadOnlyDictionary<string, string?> varDict)
    {
        return args
            .Where(arg => rulesEvaluator.Match(arg.Rules, rulesContext))
            .SelectMany(arg => arg.InterpolateValues(varDict));
    }
}
