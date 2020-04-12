using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace CmlLib.Core
{
    public partial class MProfileInfo
    {
        /// <summary>
        /// Get All MProfileInfo from mojang server and local
        /// </summary>
        public static MProfileInfo[] GetProfiles(Minecraft mc)
        {
            var list = new HashSet<MProfileInfo>(GetProfilesFromLocal(mc));
            foreach (var item in GetProfilesFromWeb())
            {
                bool isexist = false;
                foreach (var local in list)
                {
                    if (local.Name == item.Name)
                    {
                        isexist = true;
                        break;
                    }
                }

                if (!isexist)
                    list.Add(item);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Get All MProfileInfo from local
        /// </summary>
        public static MProfileInfo[] GetProfilesFromLocal(Minecraft mc)
        {
            var dirs = new DirectoryInfo(mc.Versions).GetDirectories();
            var arr = new List<MProfileInfo>(dirs.Length);

            for (int i = 0; i < dirs.Length; i++)
            {
                var dir = dirs[i];
                var filepath = System.IO.Path.Combine(dir.FullName, dir.Name + ".json");
                if (File.Exists(filepath))
                {
                    var info = new MProfileInfo();
                    info.IsWeb = false;
                    info.Name = dir.Name;
                    info.Path = filepath;
                    arr.Add(info);
                }
            }

            return arr.ToArray();
        }

        /// <summary>
        /// Get All MProfileInfo from mojang server
        /// </summary>
        public static MProfileInfo[] GetProfilesFromWeb()
        {
            JArray jarr;
            using (WebClient wc = new WebClient())
            {
                var jobj = JObject.Parse(wc.DownloadString(MojangServer.Profile));
                jarr = JArray.Parse(jobj["versions"].ToString());
            }

            var arr = new MProfileInfo[jarr.Count];
            for (int i = 0; i < jarr.Count; i++)
            {
                var obj = jarr[i].ToObject<MProfileInfo>();
                obj.IsWeb = true;
                arr[i] = obj;
            }
            return arr;
        }
    }
}
