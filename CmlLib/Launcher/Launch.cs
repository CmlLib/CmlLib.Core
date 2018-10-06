using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using Ionic.Zip;

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
            CleanNatives();
            CreateNatives();
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
            java_arg += " -Djava.library.path=\"" + StartProfile.NativePath + "\" -cp ";

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
                Session = MSession.GetOfflineSession("test");

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
                { "${version_type}", "release" }
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

        private void CreateNatives()
        {
            var path = ExtractNatives(StartProfile);
            StartProfile.NativePath = path;

            if (BaseProfile != null)
                ExtractNatives(BaseProfile, path);
        }

        /// <summary>
        /// 네이티브 라이브러리들의 압축을 해제해 랜덤 폴더에 저장합니다.
        /// </summary>
        private string ExtractNatives(MProfile profile)
        {
            var ran = new Random();
            int random = ran.Next(10000, 99999); //랜덤숫자 생성
            string path = Minecraft.Versions + profile.Id + "\\natives-" + random.ToString(); //랜덤 숫자를 만들어 경로생성
            ExtractNatives(profile, path);
            return path;
        }

        /// <summary>
        /// 네이티브 라이브러리들을 설정한 경로에 압축을 해제해 저장합니다.
        /// </summary>
        /// <param name="_path">압축 풀 폴더의 경로</param>
        private void ExtractNatives(MProfile profile, string path)
        {
            Directory.CreateDirectory(path); //폴더생성

            foreach (var item in profile.Libraries) //네이티브 라이브러리 리스트를 foreach 로 하나씩 돌림
            {
                try
                {
                    if (item.IsNative)
                    {
                        Console.WriteLine("native : {0} to {1}", item.Path,path);
                        using (var zip = ZipFile.Read(item.Path))
                        {
                            zip.ExtractAll(path, ExtractExistingFileAction.OverwriteSilently);
                        }
                    }
                }
                catch { }
            }

            profile.NativePath = path;
        }

        /// <summary>
        /// 저장된 네이티브 라이브러리들을 모두 제거합니다.
        /// </summary>
        public void CleanNatives()
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(Minecraft.Versions + StartProfile.Id);
                foreach (var item in di.GetDirectories("native*")) //native 라는 이름이 포함된 폴더를 모두 가져옴
                {
                    DeleteDirectory(item.FullName);
                }
            }
            catch { }
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
