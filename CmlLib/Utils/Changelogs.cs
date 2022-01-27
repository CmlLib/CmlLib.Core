using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CmlLib.Utils
{
    public class Changelogs
    {
        private static readonly Dictionary<string, string> changelogUrls = new Dictionary<string, string>
        {
            { "1.14.2", "https://feedback.minecraft.net/hc/en-us/articles/360028919851-Minecraft-Java-Edition-1-14-2" },
            { "1.14.3", "https://feedback.minecraft.net/hc/en-us/articles/360030771451-Minecraft-Java-Edition-1-14-3" },
            { "1.14.4", "https://feedback.minecraft.net/hc/en-us/articles/360030780172-Minecraft-Java-Edition-1-14-4" },
            { "1.15", "https://feedback.minecraft.net/hc/en-us/articles/360037384972-Minecraft-Java-Edition-Buzzy-Bees" },
            { "1.15.1", "https://feedback.minecraft.net/hc/en-us/articles/360038054332-Minecraft-Java-Edition-1-15-1" },
            { "1.15.2", "https://feedback.minecraft.net/hc/en-us/articles/360038800232-Minecraft-Java-Edition-1-15-2" },
            { "1.16", "https://feedback.minecraft.net/hc/en-us/articles/360044911972-Minecraft-Java-Edition-Nether-Release" },
            { "1.16.5", "https://feedback.minecraft.net/hc/en-us/articles/360055096392-Minecraft-Java-Edition-1-16-5" },
            { "1.17", "https://feedback.minecraft.net/hc/en-us/articles/4402626897165-Minecraft-Caves-Cliffs-Part-1-1-17-Java" },
            { "1.17.1", "https://feedback.minecraft.net/hc/en-us/articles/4404449719949-Minecraft-Java-Edition-1-17-1" },
            { "1.18", "https://feedback.minecraft.net/hc/en-us/articles/4415128577293-Minecraft-Java-Edition-1-18" },
            { "1.18.1", "https://feedback.minecraft.net/hc/en-us/articles/4416161161101-Minecraft-Java-Edition-1-18-1" },
        };
        
        public static async Task<Changelogs> GetChangelogs()
        {
            string response;
            using (var wc = new WebClient())
            {
                var url = "https://launchercontent.mojang.com/javaPatchNotes.json";
                var data = await wc.DownloadDataTaskAsync(url)
                    .ConfigureAwait(false);
                response = Encoding.UTF8.GetString(data);
            }

            var obj = JObject.Parse(response);
            var versionDict = new Dictionary<string, string?>();
            var array = obj["entries"] as JArray;
            if (array != null)
            {
                foreach (var item in array)
                {
                    var version = item["version"]?.ToString();
                    if (string.IsNullOrEmpty(version))
                        continue;
                    
                    var body = item["body"]?.ToString();
                    versionDict[version] = body;
                }
            }

            return new Changelogs(versionDict);
        }
        
        private Changelogs(Dictionary<string, string?> versions)
        {
            this.versions = versions;
        }

        private readonly Dictionary<string, string?> versions;

        public string[] GetAvailableVersions()
        {
            var list = new List<string>();
            list.AddRange(versions.Keys);

            foreach (var item in changelogUrls)
            {
                if (!versions.ContainsKey(item.Key))
                    list.Add(item.Key);
            }

            return list.ToArray();
        }

        public async Task<string?> GetChangelogHtml(string version)
        {
            if (versions.TryGetValue(version, out string? body))
                return body;
            if (changelogUrls.TryGetValue(version, out string? url))
                return await GetChangelogFromUrl(url).ConfigureAwait(false);

            return null;
        }

        private static readonly Regex articleRegex = new Regex(
            "<article class=\\\"article\\\">(.*)<\\/article>", RegexOptions.Singleline);

        private async Task<string> GetChangelogFromUrl(string url)
        {
            string html;
            using (var wc = new WebClient())
            {
                var data = await wc.DownloadDataTaskAsync(url).ConfigureAwait(false);
                html = Encoding.UTF8.GetString(data);
            }

            var regResult = articleRegex.Match(html);
            if (!regResult.Success)
                return "";

            return regResult.Value;
        }
    }
}
