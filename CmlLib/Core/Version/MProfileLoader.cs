using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace CmlLib.Core
{
    public class MProfileLoader
    {
        /// <summary>
        /// Get All MProfileInfo from mojang server and local
        /// </summary>
        public static MProfileMetadata[] GetProfileMetadatas(Minecraft mc)
        {
            var list = new List<MProfileMetadata>(GetProfileMetadatasFromLocal(mc));
            foreach (var item in GetProfileMetadatasFromWeb())
            {
                if (!list.Contains(item))
                    list.Add(item);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Get All MProfileInfo from local
        /// </summary>
        public static MProfileMetadata[] GetProfileMetadatasFromLocal(Minecraft mc)
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

            return arr.ToArray();
        }

        /// <summary>
        /// Get All MProfileInfo from mojang server
        /// </summary>
        public static MProfileMetadata[] GetProfileMetadatasFromWeb()
        {
            JArray jarr;
            using (WebClient wc = new WebClient())
            {
                var jobj = JObject.Parse(wc.DownloadString(MojangServer.Profile));
                jarr = JArray.Parse(jobj["versions"].ToString());
            }

            var arr = new MProfileMetadata[jarr.Count];
            for (int i = 0; i < jarr.Count; i++)
            {
                var obj = jarr[i].ToObject<MProfileMetadata>();
                obj.IsWeb = true;
                obj.MType = MProfileTypeConverter.FromString(obj.Type);
                arr[i] = obj;
            }
            return arr;
        }
    }
}
