using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace CmlLib.Core.Version
{
    public class MVersionLoader
    {
        /// <summary>
        /// Get All MVersionInfo from mojang server and local
        /// </summary>
        public static MVersionCollection GetVersionMetadatas(MinecraftPath mc)
        {
            var list = getFromLocal(mc);
            foreach (var item in getFromWeb())
            {
                if (!list.Contains(item))
                    list.Add(item);
            }
            return new MVersionCollection(list.ToArray());
        }

        /// <summary>
        /// Get All MVersionInfo from local
        /// </summary>
        public static MVersionCollection GetVersionMetadatasFromLocal(MinecraftPath mc)
        {
            var list = getFromLocal(mc).ToArray();
            return new MVersionCollection(list);
        }

        /// <summary>
        /// Get All MVersionInfo from mojang server
        /// </summary>
        public static MVersionCollection GetVersionMetadatasFromWeb(MinecraftPath mc)
        {
            var list = getFromWeb().ToArray();
            return new MVersionCollection(list);
        }

        private static List<MVersionMetadata> getFromLocal(MinecraftPath mc)
        {
            var dirs = new DirectoryInfo(mc.Versions).GetDirectories();
            var arr = new List<MVersionMetadata>(dirs.Length);

            for (int i = 0; i < dirs.Length; i++)
            {
                var dir = dirs[i];
                var filepath = Path.Combine(dir.FullName, dir.Name + ".json");
                if (File.Exists(filepath))
                {
                    var info = new MVersionMetadata();
                    info.IsLocalVersion = true;
                    info.Name = dir.Name;
                    info.Path = filepath;
                    info.Type = "local";
                    info.MType = MVersionType.Custom;
                    arr.Add(info);
                }
            }

            return arr;
        }

        private static List<MVersionMetadata> getFromWeb()
        {
            JArray jarr;
            using (WebClient wc = new WebClient())
            {
                var jobj = JObject.Parse(wc.DownloadString(MojangServer.Version));
                jarr = JArray.Parse(jobj["versions"].ToString());
            }

            var arr = new List<MVersionMetadata>(jarr.Count);
            for (int i = 0; i < jarr.Count; i++)
            {
                var obj = jarr[i].ToObject<MVersionMetadata>();
                obj.IsLocalVersion = false;
                obj.MType = MVersionTypeConverter.FromString(obj.Type);
                arr.Add(obj);
            }
            return arr;
        }
    }
}
