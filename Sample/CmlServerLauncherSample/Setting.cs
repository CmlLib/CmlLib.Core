using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace CmlServerLauncherSample
{
    public class Setting
    {
        private Setting() { } //싱글턴

        // 아래는 JSON 파일을 불러와 프로퍼티에 값을 넣고
        // 프로퍼티의 값을 파일에 저장하는 코드

        static Setting instance = null;
        public static Setting Json
        {
            get
            {
                if (instance == null)
                {
                    if (File.Exists(Info.SettingPath))
                    {
                        var file = File.ReadAllText(Info.SettingPath, Encoding.UTF8);
                        instance = JsonConvert.DeserializeObject<Setting>(file);
                    }
                    else
                        instance = new Setting();
                }
                return instance;
            }
        }

        public void Save()
        {
            var file = JsonConvert.SerializeObject(this);
            File.WriteAllText(Info.SettingPath, file, Encoding.UTF8);
        }

        // 설정들. [JsonProperty] 를 붙이면 자동으로 저장/불러와짐

        [JsonProperty]
        public string XmxRam = "1024";

        [JsonProperty]
        public string i_am_nothing = "only test";
    }
}
