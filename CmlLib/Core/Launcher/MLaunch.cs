using CmlLib.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CmlLib.Core
{
    public class MLaunch
    {
        private const int DefaultServerPort = 25565;

        public static readonly string SupportVersion = "1.16.6";
        public readonly static string[] DefaultJavaParameter = 
            {
                "-XX:+UnlockExperimentalVMOptions",
                "-XX:+UseG1GC",
                "-XX:G1NewSizePercent=20",
                "-XX:G1ReservePercent=20",
                "-XX:MaxGCPauseMillis=50",
                "-XX:G1HeapRegionSize=16M"
            };

        public MLaunch(MLaunchOption option)
        {
            option.CheckValid();
            LaunchOption = option;
            this.minecraftPath = option.Path;
        }

        private readonly MinecraftPath minecraftPath;
        public MLaunchOption LaunchOption { get; private set; }

        /// <summary>
        /// Start Game
        /// </summary>
        public void Start()
        {
            GetProcess().Start();
        }

        /// <summary>
        /// Build game process and return it
        /// </summary>
        public Process GetProcess()
        {
            string arg = string.Join(" ", CreateArg());
            Process mc = new Process();
            mc.StartInfo.FileName = LaunchOption.JavaPath;
            mc.StartInfo.Arguments = arg;
            mc.StartInfo.WorkingDirectory = minecraftPath.BasePath;

            return mc;
        }

        [MethodTimer.Time]
        public string[] CreateArg()
        {
            var version = LaunchOption.StartVersion;

            var args = new List<string>();

            // Common JVM Arguments
            if (LaunchOption.JVMArguments != null)
                args.AddRange(LaunchOption.JVMArguments);
            else
            {
                args.AddRange(DefaultJavaParameter);

                if (LaunchOption.MaximumRamMb > 0)
                    args.Add("-Xmx" + LaunchOption.MaximumRamMb + "m");

                if (LaunchOption.MinimumRamMb > 0)
                    args.Add("-Xms" + LaunchOption.MinimumRamMb + "m");
            }

            if (!string.IsNullOrEmpty(LaunchOption.DockName))
                args.Add("-Xdock:name=" + handleEmpty(LaunchOption.DockName));
            if (!string.IsNullOrEmpty(LaunchOption.DockIcon))
                args.Add("-Xdock:icon=" + handleEmpty(LaunchOption.DockIcon));

            // Version-specific JVM Arguments
            var classpath = new List<string>(version.Libraries.Length);

            var libraries = version.Libraries
                .Where(lib => lib.IsRequire && !lib.IsNative)
                .Select(lib => Path.GetFullPath(Path.Combine(minecraftPath.Library, lib.Path)));
            classpath.AddRange(libraries);
            classpath.Add(minecraftPath.GetVersionJarPath(version.Jar));

            var classpathStr = IOUtil.CombinePath(classpath.ToArray());

            var native = new MNative(minecraftPath, LaunchOption.StartVersion);
            native.CleanNatives();
            var nativePath = native.ExtractNatives();

            var jvmdict = new Dictionary<string, string>
            {
                { "natives_directory", nativePath },
                { "launcher_name", useNotNull(LaunchOption.GameLauncherName, "minecraft-launcher") },
                { "launcher_version", useNotNull(LaunchOption.GameLauncherVersion, "2") },
                { "classpath", classpathStr }
            };

            if (version.JvmArguments != null)
                args.AddRange(Mapper.MapInterpolation(version.JvmArguments, jvmdict));
            else
            {
                args.Add("-Djava.library.path=" + handleEmpty(nativePath));
                args.Add("-cp " + classpathStr);
            }

            args.Add(version.MainClass);

            // Game Arguments
            var gameDict = new Dictionary<string, string>
            {
                { "auth_player_name" , LaunchOption.Session.Username },
                { "version_name"     , LaunchOption.StartVersion.Id },
                { "game_directory"   , minecraftPath.BasePath },
                { "assets_root"      , minecraftPath.Assets },
                { "assets_index_name", version.AssetId },
                { "auth_uuid"        , LaunchOption.Session.UUID },
                { "auth_access_token", LaunchOption.Session.AccessToken },
                { "user_properties"  , "{}" },
                { "user_type"        , "Mojang" },
                { "game_assets"      , minecraftPath.GetAssetLegacyPath(version.AssetId) },
                { "auth_session"     , LaunchOption.Session.AccessToken },
                { "version_type"     , useNotNull(LaunchOption.VersionType, version.TypeStr) }
            };

            if (version.GameArguments != null)
                args.AddRange(Mapper.MapInterpolation(version.GameArguments, gameDict));
            else
                args.AddRange(Mapper.MapInterpolation(version.MinecraftArguments.Split(' '), gameDict));

            // Options
            if (!string.IsNullOrEmpty(LaunchOption.ServerIp))
            {
                args.Add("--server " + handleEmpty(LaunchOption.ServerIp));

                if (LaunchOption.ServerPort != DefaultServerPort)
                    args.Add("--port " + LaunchOption.ServerPort);
            }

            if (LaunchOption.ScreenWidth > 0 && LaunchOption.ScreenHeight > 0)
            {
                args.Add("--width " + LaunchOption.ScreenWidth);
                args.Add("--height " + LaunchOption.ScreenHeight);
            }

            if (LaunchOption.FullScreen)
                args.Add("--fullscreen");

            return args.ToArray();
        }

        // if input1 is null, return input2
        private string useNotNull(string input1, string input2)
        {
            if (string.IsNullOrEmpty(input1))
                return input2;
            else
                return input1;
        }

        private string handleEmpty(string input)
        {
            if (input.Contains(" "))
                return "\"" + input + "\"";
            else
                return input;
        }
    }
}
