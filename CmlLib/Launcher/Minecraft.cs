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
        /// Set Minecraft Path
        /// </summary>
        /// <param name="p">path</param>
        /// <param name="useCustomAssets">if useCustomAssets is false, it will set aseets dir to %appdata%\.minecraft\assets</param>
        public static void Initialize(string p,bool useCustomAssets = false)
        {
            path = p;
            _Path = p;

            Library = path + "\\libraries\\";
            Versions = path + "\\versions\\";
            Resource = path + "\\resources\\";

            _Library = path + "\\libraries";
            _Versions = path + "\\versions";
            _Resource = path + "\\resources";

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
