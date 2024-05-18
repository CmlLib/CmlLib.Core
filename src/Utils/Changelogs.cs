using System.Text.Json;
using System.Text.RegularExpressions;
using CmlLib.Core.Internals;

namespace CmlLib.Core.Utils;

public class Changelogs
{
    private static readonly string PatchNotesUrl = "https://launchercontent.mojang.com/javaPatchNotes.json";

    // The urls below will only be retrieved if the version is not found in 'PatchNotesUrl'
    // Versions that exist in 'PatchNotesUrl' do not need to be added the list below
    private static readonly Dictionary<string, string> AltUrls = new Dictionary<string, string>
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

    public static async Task<Changelogs> GetChangelogs(HttpClient client)
    {
        var response = await client.GetStreamAsync(PatchNotesUrl)
            .ConfigureAwait(false);
        var jsonDocument = await JsonDocument.ParseAsync(response).ConfigureAwait(false);
        var root = jsonDocument.RootElement;

        var versionDict = new Dictionary<string, string?>();
        var array = root.GetPropertyOrNull("entries")?.EnumerateArray();
        if (array != null)
        {
            foreach (var item in array)
            {
                var version = item.GetPropertyValue("version");
                if (string.IsNullOrEmpty(version))
                    continue;

                var body = item.GetPropertyValue("body");
                versionDict[version] = body;
            }
        }

        return new Changelogs(versionDict, client);
    }

    private Changelogs(Dictionary<string, string?> versions, HttpClient client)
    {
        this.httpClient = client;
        this.versions = versions;
    }

    private readonly HttpClient httpClient;
    private readonly Dictionary<string, string?> versions;

    public string[] GetAvailableVersions()
    {
        var availableVersions = new HashSet<string>();

        foreach (var item in versions.Keys)
            availableVersions.Add(item);

        foreach (var item in AltUrls)
            availableVersions.Add(item.Key);

        return availableVersions.ToArray();
    }

    public async Task<string?> GetChangelogHtml(string version)
    {
        if (versions.TryGetValue(version, out string? body))
            return body;
        if (AltUrls.TryGetValue(version, out string? url))
            return await GetChangelogFromUrl(url).ConfigureAwait(false);

        return null;
    }

    private static readonly Regex ArticleRegex = new Regex(
        "<article class=\\\"article\\\">(.*)<\\/article>", RegexOptions.Singleline);

    private async Task<string> GetChangelogFromUrl(string url)
    {
        var html = await httpClient.GetStringAsync(url).ConfigureAwait(false);

        var regResult = ArticleRegex.Match(html);
        if (!regResult.Success)
            return "";

        return regResult.Value;
    }
}
