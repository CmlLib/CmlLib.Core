using CmlLib.Utils;
using System.Diagnostics;
using CmlLib.Core.Version;

namespace CmlLib.Core;

public class MLaunch
{
    private const int DefaultServerPort = 25565;

    public static readonly string SupportVersion = "1.18.2";
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
        this.minecraftPath = option.GetMinecraftPath();
    }

    private readonly MinecraftPath minecraftPath;
    private readonly MLaunchOption launchOption;
    
    // make process that ready to launch game
    public Process GetProcess()
    {
        string arg = string.Join(" ", CreateArg());
        Process mc = new Process();
        mc.StartInfo.FileName = 
            useNotNull(launchOption.GetStartVersion().JavaBinaryPath, launchOption.GetJavaPath()) ?? "";
        mc.StartInfo.Arguments = arg;
        mc.StartInfo.WorkingDirectory = minecraftPath.BasePath;

        return mc;
    }

    // make library files into jvm classpath string
    private string createClassPath(MVersion version)
    {
        // if there is no libraries then launcher would need only one library file: <version>.jar file itself
        var classpath = new List<string>(version.Libraries?.Length ?? 1);

        // libraries 
        if (version.Libraries != null)
        {
            var libraries = version.Libraries
                .Where(lib => lib.IsRequire && !lib.IsNative && !string.IsNullOrEmpty(lib.Path))
                .Select(lib => Path.GetFullPath(Path.Combine(minecraftPath.Library, lib.Path!)));
            classpath.AddRange(libraries);
        }

        // <version>.jar file
        if (!string.IsNullOrEmpty(version.Jar))
            classpath.Add(minecraftPath.GetVersionJarPath(version.Jar));

        var classpathStr = IOUtil.CombinePath(classpath.ToArray());
        return classpathStr;
    }

    private string createNativePath(MVersion version)
    {
        var native = new MNative(minecraftPath, version);
        native.CleanNatives();
        var nativePath = native.ExtractNatives();
        return nativePath;
    }

    public string[] CreateArg()
    {
        MVersion version = launchOption.GetStartVersion();
        var args = new List<string>();
        
        var classpath = createClassPath(version);
        var nativePath = createNativePath(version);
        var session = launchOption.GetSession();
        
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
            { "assets_index_name", version.Assets?.Id ?? "legacy" },
            { "auth_uuid"        , session.UUID },
            { "auth_access_token", session.AccessToken },
            { "user_properties"  , "{}" },
            { "auth_xuid"        , session.Xuid ?? "xuid" },
            { "clientid"         , launchOption.ClientId ?? "clientId" },
            { "user_type"        , session.UserType ?? "Mojang" },
            { "game_assets"      , minecraftPath.GetAssetLegacyPath(version.Assets?.Id ?? "legacy") },
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

        // JVM argument
        
        // version-specific jvm arguments
        if (version.JvmArguments != null)
            args.AddRange(Mapper.MapInterpolation(version.JvmArguments, argDict));
        
        // default jvm arguments
        if (launchOption.JVMArguments != null)
            args.AddRange(launchOption.JVMArguments);
        else
        {
            if (launchOption.MaximumRamMb > 0)
                args.Add("-Xmx" + launchOption.MaximumRamMb + "m");

            if (launchOption.MinimumRamMb > 0)
                args.Add("-Xms" + launchOption.MinimumRamMb + "m");
            
            args.AddRange(DefaultJavaParameter);
        }

        if (version.JvmArguments == null)
        {
            args.Add("-Djava.library.path=" + handleEmpty(nativePath));
            args.Add("-cp " + classpath);
        }

        // for macOS
        if (!string.IsNullOrEmpty(launchOption.DockName))
            args.Add("-Xdock:name=" + handleEmpty(launchOption.DockName));
        if (!string.IsNullOrEmpty(launchOption.DockIcon))
            args.Add("-Xdock:icon=" + handleEmpty(launchOption.DockIcon));

        // logging
        var loggingArgument = version.LoggingClient?.Argument;
        if (!string.IsNullOrEmpty(loggingArgument))
            args.Add(Mapper.Interpolation(loggingArgument, new Dictionary<string, string?>()
            {
                { "path", minecraftPath.GetLogConfigFilePath(version.LoggingClient?.LogFile?.Id ?? version.Id) }
            }, true));

        // main class
        if (!string.IsNullOrEmpty(version.MainClass))
            args.Add(version.MainClass);

        // game arguments
        if (version.GameArguments != null)
            args.AddRange(Mapper.MapInterpolation(version.GameArguments, argDict));
        else if (!string.IsNullOrEmpty(version.MinecraftArguments))
            args.AddRange(Mapper.MapInterpolation(version.MinecraftArguments.Split(' '), argDict));

        // options
        if (!string.IsNullOrEmpty(launchOption.ServerIp))
        {
            args.Add("--server " + handleEmpty(launchOption.ServerIp));

            if (launchOption.ServerPort != DefaultServerPort)
                args.Add("--port " + launchOption.ServerPort);
        }

        if (launchOption.ScreenWidth > 0 && launchOption.ScreenHeight > 0)
        {
            args.Add("--width " + launchOption.ScreenWidth);
            args.Add("--height " + launchOption.ScreenHeight);
        }

        if (launchOption.FullScreen)
            args.Add("--fullscreen");

        return args.ToArray();
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
