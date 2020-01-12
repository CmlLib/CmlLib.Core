using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace CmlLib.Utils
{
    public class NaverCafePost
    {
        public NaverCafePost(string name, string url)
        {
            this.Title = name;
            this.Url = url;
        }
        
        public string Title { get; private set; }
        public string Url { get; private set; }
    }

    public class NaverCafe
    {
        static string cafeurl = "http://cafe.naver.com";

        public static NaverCafePost[] GetNotices(string clubid, string cafename)
        {
            string requrl = $"https://apis.naver.com/cafe-web/cafe2/ArticleList.json?search.clubid={clubid}&search.queryType=lastArticle&search.menuid=&search.page=1";

            string res = "";
            using (var wc = new WebClient())
            {
                var data = wc.DownloadData(requrl);
                res = Encoding.UTF8.GetString(data);
            }

            var job = JObject.Parse(res);
            if (job["message"]?["status"]?.ToString() != "200")
                return new NaverCafePost[0];

            var listtoken = job["message"]?["result"]?["articleList"];
            if (listtoken == null)
                return new NaverCafePost[0];

            var boardlist = (JArray)listtoken;
            var result = new List<NaverCafePost>(boardlist.Count);

            foreach (JObject item in boardlist)
            {
                var articleId = item["articleId"]?.ToString();
                var title = item["subject"]?.ToString();

                var url = $"{cafeurl}/{cafename}/{articleId}";
                result.Add(new NaverCafePost(title, url));
            }

            return result.ToArray();
        }

        public static NaverCafePost[] GetPosts(string clubid, string menuid, string cafename)
        {
            string requrl = $"https://apis.naver.com/cafe-web/cafe2/ArticleList.json?search.clubid={clubid}&search.queryType=lastArticle&search.menuid={menuid}&search.page=1";

            string res = "";
            using (var wc = new WebClient())
            {
                var data = wc.DownloadData(requrl);
                res = Encoding.UTF8.GetString(data);
            }

            var job = JObject.Parse(res);
            if (job["message"]?["status"]?.ToString() != "200")
                return new NaverCafePost[0];

            var listtoken = job["message"]?["result"]?["articleList"];
            if (listtoken == null)
                return new NaverCafePost[0];

            var boardlist = (JArray)listtoken;
            var result = new List<NaverCafePost>(boardlist.Count);

            foreach (JObject item in boardlist)
            {
                var articleId = item["articleId"]?.ToString();
                var title = item["subject"]?.ToString();

                var url = $"{cafeurl}/{cafename}/{articleId}";
                result.Add(new NaverCafePost(title, url));
            }

            return result.ToArray();
        }

        static string spl(string o, string f, string b)
        {
            return Regex.Split(Regex.Split(o, f)[1], b)[0];
        }
    }
}
