using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace CmlLib.Launcher
{

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

    }
}
