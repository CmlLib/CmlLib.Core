using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CmlLib.Core
{
    public class MLaunch
    {
        private static Regex argBracket = new Regex(@"\$\{(.*?)}");
        private const int DefaultServerPort = 25565;

        public const string SupportVersion = "1.16.1";
        public readonly static string[] DefaultJavaParameter = new string[]
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
            this.MinecraftPath = option.Path;
        }

        MinecraftPath MinecraftPath;
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
            mc.StartInfo.WorkingDirectory = MinecraftPath.BasePath;

            return mc;
        }

        public string[] CreateArg()
        {
            var version = LaunchOption.StartVersion;

            var args = new List<string>();

            // Common JVM Arguments
            if (LaunchOption.JVMArguments != null)
                args.AddRange(LaunchOption.JVMArguments);
            else
                args.AddRange(DefaultJavaParameter);

            args.Add("-Xmx" + LaunchOption.MaximumRamMb + "m");

            if (!string.IsNullOrEmpty(LaunchOption.DockName))
                args.Add("-Xdock:name=" + handleEmpty(LaunchOption.DockName));
            if (!string.IsNullOrEmpty(LaunchOption.DockIcon))
                args.Add("-Xdock:icon=" + handleEmpty(LaunchOption.DockIcon));

            // Version-specific JVM Arguments
            var libArgs = new List<string>(version.Libraries.Length);

            foreach (var item in version.Libraries)
            {
                if (item.IsRequire && !item.IsNative)
                {
                    var libPath = Path.GetFullPath(Path.Combine(MinecraftPath.Library, item.Path));
                    libArgs.Add(handleEmpty(libPath));
                }
            }

            libArgs.Add(handleEmpty(Path.Combine(MinecraftPath.Versions, version.Jar, version.Jar + ".jar")));

            var libs = string.Join(Path.PathSeparator.ToString(), libArgs);

            var native = new MNative(MinecraftPath, LaunchOption.StartVersion);
            native.CleanNatives();
            var nativePath = native.ExtractNatives();

            var jvmdict = new Dictionary<string, string>()
            {
                { "natives_directory", handleEmpty(nativePath) },
                { "launcher_name", useNotNull(LaunchOption.GameLauncherName, "minecraft-launcher") },
                { "launcher_version", useNotNull(LaunchOption.GameLauncherVersion, "2") },
                { "classpath", libs }
            };

            if (version.JvmArguments != null)
                args.AddRange(argumentInsert(version.JvmArguments, jvmdict));
            else
            {
                args.Add("-Djava.library.path=" + handleEmpty(nativePath));
                args.Add("-cp " + libs);
            }

            args.Add(version.MainClass);

            // Game Arguments
            var gameDict = new Dictionary<string, string>()
            {
                { "auth_player_name", LaunchOption.Session.Username },
                { "version_name", LaunchOption.StartVersion.Id },
                { "game_directory", handleEmpty(MinecraftPath.BasePath) },
                { "assets_root", handleEmpty(MinecraftPath.Assets) },
                { "assets_index_name", version.AssetId },
                { "auth_uuid", LaunchOption.Session.UUID },
                { "auth_access_token", LaunchOption.Session.AccessToken },
                { "user_properties", "{}" },
                { "user_type", "Mojang" },
                { "game_assets", handleEmpty(MinecraftPath.AssetLegacy) },
                { "auth_session", LaunchOption.Session.AccessToken },
                { "version_type", useNotNull(LaunchOption.VersionType, version.TypeStr) }
            };

            if (version.GameArguments != null)
                args.AddRange(argumentInsert(version.GameArguments, gameDict));
            else
                args.AddRange(argumentInsert(version.MinecraftArguments.Split(' '), gameDict));

            // Options
            if (!string.IsNullOrEmpty(LaunchOption.ServerIp))
            {
                args.Add("--server " + LaunchOption.ServerIp);

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

        // replace ${key} to value
        // ex) "--accessToken ${access_token}" to "--accessToken " + dicts["access_token"]
        string[] argumentInsert(string[] arg, Dictionary<string, string> dicts)
        {
            var args = new List<string>(arg.Length);
            foreach (string item in arg)
            {
                var m = argBracket.Match(item);

                if (m.Success)
                {
                    var argKey = m.Groups[1].Value; // ${argKey}
                    var argValue = "";

                    if (dicts.TryGetValue(argKey, out argValue))
                        args.Add(replaceByPos(item, argValue, m.Index, m.Length)); // replace ${argKey} to dicts value
                    else
                        args.Add(item);
                }
                else
                    args.Add(handleEArg(item));
            }

            return args.ToArray();
        }

        string replaceByPos(string input, string replace, int startIndex, int length)
        {
            var sb = new StringBuilder(input);
            sb.Remove(startIndex, length);
            sb.Insert(startIndex, replace);
            return sb.ToString();
        }

        // if input1 is null, return input2
        string useNotNull(string input1, string input2)
        {
            if (string.IsNullOrEmpty(input1))
                return handleEmpty(input2);
            else
                return handleEmpty(input1);
        }

        // handle empty string in --key=value style argument
        // --key=va lue => --key="va lue"
        string handleEArg(string input)
        {
            if (input.Contains(" ") && input.Contains("="))
            {
                var s = input.Split('=');
                return s[0] + "=\"" + s[1] + "\"";
            }
            else
                return input;
        }

        string handleEmpty(string input)
        {
            if (input.Contains(" "))
                return "\"" + input + "\"";
            else
                return input;
        }
    }
}
