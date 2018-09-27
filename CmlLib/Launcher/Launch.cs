using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace CmlLib.Launcher
{
    /// <summary>
    /// 게임 실행에 필요한 파라미터 생성, 게임 실행을 합니다.
    /// </summary>
    public class MLaunch
    {
        static MLaunch()
        {
            DefaultJavaParameter = 
                "-XX:+UnlockExperimentalVMOptions " +
                "-XX:+UseG1GC " +
                "-XX:G1NewSizePercent=20 " +
                "-XX:G1ReservePercent=20 " +
                "-XX:MaxGCPauseMillis=50 " +
                "-XX:G1HeapRegionSize=16M";

        }

        /// <summary>
        /// 기본 자바 파라미터
        /// </summary>
        public static string DefaultJavaParameter;

        public static string SupportLaunchVersion = "1.3";

        public string JavaPath { get; set; } = "";
        public string XmxRam { get; set; }
        public MProfile StartProfile { get; set; }
        public MProfile BaseProfile { get; set; } = null;
        public MSession Session { get; set; }
        public string LauncherName { get; set; } = "";
        public string ServerIp { get; set; } = "";

        string javaParams = DefaultJavaParameter;
        public string CustomJavaParameter
        {
            get
                => javaParams;
            set
            {
                if (value == "") javaParams = DefaultJavaParameter;
                else javaParams = value;
            }
        }

        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }

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
            return makeProcess(JavaPath, Minecraft.path, isdebug);
        }

        // 인수를 만들고 Process 객체를 만듬
        Process makeProcess(string filename,string work,bool isdebug)
        {
            string arg = makeArg();
            Process mc = new Process();
            mc.StartInfo.FileName = filename;
            mc.StartInfo.Arguments = arg;
            mc.StartInfo.WorkingDirectory = work;

            if (isdebug)
                System.IO.File.WriteAllText("mc_arg.txt", mc.StartInfo.Arguments);

            return mc;
        }

        // 인수를 만듬
        string makeArg()
        {
            var argstring = new StringBuilder();
            var profile = StartProfile;
            if (BaseProfile != null)
                profile = BaseProfile;

            ///// JAVA ARG /////

            string java_arg = javaParams;
            java_arg += " -Djava.library.path=\"" + profile.NativePath + "\" -cp ";

            argstring.Append(java_arg);

            List<MLibrary> libList = new List<MLibrary>();
            if (BaseProfile != null)
                libList.AddRange(StartProfile.Libraries);
            libList.AddRange(profile.Libraries);

            foreach (var item in libList)
            {
                if (!item.IsNative)
                    argstring.Append("\"" + item.Path.Replace("/", "\\") + "\";");
            }

            ///// JAVA ARG END /////

            string mcjarid = profile.Id;

            argstring.Append("\"" + Minecraft.Versions + mcjarid + "\\" + mcjarid + ".jar\" ");
            argstring.Append(StartProfile.MainClass + "  ");

            ///// GAME ARG /////                 

            if (Session == null)
                Session = MSession.getDefault("test");

            Dictionary<string, string> argDicts = new Dictionary<string, string>()
            {
                { "${auth_player_name}", Session.Username },
                { "${version_name}", StartProfile.Id },
                { "${game_directory}", "\"" + Minecraft._Path + "\"" },
                { "${assets_root}", "\"" + Minecraft._Assets + "\"" },
                { "${assets_index_name}", profile.AssetId },
                { "${auth_uuid}", Session.UUID },
                { "${auth_access_token}", Session.AccessToken },
                { "${user_properties}", "{}" },
                { "${user_type}", "Mojang" },
                { "${game_assets}", "\"" + Minecraft.Assets + "vitual\\legacy\"" },
                { "${auth_session}", Session.AccessToken },
                { "${version_type}", "" }
            };

            StringBuilder sb = new StringBuilder();
            if (StartProfile.Arguments != "") // Arguments Json 을 사용하는 1.3 과 그 이후의 버전
            {
                var jarr = (JArray)JObject.Parse(StartProfile.Arguments)["game"];
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
                sb.Append(StartProfile.MinecraftArguments);
                foreach (var item in argDicts)
                {
                    sb.Replace(item.Key, item.Value);
                }
            }

            if (ServerIp != "")
                sb.Append(" --server " + ServerIp);
            if (LauncherName != "")
                sb.Replace("${version_type}", LauncherName.Replace(" ", "_"));
            else
                sb.Replace(" --versionType ${version_type}","");

            if (ScreenWidth != 0 && ScreenHeight != 0)
            {
                sb.Append(" --width ");
                sb.Append(ScreenWidth);
                sb.Append(" --height ");
                sb.Append(ScreenHeight);
            }

            argstring.Append(sb);
            return argstring.ToString();
        }


        [Obsolete("GetProcess 메서드를 이용하세요.")]
        public Process GetDebugProcess()
        {
            var mc = new Process();
            mc.StartInfo.FileName = "cmd.exe";
            mc.StartInfo.Arguments = "/c \"java.exe " + makeArg() + "\"";
            mc.StartInfo.WorkingDirectory = Minecraft.path;
            return mc;
        }

        [Obsolete("GetProcess 메서드를 이용하세요.")]
        public void DebugStart_java_exe(string javafolder)
        {
            makeProcess("java.exe", javafolder, true).Start();
        }
    }
}
