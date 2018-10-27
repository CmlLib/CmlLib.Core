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
            option.CheckVaild();
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
            CleanNatives();
            CreateNatives();
            return makeProcess(LaunchOption.JavaPath, Minecraft.path, isdebug);
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
                { "${game_assets}", "\"" + Minecraft.Assets + "virtual\\legacy\"" },
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

        private void CreateNatives()
        {
            var path = ExtractNatives(LaunchOption.StartProfile);
            LaunchOption.StartProfile.NativePath = path;

            if (LaunchOption.BaseProfile != null)
                ExtractNatives(LaunchOption.BaseProfile, path);
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
                DirectoryInfo di = new DirectoryInfo(Minecraft.Versions + LaunchOption.StartProfile.Id);
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
