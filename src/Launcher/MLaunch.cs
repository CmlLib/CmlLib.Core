using CmlLib.Core.Launcher;
using CmlLib.Core.ProcessBuilder;
using CmlLib.Core.Rules;
using CmlLib.Core.Version;
using CmlLib.Core.Internals;
using System.Diagnostics;

namespace CmlLib.Core;

public class MLaunch
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

    public MLaunch(MLaunchOption option)
    {
        option.CheckValid();
        launchOption = option;
        version = option.GetStartVersion();
        this.minecraftPath = option.GetMinecraftPath();
        builder = new MinecraftArgumentBuilder(rulesEvaluator, launchOption.RulesContext!);
    }

    private readonly IVersion version;
    private readonly IRulesEvaluator rulesEvaluator = new RulesEvaluator();
    private readonly MinecraftArgumentBuilder builder;
    private readonly MinecraftPath minecraftPath;
    private readonly MLaunchOption launchOption;
    
    // make process that ready to launch game
    public Process CreateProcess()
    {
        string arg = string.Join(" ", BuildArguments());
        Process mc = new Process();
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
        var nativePath = createNativePath();
        var session = launchOption.GetSession();
        var assetId = version.GetInheritedProperty(version => version.AssetIndex?.Id) ?? "legacy";
        
        var argDict = new Dictionary<string, string?>
        {
            { "library_directory"  , minecraftPath.Library },
            { "natives_directory"  , nativePath },
            { "launcher_name"      , useNotNull(launchOption.GameLauncherName, "minecraft-launcher") },
            { "launcher_version"   , useNotNull(launchOption.GameLauncherVersion, "2") },
            { "classpath_separator", Path.PathSeparator.ToString() },
            { "classpath"          , classpath },

            { "auth_xuid"        , session.Xuid },
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
        // version-specific jvm arguments
        var jvmArgs = version.ConcatInheritedCollection(v => v.JvmArguments);
        foreach (var item in builder.Build(jvmArgs, argDict))
            yield return item;
        
        // default jvm arguments
        if (launchOption.JVMArguments != null && launchOption.JVMArguments.Count() != 0)
            foreach (var item in launchOption.JVMArguments)
                yield return item;
        else
        {
            if (launchOption.MaximumRamMb > 0)
                yield return ("-Xmx" + launchOption.MaximumRamMb + "m");

            if (launchOption.MinimumRamMb > 0)
                yield return ("-Xms" + launchOption.MinimumRamMb + "m");
            
            foreach (var item in DefaultJavaParameter)
                yield return item;
        }

        if (jvmArgs.Count() == 0)
        {
            yield return ("-Djava.library.path=" + handleEmpty(argDict["natives_directory"]));
            yield return ("-cp " + argDict["classpath"]);
        }

        // for macOS
        if (!string.IsNullOrEmpty(launchOption.DockName))
            yield return ("-Xdock:name=" + handleEmpty(launchOption.DockName));
        if (!string.IsNullOrEmpty(launchOption.DockIcon))
            yield return ("-Xdock:icon=" + handleEmpty(launchOption.DockIcon));

        // logging
        var logging = version.GetInheritedProperty(v => v.Logging);
        if (!string.IsNullOrEmpty(logging?.Argument))
        {
            var mappedArgs = builder.Build(new MArgument(logging.Argument), new Dictionary<string, string?>()
            {
                { "path", minecraftPath.GetLogConfigFilePath(logging.LogFile?.Id ?? version.Id) }
            });
            foreach (var item in mappedArgs)
                yield return item;
        }

        // main class
        var mainClass = version.GetInheritedProperty(v => v.MainClass);
        if (!string.IsNullOrEmpty(mainClass))
            yield return (mainClass);
    }

    private IEnumerable<string> buildGameArguments(Dictionary<string, string?> argDict)
    {
        // game arguments
        var gameArgs = version.ConcatInheritedCollection(v => v.GameArguments);
        foreach (var item in builder.Build(gameArgs, argDict))
            yield return item;

        // options
        if (!string.IsNullOrEmpty(launchOption.ServerIp))
        {
            yield return ("--server " + handleEmpty(launchOption.ServerIp));

            if (launchOption.ServerPort != DefaultServerPort)
                yield return ("--port " + launchOption.ServerPort);
        }

        if (launchOption.ScreenWidth > 0 && launchOption.ScreenHeight > 0)
        {
            yield return ("--width " + launchOption.ScreenWidth);
            yield return ("--height " + launchOption.ScreenHeight);
        }

        if (launchOption.FullScreen)
            yield return ("--fullscreen");
    }

    // make library files into jvm classpath string
    private IEnumerable<string> getClasspaths()
    {
        // libraries
        var libPaths = version
            .ConcatInheritedCollection(v => v.Libraries)
            .Where(lib => lib.CheckIsRequired("SIDE"))
            .Where(lib => lib.Rules == null || rulesEvaluator.Match(lib.Rules, launchOption.RulesContext!))
            .Where(lib => lib.Artifact != null)
            .Select(lib => lib.GetLibraryPath());

        foreach (var item in libPaths)
            yield return item;
            
        // <version>.jar file
        if (!string.IsNullOrEmpty(version.Jar)) // !!!!!!!!!!!!!!!!!!!!!!!!!
            yield return (minecraftPath.GetVersionJarPath(version.Jar));
    }

    private string createNativePath()
    {
        var native = new MNative(minecraftPath, version, rulesEvaluator, launchOption.RulesContext!);
        native.CleanNatives();
        var nativePath = native.ExtractNatives();
        return nativePath;
    }

    // if input1 is null, return input2
    private string? useNotNull(string? input1, string? input2)
    {
        if (string.IsNullOrEmpty(input1))
            return input2;
        else
            return input1;
    }

    private string? handleEmpty(string? input)
    {
        if (input == null)
            return null;
        
        if (input.Contains(" "))
            return "\"" + input + "\"";
        else
            return input;
    }
}
