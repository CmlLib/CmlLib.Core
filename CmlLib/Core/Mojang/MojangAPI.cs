using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace CmlLib.Core.Mojang
{
    // use MojangAPI library!
    // https://github.com/CmlLib/MojangAPI

    [Obsolete("use MojangAPI library: https://github.com/CmlLib/MojangAPI")]
    public static class MojangAPI
    {
        private static string readRes(WebResponse res)
        {
            using var resStream = res.GetResponseStream();
            if (resStream == null)
                return "";
            using var sr = new StreamReader(resStream);
            return sr.ReadToEnd();
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

            return new UserProfile
            {
                UUID = job["id"]?.ToString(),
                Name = job["name"]?.ToString(),
            };
        }
    }
}
