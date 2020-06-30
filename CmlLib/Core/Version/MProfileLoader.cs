using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace CmlLib.Core.Version
{
    public class MProfileLoader
    {
        /// <summary>
        /// Get All MProfileInfo from mojang server and local
        /// </summary>
        public static MProfileMetadataCollection GetProfileMetadatas(Minecraft mc)
        {
            var list = getFromLocal(mc);
            foreach (var item in getFromWeb())
            {
                if (!list.Contains(item))
                    list.Add(item);
            }
            return new MProfileMetadataCollection(mc, list.ToArray());
        }

        /// <summary>
        /// Get All MProfileInfo from local
        /// </summary>
        public static MProfileMetadataCollection GetProfileMetadatasFromLocal(Minecraft mc)
        {
            var list = getFromLocal(mc).ToArray();
            return new MProfileMetadataCollection(mc, list);
        }

        /// <summary>
        /// Get All MProfileInfo from mojang server
        /// </summary>
        public static MProfileMetadataCollection GetProfileMetadatasFromWeb(Minecraft mc)
        {
            var list = getFromWeb().ToArray();
            return new MProfileMetadataCollection(mc, list);
        }

        private static List<MProfileMetadata> getFromLocal(Minecraft mc)
        {
            var dirs = new DirectoryInfo(mc.Versions).GetDirectories();
            var arr = new List<MProfileMetadata>(dirs.Length);

            for (int i = 0; i < dirs.Length; i++)
            {
                var dir = dirs[i];
                var filepath = Path.Combine(dir.FullName, dir.Name + ".json");
                if (File.Exists(filepath))
                {
                    var info = new MProfileMetadata();
                    info.IsWeb = false;
                    info.Name = dir.Name;
                    info.Path = filepath;
                    info.Type = "local";
                    info.MType = MProfileType.Custom;
                    arr.Add(info);
                }
            }

            return arr;
        }

        private static List<MProfileMetadata> getFromWeb()
        {
            JArray jarr;
            using (WebClient wc = new WebClient())
            {
                var jobj = JObject.Parse(wc.DownloadString(MojangServer.Profile));
                jarr = JArray.Parse(jobj["versions"].ToString());
            }

            var arr = new List<MProfileMetadata>(jarr.Count);
            for (int i = 0; i < jarr.Count; i++)
            {
                var obj = jarr[i].ToObject<MProfileMetadata>();
                obj.IsWeb = true;
                obj.MType = MProfileTypeConverter.FromString(obj.Type);
                arr.Add(obj);
            }
            return arr;
        }
    }
}
