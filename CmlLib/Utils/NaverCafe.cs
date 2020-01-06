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

        public static NaverCafePost[] GetNotices(string clubid)
        {
            string html = "";
            using (var wc = new WebClient())
            {
                var data = wc.DownloadData("https://m.cafe.naver.com/NoticeList.nhn?search.clubid=" + clubid);
                html = Encoding.UTF8.GetString(data);
            }

            var boardlist = spl(html, "list_area notice_board", "</ul>");

            var resultlist = new List<NaverCafePost>(boardlist.Length);
            foreach (var item in Regex.Split(boardlist, "</li>"))
            {
                try
                {
                    if (!item.Contains("href"))
                        continue;

                    var url = cafeurl + spl(item, "href=\"", "\"");
                    var tit = spl(item, "class=\"tit\">", "</").Trim();

                    resultlist.Add(new NaverCafePost(tit, url));
                }
                catch { }
            }

            return resultlist.ToArray();
        }

        public static NaverCafePost[] GetPosts(string clubid, string menuid)
        {
            string requrl = $"https://m.cafe.naver.com/ArticleList.nhn?search.clubid={clubid}&search.menuid={menuid}";

            string html = "";
            using (var wc = new WebClient())
            {
                var data = wc.DownloadData(requrl);
                html = Encoding.UTF8.GetString(data);
            }

            var boardlist = spl(html, "<ul class=\"list_area\">", "</ul>");

            var resultlist = new List<NaverCafePost>(boardlist.Length);
            foreach (var item in Regex.Split(boardlist, "</li>"))
            {
                try
                {
                    if (!item.Contains("board_box"))
                        continue;

                    var url = cafeurl + spl(item, "href=\"", "\"");
                    var tit = spl(item, "class=\"tit\">", "</").Trim();

                    resultlist.Add(new NaverCafePost(tit, url));
                }
                catch { }
            }

            return resultlist.ToArray();
        }

        static string spl(string o, string f, string b)
        {
            return Regex.Split(Regex.Split(o, f)[1], b)[0];
        }
    }
}
