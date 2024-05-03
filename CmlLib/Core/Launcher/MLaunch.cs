using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CmlLib.Core.Version;
using CmlLib.Utils;
using MethodTimer;

namespace CmlLib.Core;

public class MLaunch
{
    private const int DefaultServerPort = 25565;

    public static readonly string SupportVersion = "1.17.1";

    public static readonly string[] DefaultJavaParameter =
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

    private readonly MLaunchOption launchOption;

    private readonly MinecraftPath minecraftPath;

    public MLaunch(MLaunchOption option)
    {
        option.CheckValid();
        launchOption = option;
        minecraftPath = option.GetMinecraftPath();
    }

    public Process GetProcess(bool needForce)
    {
        var arg = string.Join(" ", CreateArg(needForce));
        var mc = new Process();
        mc.StartInfo.FileName =
            useNotNull(launchOption.GetStartVersion().JavaBinaryPath, launchOption.GetJavaPath()) ?? "";
        mc.StartInfo.Arguments = arg;
        mc.StartInfo.WorkingDirectory = minecraftPath.BasePath;

        return mc;
    }

    private string createClassPath(MVersion version)
    {
        var classpath = new List<string>(version.Libraries?.Length ?? 1);

        if (version.Libraries != null)
        {
            var libraries = version.Libraries
                .Where(lib => lib.IsRequire && !lib.IsNative && !string.IsNullOrEmpty(lib.Path))
                .Select(lib => Path.GetFullPath(Path.Combine(minecraftPath.Library, lib.Path!)));
            classpath.AddRange(libraries);
        }

        if (!string.IsNullOrEmpty(version.Jar))
            classpath.Add(minecraftPath.GetVersionJarPath(version.Jar));

        var classpathStr = IOUtil.CombinePath(classpath.ToArray());
        return classpathStr;
    }

    private string createNativePath(MVersion version, bool needForce)
    {
        var native = new MNative(minecraftPath, version);
        // native.CleanNatives();
        var nativePath = native.ExtractNatives(needForce);
        return nativePath;
    }

    [Time]
    public string[] CreateArg(bool needForce)
    {
        var version = launchOption.GetStartVersion();
        var args = new List<string>();

        var classpath = ValidatePath(createClassPath(version));
        var nativePath = ValidatePath(createNativePath(version, needForce));
        var session = launchOption.GetSession();

        var argDict = new Dictionary<string, string?>
        {
            { "library_directory", ValidatePath(minecraftPath.Library) },
            { "natives_directory", nativePath },
            { "launcher_name", useNotNull(launchOption.GameLauncherName, "minecraft-launcher") },
            { "launcher_version", useNotNull(launchOption.GameLauncherVersion, "2") },
            { "classpath_separator", ";" },
            { "classpath", classpath },

            { "auth_player_name", session.Username },
            { "auth_uuid", session.UUID },
            { "auth_access_token", session.AccessToken },
            { "auth_xuid", session.Xuid ?? "xuid" },
            { "user_type", session.UserType ?? "Mojang" },
            { "auth_session", session.AccessToken },

            { "version_name", version.Id },
            { "game_directory", ValidatePath(minecraftPath.BasePath) },
            { "assets_root", ValidatePath(minecraftPath.Assets) },
            { "assets_index_name", version.AssetId ?? "legacy" },
            { "user_properties", "{}" },
            { "clientid", launchOption.ClientId ?? "clientId" },
            { "game_assets", minecraftPath.GetAssetLegacyPath(version.AssetId ?? "legacy") },
            { "version_type", useNotNull(launchOption.VersionType, version.TypeStr) }
        };

        // JVM argument

        // version-specific jvm arguments
        if (version.JvmArguments != null)
            args.AddRange(Mapper.MapInterpolation(version.JvmArguments, argDict));

        // default jvm arguments

        args.AddRange(DefaultJavaParameter);
        if (launchOption.JVMArguments != null && launchOption.JVMArguments.Any())
        {
            args.AddRange(launchOption.JVMArguments);
        }
        else
        {
            if (launchOption.MaximumRamMb > 0)
                args.Add("-Xmx" + launchOption.MaximumRamMb + "m");

            if (launchOption.MinimumRamMb > 0)
                args.Add("-Xms" + launchOption.MinimumRamMb + "m");
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
            args.Add(Mapper.Interpolation(loggingArgument, new Dictionary<string, string?>
            {
                { "path", ValidatePath(minecraftPath.GetLogConfigFilePath(version.LoggingClient?.Id ?? version.Id)) }
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
            if (launchOption.ServerPort != DefaultServerPort)
                args.Add("--quickPlayMultiplayer " + $"{launchOption.ServerIp}:{launchOption.ServerPort}");
            else
                args.Add("--quickPlayMultiplayer " + $"{launchOption.ServerIp}");
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

    private string? ValidatePath(string library)
    {
        return MRule.OSName == MRule.Windows
            ? library.Replace("/", "\\")
            : library.Replace("\\", "/");
    }

    // if input1 is null, return input2
    private string? useNotNull(string? input1, string? input2)
    {
        if (string.IsNullOrEmpty(input1))
            return input2;
        return input1;
    }

    private string? handleEmpty(string? input)
    {
        if (input == null)
            return null;
        return "\"" + input + "\"";

        if (input.Contains(" "))
            return "\"" + input + "\"";
        else
            return input;
    }
}
