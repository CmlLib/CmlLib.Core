using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace CmlLib.Core.Mojang
{
    // use MojangAPI library!
    // https://github.com/CmlLib/MojangAPI

    [Obsolete("use MojangAPI library: https://github.com/CmlLib/MojangAPI")]
    public static class MojangAPI
    {
        private static void writeReq(WebRequest req, string data)
        {
            using (var reqStream = req.GetRequestStream())
            using (var sw = new StreamWriter(reqStream))
            {
                sw.Write(data);
            }
        }

        private static string readRes(WebResponse res)
        {
            using (var resStream = res.GetResponseStream())
            using (var sr = new StreamReader(resStream))
            {
                return sr.ReadToEnd();
            }
        }

        // entitlements
        public static bool CheckGameOwnership(string bearerToken)
        {
            var url = "https://api.minecraftservices.com/entitlements/mcstore";
            var req = WebRequest.CreateHttp(url);
            req.Method = "GET";
            req.Headers["Authorization"] = "Bearer " + bearerToken;

            var res = req.GetResponse();
            var resBody = readRes(res);

            var job = JObject.Parse(resBody);
            var itemsCount = (job["items"] as JArray)?.Count ?? 0;
            return itemsCount != 0;
        }

        public static UserProfile GetProfileUsingToken(string bearerToken)
        {
            var url = "https://api.minecraftservices.com/minecraft/profile";
            var req = WebRequest.CreateHttp(url);
            req.Method = "GET";
            req.Headers["Authorization"] = "Bearer " + bearerToken;

            var res = req.GetResponse();
            var resBody = readRes(res);
            var job = JObject.Parse(resBody);

            var skinObj = job["skins"]?[0];

            return new UserProfile
            {
                UUID = job["id"]?.ToString(),
                Name = job["name"]?.ToString(),
                Skin = new Skin
                (
                    url: skinObj?["url"]?.ToString(),
                    type: skinObj?["alias"]?.ToString()
                )
            };
        }

        public static UserProfile GetProfileUsingUUID(string uuid)
        {
            var url = "https://sessionserver.mojang.com/session/minecraft/profile/" + uuid;
            var req = WebRequest.CreateHttp(url);
            req.Method = "GET";

            var res = req.GetResponse();
            var resBody = readRes(res);
            var job = JObject.Parse(resBody);

            JObject propObj = null;
            var propValue = job["properties"]?[0]?["value"];
            if (propValue != null)
            {
                var decoded = Convert.FromBase64String(propValue.ToString());
                propObj = JObject.Parse(Encoding.UTF8.GetString(decoded));
            }

            var skinObj = propObj["textures"]?["SKIN"];

            Skin skin;
            if (skinObj == null)
                skin = new Skin(null, Skin.GetDefaultSkinType(uuid));
            else
                skin = new Skin
                (
                    url: skinObj["url"]?.ToString(),
                    type: skinObj["metadata"]?["model"]?.ToString()
                );

            return new UserProfile
            {
                UUID = propObj["profileId"]?.ToString(),
                Name = propObj["profileName"]?.ToString(),
                Skin = skin
            };
        }
    }
}
