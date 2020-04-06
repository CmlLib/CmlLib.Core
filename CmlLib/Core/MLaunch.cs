using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CmlLib.Core
{

    public class MLaunch
    {
        public static string DefaultJavaParameter =
                "-XX:+UnlockExperimentalVMOptions " +
                "-XX:+UseG1GC " +
                "-XX:G1NewSizePercent=20 " +
                "-XX:G1ReservePercent=20 " +
                "-XX:MaxGCPauseMillis=50 " +
                "-XX:G1HeapRegionSize=16M";
        public static string SupportLaunchVersion = "1.15.1";

        private static Regex argBracket = new Regex(@"\$\{(.*?)}");

        public MLaunch(MLaunchOption option)
        {
            option.CheckValid();
            LaunchOption = option;
            this.Minecraft = option.StartProfile.Minecraft;
        }

        Minecraft Minecraft;
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
            var native = new MNative(LaunchOption);
            native.CleanNatives();
            native.CreateNatives();

            string arg = CreateArg();
            Process mc = new Process();
            mc.StartInfo.FileName = LaunchOption.JavaPath;
            mc.StartInfo.Arguments = arg;
            mc.StartInfo.WorkingDirectory = Minecraft.path;

            return mc;
        }

        public string CreateArg()
        {
            var profile = LaunchOption.StartProfile;

            var args = new List<string>();

            // common jvm args
            if (LaunchOption.CustomJavaParameter == "")
                args.Add(DefaultJavaParameter);
            else
                args.Add(LaunchOption.CustomJavaParameter);

            args.Add(" -Xmx" + LaunchOption.MaximumRamMb + "m");

            // specific jvm args
            var libArgs = new List<string>(profile.Libraries.Length);

            foreach (var item in profile.Libraries)
            {
                if (!item.IsNative)
                    libArgs.Add(handleEmpty(Path.GetFullPath(item.Path)));
            }

            libArgs.Add(handleEmpty(Path.Combine(Minecraft.Versions, profile.Jar, profile.Jar + ".jar")));

            var libs = string.Join(Path.PathSeparator.ToString(), libArgs);

            var jvmdict = new Dictionary<string, string>()
            {
                { "natives_directory", handleEmpty(profile.NativePath) },
                { "launcher_name", "minecraft-launcher" },
                { "launcher_version", "2" },
                { "classpath", libs }
            };

            if (profile.JvmArguments != null)
                args.AddRange(argumentInsert(profile.JvmArguments, jvmdict));
            else
            {
                args.Add("-Djava.library.path=" + handleEmpty(LaunchOption.StartProfile.NativePath));
                args.Add("-cp " + libs);
            }

            args.Add(profile.MainClass);

            // game args
            var gameDict = new Dictionary<string, string>()
            {
                { "auth_player_name", LaunchOption.Session.Username },
                { "version_name", LaunchOption.StartProfile.Id },
                { "game_directory", handleEmpty(Minecraft.path) },
                { "assets_root", handleEmpty(Minecraft.Assets) },
                { "assets_index_name", profile.AssetId },
                { "auth_uuid", LaunchOption.Session.UUID },
                { "auth_access_token", LaunchOption.Session.AccessToken },
                { "user_properties", "{}" },
                { "user_type", "Mojang" },
                { "game_assets", Minecraft.AssetLegacy },
                { "auth_session", LaunchOption.Session.AccessToken }
            };

            if (LaunchOption.LauncherName == "")
                gameDict.Add("version_type", profile.TypeStr);
            else
                gameDict.Add("version_type", LaunchOption.LauncherName);

            if (profile.GameArguments != null)
                args.AddRange(argumentInsert(profile.GameArguments, gameDict));
            else
                args.AddRange(argumentInsert(profile.MinecraftArguments.Split(' '), gameDict));

            // options
            if (LaunchOption.ServerIp != "")
                args.Add("--server " + LaunchOption.ServerIp);

            if (LaunchOption.ScreenWidth > 0 && LaunchOption.ScreenHeight > 0)
            {
                args.Add("--width " + LaunchOption.ScreenWidth);
                args.Add("--height " + LaunchOption.ScreenHeight);
            }

            return string.Join(" ", args);
        }

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

        bool isn(string v)
        {
            return (v == null || v == "");
        }

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
