﻿using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CmlLib.Utils;

public class Changelogs
{
    private static readonly Dictionary<string, string> changelogUrls = new()
    {
        { "1.14.2", "https://feedback.minecraft.net/hc/en-us/articles/360028919851-Minecraft-Java-Edition-1-14-2" },
        { "1.14.3", "https://feedback.minecraft.net/hc/en-us/articles/360030771451-Minecraft-Java-Edition-1-14-3" },
        { "1.14.4", "https://feedback.minecraft.net/hc/en-us/articles/360030780172-Minecraft-Java-Edition-1-14-4" }
    };

    private static readonly Regex articleRegex = new(
        "<article class=\\\"article\\\">(.*)<\\/article>", RegexOptions.Singleline);

    private readonly Dictionary<string, string?> versions;

    private Changelogs(Dictionary<string, string?> versions)
    {
        this.versions = versions;
    }

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
            foreach (var item in array)
            {
                var version = item["version"]?.ToString();
                if (string.IsNullOrEmpty(version))
                    continue;

                var body = item["body"]?.ToString();
                versionDict[version] = body;
            }

        return new Changelogs(versionDict);
    }

    public string[] GetAvailableVersions()
    {
        var list = new List<string>();
        list.AddRange(versions.Keys);

        foreach (var item in changelogUrls)
            if (!versions.ContainsKey(item.Key))
                list.Add(item.Key);

        return list.ToArray();
    }

    public async Task<string?> GetChangelogHtml(string version)
    {
        if (versions.TryGetValue(version, out var body))
            return body;
        if (changelogUrls.TryGetValue(version, out var url))
            return await GetChangelogFromUrl(url).ConfigureAwait(false);

        return null;
    }

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
