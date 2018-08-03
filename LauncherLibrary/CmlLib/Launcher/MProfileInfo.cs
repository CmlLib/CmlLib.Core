using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace CmlLib.Launcher
{
    public enum MProfileType { OldAlpha, Snapshot, Release, Unknown }

    public partial class MProfileInfo
    {
        /// <summary>
        /// true 이면 모장 서버에 있는 프로파일, false 이면 로컬에 있는 프로파일
        /// </summary>
        public bool IsWeb = true;

        /// <summary>
        /// 프로파일의 이름
        /// </summary>
        [JsonProperty("id")]
        public string Name { get; set; }

        /// <summary>
        /// 프로파일의 종류
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// 프로파일이 생성된 날짜
        /// </summary>
        [JsonProperty("releaseTime")]
        public string ReleaseTime { get; set; }

        /// <summary>
        /// 모장 서버에 있는 프로파일의 URL 
        /// </summary>
        [JsonProperty("url")]
        public string Path { get; set; }

        /// <summary>
        /// 프로파일을 다운로드하고 파싱해 반환합니다.
        /// </summary>
        /// <returns>파싱된 프로파일</returns>
        public MProfile GetProfile()
        {
            string json;
            if (IsWeb)
            {
                using (var wc = new WebClient())
                {
                    json = wc.DownloadString(Path);
                    var path = Minecraft.Versions + Name;
                    Directory.CreateDirectory(path);
                    File.WriteAllText(path + "\\" + Name + ".json", json);
                }
            }
            else
            {
                json = File.ReadAllText(Path);
            }

            var p = new MProfile(IsWeb, json);
            return p;
        }
    }
}
