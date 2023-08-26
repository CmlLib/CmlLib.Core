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

        launchOption = option;
        version = option.GetStartVersion();
        minecraftPath = option.GetMinecraftPath();
        rulesContext = option.GetRulesContext();
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
        var classpaths = getClasspaths();
        var classpath = IOUtil.CombinePath(classpaths);
        var session = launchOption.GetSession();
        var assetId = version.GetInheritedProperty(version => version.AssetIndex?.Id) ?? "legacy";
        
        var argDict = new Dictionary<string, string?>
        {
            { "library_directory"  , minecraftPath.Library },
            { "natives_directory"  , launchOption.NativesDirectory },
            { "launcher_name"      , useNotNull(launchOption.GameLauncherName, "minecraft-launcher") },
            { "launcher_version"   , useNotNull(launchOption.GameLauncherVersion, "2") },
            { "classpath_separator", Path.PathSeparator.ToString() },
            { "classpath"          , classpath },

            { "auth_player_name" , session.Username },
            { "version_name"     , version.Id },
            { "game_directory"   , minecraftPath.BasePath },
            { "assets_root"      , minecraftPath.Assets },
            { "assets_index_name", assetId },
            { "auth_uuid"        , session.UUID },
            { "auth_access_token", session.AccessToken },
            { "user_properties"  , "{}" },
            { "auth_xuid"        , session.Xuid ?? "xuid" },
            { "clientid"         , launchOption.ClientId ?? "clientId" },
            { "user_type"        , session.UserType ?? "Mojang" },
            { "game_assets"      , minecraftPath.GetAssetLegacyPath(assetId) },
            { "auth_session"     , session.AccessToken },
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
        var builder = new ProcessArgumentBuilder(rulesEvaluator, rulesContext);

        // version-specific jvm arguments
        var jvmArgs = version.ConcatInheritedCollection(v => v.JvmArguments);
        builder.AddArguments(jvmArgs, argDict);
        
        // default jvm arguments
        if (launchOption.JVMArguments != null)
            foreach (var item in launchOption.JVMArguments)
                builder.Add(item);
        else
            foreach (var item in DefaultJavaParameter)
                builder.Add(item);

        // libraries
        builder.TryAddKeyValue("-Djava.library.path", argDict["natives_directory"]);
        if (!builder.CheckKeyAdded("-cp"))
        {
            builder.Add("-cp");
            builder.AddRaw(argDict["classpath"]);
        }

        // -Xmx, -Xms
        if (!builder.CheckKeyAdded("-Xmx") && launchOption.MaximumRamMb > 0)
            builder.Add("-Xmx" + launchOption.MaximumRamMb + "m");
        if (!builder.CheckKeyAdded("-Xms") && launchOption.MinimumRamMb > 0)
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
            builder.AddArgument(new MArgument(logging.Argument), new Dictionary<string, string?>()
            {
                { "path", minecraftPath.GetLogConfigFilePath(logging.LogFile?.Id ?? version.Id) }
            });
        }

        // main class
        var mainClass = version.GetInheritedProperty(v => v.MainClass);
        if (!string.IsNullOrEmpty(mainClass))
            builder.Add(mainClass);

        return builder.Build();
    }

    private IEnumerable<string> buildGameArguments(Dictionary<string, string?> argDict)
    {
        var builder = new ProcessArgumentBuilder(rulesEvaluator, rulesContext);

        // game arguments
        var gameArgs = version.ConcatInheritedCollection(v => v.GameArguments);
        builder.AddArguments(gameArgs, argDict);

        // options
        if (!string.IsNullOrEmpty(launchOption.ServerIp))
        {
            builder.AddRange("--server", launchOption.ServerIp);

            if (launchOption.ServerPort != DefaultServerPort)
                builder.AddRange("--port", launchOption.ServerPort.ToString());
        }

        if (launchOption.ScreenWidth > 0 && launchOption.ScreenHeight > 0)
        {
            builder.AddRange("--width", launchOption.ScreenWidth.ToString());
            builder.AddRange("--height", launchOption.ScreenHeight.ToString());
        }

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
            .Where(lib => lib.CheckIsRequired("SIDE"))
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
}
