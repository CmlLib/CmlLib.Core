using Ionic.Zip;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CmlLib.Launcher
{
    /// <summary>
    /// 게임 실행에 필요한 파라미터 생성, 게임 실행을 합니다.
    /// </summary>
    public class MLaunch
    {
        /// <summary>
        /// 기본 자바 파라미터
        /// </summary>
        public static string DefaultJavaParameter =
                "-XX:+UnlockExperimentalVMOptions " +
                "-XX:+UseG1GC " +
                "-XX:G1NewSizePercent=20 " +
                "-XX:G1ReservePercent=20 " +
                "-XX:MaxGCPauseMillis=50 " +
                "-XX:G1HeapRegionSize=16M";
        public static string SupportLaunchVersion = "1.3";

        public MLaunch(MLaunchOption option)
        {
            option.CheckValid();
            LaunchOption = option;
        }

        public MLaunchOption LaunchOption { get; private set; }

        /// <summary>
        /// 설정한 프로퍼티로 인수를 만들고 게임을 실행합니다.
        /// </summary>
        /// <param name="isdebug">true 로 설정하면 디버그 모드가 됩니다.</param>
        public void Start(bool isdebug = false)
        {
            GetProcess(isdebug).Start();
        }

        /// <summary>
        /// 설정한 프로퍼티로 인수를 만들어 Process 객체를 만들고 반환합니다.
        /// </summary>
        /// <param name="isdebug">true 로 설정하면 디버그 모드가 됩니다.</param>
        /// <returns>만들어진 Process 객체</returns>
        public Process GetProcess(bool isdebug = false)
        {
            var native = new MNative(LaunchOption);
            native.CleanNatives();
            native.CreateNatives();

            string arg = makeArg();
            Process mc = new Process();
            mc.StartInfo.FileName = LaunchOption.JavaPath;
            mc.StartInfo.Arguments = arg;
            mc.StartInfo.WorkingDirectory = Minecraft.path;

            if (isdebug)
                File.WriteAllText("mc_arg.txt", mc.StartInfo.Arguments);

            return mc;
        }

        string makeArg()
        {
            var profile = LaunchOption.StartProfile;
            if (LaunchOption.BaseProfile != null)
                profile = LaunchOption.BaseProfile;

            var sb = new StringBuilder();

            ///// JAVA ARG /////

            if (LaunchOption.CustomJavaParameter == "")
                sb.Append(DefaultJavaParameter);
            else
                sb.Append(LaunchOption.CustomJavaParameter);

            sb.Append(" -Xmx" + LaunchOption.MaximumRamMb + "m");
            sb.Append(" -Djava.library.path=\"" + LaunchOption.StartProfile.NativePath + "\" -cp ");

            List<MLibrary> libList = new List<MLibrary>();
            if (LaunchOption.BaseProfile != null)
                libList.AddRange(LaunchOption.StartProfile.Libraries);
            libList.AddRange(profile.Libraries);

            foreach (var item in libList)
            {
                if (!item.IsNative)
                    sb.Append("\"" + item.Path.Replace("/", "\\") + "\";");
            }

            ///// JAVA ARG END /////

            string mcjarid = profile.Id;

            sb.Append("\"" + Minecraft.Versions + mcjarid + "\\" + mcjarid + ".jar\" ");
            sb.Append(LaunchOption.StartProfile.MainClass + "  ");

            ///// GAME ARG ///// 

            Dictionary<string, string> argDicts = new Dictionary<string, string>()
            {
                { "${auth_player_name}", LaunchOption.Session.Username },
                { "${version_name}", LaunchOption.StartProfile.Id },
                { "${game_directory}", "\"" + Minecraft._Path + "\"" },
                { "${assets_root}", "\"" + Minecraft._Assets + "\"" },
                { "${assets_index_name}", profile.AssetId },
                { "${auth_uuid}", LaunchOption.Session.UUID },
                { "${auth_access_token}", LaunchOption.Session.AccessToken },
                { "${user_properties}", "{}" },
                { "${user_type}", "Mojang" },
                { "${game_assets}", "\"" + Minecraft.AssetLegacy },
                { "${auth_session}", LaunchOption.Session.AccessToken }
            };

            if (LaunchOption.LauncherName == "")
                argDicts.Add("${version_type}", "release");
            else
                argDicts.Add("${version_type}", LaunchOption.LauncherName);

            if (LaunchOption.StartProfile.Arguments != "") // Arguments Json 을 사용하는 1.3 과 그 이후의 버전
            {
                var jarr = (JArray)JObject.Parse(LaunchOption.StartProfile.Arguments)["game"];
                foreach (var item in jarr)
                {
                    if (!(item is JObject))
                    {
                        var str = item.ToString();

                        if (str[0] != '$')
                            sb.Append(str);

                        foreach (var arg in argDicts)
                        {
                            if (arg.Key == str)
                            {
                                sb.Append(arg.Value);
                                break;
                            }
                        }

                        sb.Append(" ");
                    }

                }
            }
            else // MinecraftArguments 를 사용하는 1.3 이전의 버전
            {
                sb.Append(LaunchOption.StartProfile.MinecraftArguments);
                foreach (var item in argDicts)
                {
                    sb.Replace(item.Key, item.Value);
                }
            }

            // 추가 옵션들 설정

            if (LaunchOption.ServerIp != "")
                sb.Append(" --server " + LaunchOption.ServerIp);

            if (LaunchOption.ScreenWidth > 0 && LaunchOption.ScreenHeight > 0)
            {
                sb.Append(" --width ");
                sb.Append(LaunchOption.ScreenWidth);
                sb.Append(" --height ");
                sb.Append(LaunchOption.ScreenHeight);
            }

            return sb.ToString();
        }

        private void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, true);
        }
    }
}
