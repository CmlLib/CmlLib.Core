using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmlLib.Launcher
{
    public class Minecraft
    {
        public static string mPath = Environment.GetEnvironmentVariable("appdata") + "\\.minecraft";

        public static string path = "";
        public static string Library;
        public static string Versions;
        public static string Resource;
        public static string Index;
        public static string Assets;

        public static string _Path;
        public static string _Library;
        public static string _Versions;
        public static string _Resource;
        public static string _Index;
        public static string _Assets;

        /// <summary>
        /// 설정한 경로를 게임 실행, 다운로드 경로로 설정합니다.
        /// </summary>
        /// <param name="p">경로</param>
        /// <param name="useCustomAssets">true 로 설정하면 다운로드 시간 단축을 위한 경로 통합을 사용하지 않게 할 수 있습니다.</param>
        public static void init(string p,bool useCustomAssets = false)
        {
            path = p;
            _Path = p;

            Library = path + "\\libraries\\";
            Versions = path + "\\versions\\";
            Resource = path + "\\resource\\";

            _Library = path + "\\libraries";
            _Versions = path + "\\versions";
            _Resource = path + "\\resource";

            if (useCustomAssets)
            {
                Index = path + "\\assets\\indexes\\";
                Assets = path + "\\assets\\";
                _Index = path + "\\assets\\indexes";
                _Assets = path + "\\assets";
            }
            else
            {
                Index = mPath + "\\assets\\indexes\\";
                Assets = mPath + "\\assets\\";
                _Index = mPath + "\\assets\\indexes";
                _Assets = mPath + "\\assets";
            }

            Directory.CreateDirectory(path);
            Directory.CreateDirectory(Library);
            Directory.CreateDirectory(Versions);
            Directory.CreateDirectory(Resource);
            Directory.CreateDirectory(Index);
            Directory.CreateDirectory(Assets);
            Directory.CreateDirectory(Assets + "virtual\\legacy");
            Directory.CreateDirectory(Assets + "objects");

            path += "\\";
        }
    }
}
