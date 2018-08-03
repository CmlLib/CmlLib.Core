using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CmlLib.Launcher
{
    public class MProfile
    {
        // 프로파일 파싱
        internal MProfile(bool isweb, string json)
        {
            var job = JObject.Parse(json);

            Id = job["id"]?.ToString();

            var assetindex = (JObject)job["assetIndex"];
            if (assetindex != null)
            {
                AssetId = n(assetindex["id"]?.ToString());
                AssetUrl = n(assetindex["url"]?.ToString());
            }

            ClientDownloadUrl = n(job["downloads"]?["client"]?["url"]?.ToString());
            Libraries = MLibrary.ParseJson((JArray)job["libraries"]);
            MainClass = n(job["mainClass"]?.ToString());

            var ma = job["minecraftArguments"]?.ToString();
            if (ma != null)
                MinecraftArguments = ma;
            var ag = job["arguments"]?.ToString();
            if (ag != null)
                MinecraftArguments = ag;

            ReleaseTime = job["releaseTime"]?.ToString();

            var ype = job["type"]?.ToString();
            switch (ype)
            {
                case "release":
                    Type = MProfileType.Release;
                    break;
                case "snapshot":
                    Type = MProfileType.Snapshot;
                    break;
                case "old_alpha":
                    Type = MProfileType.OldAlpha;
                    break;
                default:
                    Type = MProfileType.Unknown;
                    break;
            }

            if (job["jar"] != null)
            {
                IsForge = true;
                InnerJarId = job["jar"].ToString();
            }
        }
        string n(string t)
        {
            return (t == null) ? "" : t;
        }

        public void ChangeAssets(string Id, string Url)
        {
            AssetId = Id; AssetUrl = Url;
        }

        /// <summary>
        /// true 이면 모장 서버에 있는 프로파일, false 이면 로컬에 있는 프로파일
        /// </summary>
        public bool IsWeb { get; private set; }
        /// <summary>
        /// true 이면 포지 프로파일
        /// </summary>
        public bool IsForge { get; private set; } = false;

        /// <summary>
        /// 프로파일의 id
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// 인덱스 파일의 ID
        /// </summary>
        public string AssetId { get; private set; }
        /// <summary>
        /// 인덱스 파일의 경로
        /// </summary>
        public string AssetUrl { get; private set; }

        /// <summary>
        /// 게임 다운로드 경로
        /// </summary>
        public string ClientDownloadUrl { get; private set; }
        /// <summary>
        /// 라이브러리 리스트
        /// </summary>
        public List<MLibrary> Libraries { get; private set; }
        /// <summary>
        /// 메인 클래스
        /// </summary>
        public string MainClass { get; private set; }
        /// <summary>
        /// 게임 인수
        /// </summary>
        public string MinecraftArguments { get; private set; } = ""; 
        /// <summary>
        /// 프로파일의 생성된 날짜
        /// </summary>
        public string ReleaseTime { get; private set; }
        /// <summary>
        /// 프로파일의 종류
        /// </summary>
        public MProfileType Type { get; private set; }

        /// <summary>
        /// 포지 프로파일일때 베이스가 되는 프로파일의 ID
        /// </summary>
        public string InnerJarId { get; private set; }
        /// <summary>
        /// 네이티브 라이브러리가 저장되 있는 경로
        /// </summary>
        public string NativePath { get; set; }
    }
}
