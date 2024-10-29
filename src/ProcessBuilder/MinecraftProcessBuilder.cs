using CmlLib.Core.Rules;
using CmlLib.Core.Version;
using CmlLib.Core.Internals;
using System.Diagnostics;

namespace CmlLib.Core.ProcessBuilder;

public class MinecraftProcessBuilder
{
    public MinecraftProcessBuilder(
        IRulesEvaluator evaluator, 
        RulesEvaluatorContext context,
        MLaunchOption option)
    {
        option.CheckValid();

        Debug.Assert(option.StartVersion != null);
        Debug.Assert(option.Path != null);

        launchOption = option;
        version = option.StartVersion;
        minecraftPath = option.Path;
        rulesEvaluator = evaluator;
        baseRulesContext = context;
    }

    private readonly IVersion version;
    private readonly IRulesEvaluator rulesEvaluator;
    private readonly RulesEvaluatorContext baseRulesContext;
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
        var features = baseRulesContext.Features
            .Concat(launchOption.Features)
            .Concat(addFeatures());
            
        var context = baseRulesContext with 
        { 
            Features = new HashSet<string>(features)
        };

        var argDict = buildArgumentDictionary(context);

        var builder = new MinecraftArgumentBuilder(rulesEvaluator, context, argDict);
        addJvmArguments(builder);
        addGameArguments(builder);
        return builder.Build();
    }

    private IEnumerable<string> addFeatures()
    {
        if (launchOption.IsDemo)
        {
            yield return "is_demo_user";
        }

        if (launchOption.ScreenWidth > 0 && 
            launchOption.ScreenHeight > 0)
        {
            yield return "has_custom_resolution";
        }

        if (!string.IsNullOrEmpty(launchOption.QuickPlayPath))
        {
            yield return "has_quick_plays_support";
        }

        if (!string.IsNullOrEmpty(launchOption.QuickPlaySingleplayer))
        {
            yield return "is_quick_play_singleplayer";
        }

        if (!string.IsNullOrEmpty(launchOption.ServerIp))
        {
            yield return "is_quick_play_multiplayer";
        }

        if (!string.IsNullOrEmpty(launchOption.QuickPlayRealms))
        {
            yield return "is_quick_play_realms";
        }
    }

    private IReadOnlyDictionary<string, string?> buildArgumentDictionary(RulesEvaluatorContext context)
    {
        Debug.Assert(launchOption.Session != null);

        var classpaths = getClasspaths(context);
        var classpath = IOUtil.CombinePath(classpaths, launchOption.PathSeparator);
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

            { "resolution_width"     , launchOption.ScreenWidth.ToString() },
            { "resolution_height"    , launchOption.ScreenHeight.ToString() },
            { "quickPlayPath"        , launchOption.QuickPlayPath },
            { "quickPlaySingleplayer", launchOption.QuickPlaySingleplayer },
            { "quickPlayMultiplayer" , createAddress(launchOption.ServerIp, launchOption.ServerPort) },
            { "quickPlayRealms"      , launchOption.QuickPlayRealms }
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
    private IEnumerable<string> getClasspaths(RulesEvaluatorContext context)
    {
        var libNames = new HashSet<string>();
        var libPaths = version
            .EnumerateToParent()
            .SelectMany(v => v.Libraries)
            .Where(lib => lib.CheckIsRequired(JsonVersionParserOptions.ClientSide))
            .Where(lib => lib.Rules == null || rulesEvaluator.Match(lib.Rules, context))
            .Where(lib => lib.Artifact != null)
            .Where(lib => libNames.Add(getLibName(lib)))
            .Select(lib => Path.Combine(minecraftPath.Library, lib.GetLibraryPath()))
            .Concat([minecraftPath.GetVersionJarPath(version.MainJarId)]);
        return libPaths;

        static string getLibName(MLibrary lib)
        {
            try
            {
                var package = PackageName.Parse(lib.Name);
                return package.GetIdentifier();
            }
            catch
            {
                return lib.Name;
            }
        }
    }

    private string? createAddress(string? ip, int port)
    {
        if (port == MinecraftArgumentBuilder.DefaultServerPort)
            return ip;
        else
            return ip + ":" + port;
    }

    private void addJvmArguments(MinecraftArgumentBuilder builder)
    {
        if (launchOption.JvmArgumentOverrides != null)
        {
            // override all jvm arguments
            // even if necessary arguments are missing (-cp, -Djava.library.path),
            // the builder will still add the necessary arguments
            builder.AddArguments(launchOption.JvmArgumentOverrides);
        }
        else
        {
            // version-specific jvm arguments
            var jvmArgs = version.ConcatInheritedJvmArguments().ToList();
            if (jvmArgs.Any())
                builder.AddArguments(jvmArgs);
            else
                builder.AddArguments(MLaunchOption.DefaultJvmArguments);

            // add extra jvm arguments
            builder.AddArguments(launchOption.ExtraJvmArguments);
        }

        // native library
        builder.TryAddNativesDirectory();

        // classpath
        builder.TryAddClassPath();

        // -Xmx
        if (launchOption.MaximumRamMb > 0)
            builder.TryAddXmx(launchOption.MaximumRamMb);

        // -Xms
        if (launchOption.MinimumRamMb > 0)
            builder.TryAddXms(launchOption.MinimumRamMb);
            
        // for macOS
        if (!string.IsNullOrEmpty(launchOption.DockName))
            builder.TryAddDockName(launchOption.DockName);
        if (!string.IsNullOrEmpty(launchOption.DockIcon))
            builder.TryAddDockIcon(launchOption.DockIcon);

        // logging
        var logging = version.GetInheritedProperty(v => v.Logging);
        if (!string.IsNullOrEmpty(logging?.Argument))
        {
            var logArguments = MArgument.FromCommandLine(logging.Argument);
            builder.AddArguments([logArguments], new Dictionary<string, string?>()
            {
                { "path", minecraftPath.GetLogConfigFilePath(logging.LogFile?.Id ?? version.Id) }
            });
        }

        // main class
        var mainClass = version.GetInheritedProperty(v => v.MainClass);
        if (!string.IsNullOrEmpty(mainClass))
            builder.AddArguments([mainClass]);
    }

    private void addGameArguments(MinecraftArgumentBuilder builder)
    {
        // game arguments
        builder.AddArguments(version.ConcatInheritedGameArguments());

        // add extra game arguments
        builder.AddArguments(launchOption.ExtraGameArguments);

        // demo
        if (launchOption.IsDemo)
            builder.SetDemo();

        // screen size
        if (launchOption.ScreenWidth > 0 && launchOption.ScreenHeight > 0)
            builder.TryAddScreenResolution(launchOption.ScreenWidth, launchOption.ScreenHeight);

        // quickPlayMultiplayer
        if (!string.IsNullOrEmpty(launchOption.ServerIp))
            builder.TryAddQuickPlayMultiplayer(launchOption.ServerIp, launchOption.ServerPort);

        // fullscreen
        if (launchOption.FullScreen)
            builder.SetFullscreen();
    }
}
