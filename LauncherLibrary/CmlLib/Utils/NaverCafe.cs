using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace CmlLib.Utils
{
    class NaverCafe
    {
        static string cafeurl = "http://cafe.naver.com";

        public static List<KeyValuePair<string , string>> GetNotices(string clubid)
        {
            string html = "";
            using (var wc = new WebClient())
            {
                var data = wc.DownloadData("https://m.cafe.naver.com/NoticeList.nhn?search.clubid=" + clubid);
                html = Encoding.UTF8.GetString(data);
            }

            var boardlist = spl(html, "list_area notice_board", "</ul>");

            var resultlist = new List<KeyValuePair<string, string>>();
            foreach (var item in Regex.Split(boardlist, "</li>"))
            {
                try
                {
                    if (!item.Contains("href"))
                        continue;

                    var url = cafeurl + spl(item, "href=\"", "\"");
                    var tit = cleaner(spl(item, "class=\"tit\">", "</"));

                    resultlist.Add(new KeyValuePair<string, string>(tit, url));
                }
                catch { }
            }

            return resultlist;
        }

        public static List<KeyValuePair<string, string>> GetPosts(string clubid, string menuid)
        {
            string requrl = $"https://m.cafe.naver.com/ArticleList.nhn?search.clubid={clubid}&search.menuid={menuid}";

            string html = "";
            using (var wc = new WebClient())
            {
                var data = wc.DownloadData(requrl);
                html = Encoding.UTF8.GetString(data);
            }

            var boardlist = spl(html, "<ul class=\"list_area\">", "</ul>");

            var resultlist = new List<KeyValuePair<string, string>>();
            foreach (var item in Regex.Split(boardlist, "</li>"))
            {
                try
                {
                    if (!item.Contains("board_box"))
                        continue;

                    var url = cafeurl + spl(item, "href=\"", "\"");
                    var tit = cleaner(spl(item, "class=\"tit\">", "</"));

                    resultlist.Add(new KeyValuePair<string, string>(tit, url));
                }
                catch { }
            }

            return resultlist;
        }

        static string spl(string o, string f, string b)
        {
            return Regex.Split(Regex.Split(o, f)[1], b)[0];
        }

        static string cleaner(string str)
        {
            var sb = new StringBuilder();

            var line = str.Split('\n');
            foreach (var item in line)
            {
                if (item.Replace(" ", "") != "")
                {
                    int chari = -1;
                    for (int i = 0; i < item.Length; i++)
                    {
                        if (item[i] != ' ')
                        {
                            chari = i;
                            break;
                        }
                    }

                    sb.Append(item.Remove(0, chari));
                    sb.Append(" ");
                }
            }

            return sb.ToString();
        }
    }
}
