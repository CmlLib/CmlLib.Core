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
        public static string DefaultPath = Environment.GetEnvironmentVariable("appdata") + "\\.minecraft";

        public static string path = "";
        public static string Library;
        public static string Versions;
        public static string Resource;
        public static string Index;
        public static string Assets;
        public static string AssetObject;
        public static string AssetLegacy;

        public static string _Path;
        public static string _Library;
        public static string _Versions;
        public static string _Resource;
        public static string _Index;
        public static string _Assets;
        public static string _AssetObject;
        public static string _AssetLegacy;

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

            var resPath = DefaultPath;
            if (useCustomAssets)
                resPath = path;

            Index = resPath + "\\assets\\indexes\\";
            Assets = resPath + "\\assets\\";
            AssetObject = Assets + "objects\\";
            AssetLegacy = Assets + "virtual\\legacy\\";

            _Index = resPath + "\\assets\\indexes";
            _Assets = resPath + "\\assets";
            _AssetObject = Assets + "objects";
            _AssetLegacy = Assets + "virtual\\legacy";

            Directory.CreateDirectory(Library);
            Directory.CreateDirectory(Versions);
            Directory.CreateDirectory(Index);
            Directory.CreateDirectory(Resource);
            Directory.CreateDirectory(AssetObject);
            Directory.CreateDirectory(AssetLegacy);

            path += "\\";
        }
    }
}
