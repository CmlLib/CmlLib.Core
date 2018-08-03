using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmlServerLauncherSample
{
    public static class Info
    {
        // 런처 이름
        public static string LauncherName = "Sample_Server_Launcher";

        // 실행할 버전 목록 (여러개 설정 가능)
        public static string[] LaunchVersions =
        {
            "1.12.2",
            "1.12.2-forge1.12.2-14.23.4.2739",
            "1.7.10"
        };

        // 패치할 파일의 버전 (패치할게 없으면 비워두세요)
        public static string PatchVer = "https://www.dropbox.com/s/1uv1vaus0ixj2x8/ver.txt?dl=1";

        // 패치할 zip 파일의 URL (패치할게 없으면 비워두세요)
        public static string PatchURL = "https://www.dropbox.com/s/sl9zzkail5ric9j/.minecraft.zip?dl=1";

        // 마인크래프트 파일들을 저장하고 실행할 경로
        public static string MinecraftPath = 
            Environment.GetEnvironmentVariable("APPDATA") + @"\.minecraft";

        // 셋팅 파일이 저장되는 경로
        public static string SettingPath = MinecraftPath + @"\sampleSetting.json";

        // 아래 설정들은 선택 사항으로 굳이 수정할 필요는 없음

        public static int ScreenWidth = 0; // 게임화면의 크기 지정
        public static int ScreenHeight = 0; // 0은 기본값

        public static string ServerIp = ""; // 게임 시작 후 접속할 서버의 주소


    }
}
