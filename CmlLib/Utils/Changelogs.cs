using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace CmlLib.Utils
{
    public class Changelogs
    {
        private static Dictionary<string, string> changelogUrls = new Dictionary<string, string>()
        {
            { "1.13",   "https://feedback.minecraft.net/hc/en-us/articles/360007323492-Minecraft-Java-Edition-1-13-Update-Aquatic-" },
            { "1.14.2", "https://feedback.minecraft.net/hc/en-us/articles/360028919851-Minecraft-Java-Edition-1-14-2" },
            { "1.14.3", "https://feedback.minecraft.net/hc/en-us/articles/360030771451-Minecraft-Java-Edition-1-14-3" },
            { "1.14.4", "https://feedback.minecraft.net/hc/en-us/articles/360030780172-Minecraft-Java-Edition-1-14-4" },
            { "1.15.1", "https://feedback.minecraft.net/hc/en-us/articles/360038054332-Minecraft-Java-Edition-1-15-1" },
            { "1.15.2", "https://feedback.minecraft.net/hc/en-us/articles/360038800232-Minecraft-Java-Edition-1-15-2" },
            { "1.16",   "https://feedback.minecraft.net/hc/en-us/articles/360044911972-Minecraft-Java-Edition-Nether-Release" },
            { "1.16.5", "https://feedback.minecraft.net/hc/en-us/articles/360055096392-Minecraft-Java-Edition-1-16-5" }
        };

        public static string[] GetAvailableVersions()
        {
            return changelogUrls.Keys.ToArray();
        }

        public static string GetChangelogUrl(string version)
        {
            var url = "";
            if (changelogUrls.TryGetValue(version, out url))
                return url;
            else
                return null;
        }

        static Regex articleRegex = new Regex("<article class=\\\"article\\\">(.*)<\\/article>", RegexOptions.Singleline);

        public static string GetChangelogHtml(string version)
        {
            var url = GetChangelogUrl(version);
            if (string.IsNullOrEmpty(url))
                return "";

            var html = "";
            using (var wc = new WebClient())
            {
                html = Encoding.UTF8.GetString(wc.DownloadData(url));
            }

            //System.IO.File.WriteAllText("test.txt", html);

            var regResult = articleRegex.Match(html);
            if (!regResult.Success)
                return "";

            return regResult.Value;
        }
    }
}
