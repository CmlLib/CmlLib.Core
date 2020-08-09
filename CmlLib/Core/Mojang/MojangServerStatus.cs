using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json.Linq;

namespace CmlLib.Core.Mojang
{
    public class MojangServerStatus
    {
        public enum ServerStatusColor { Green, Yellow, Red, Unknown }

        public static MojangServerStatus GetStatus()
        {
            // request
            var response = "";
            using (var wc = new WebClient())
            {
                response = wc.DownloadString("https://status.mojang.com/check");
            }

            // to dict
            var jarr = JArray.Parse(response);
            var dict = new Dictionary<string, ServerStatusColor>();
            foreach (JObject item in jarr)
            {
                var property = item.First as JProperty;
                var color = ToStatusColor(property.Value.ToString());
                dict.Add(property.Name, (ServerStatusColor)color);
            }

            // to object
            return new MojangServerStatus()
            {
                Minecraft = getColorFromDict(dict, "minecraft.net"),
                Session = getColorFromDict(dict, "session.minecraft.net"),
                Account = getColorFromDict(dict, "account.mojang.com"),
                Auth = getColorFromDict(dict, "auth.mojang.com"),
                Skins = getColorFromDict(dict, "skins.minecraft.net"),
                AuthServer = getColorFromDict(dict, "authserver.mojang.com"),
                SessionServer = getColorFromDict(dict, "sessionserver.mojang.com"),
                Api = getColorFromDict(dict, "api.mojang.com"),
                Textures = getColorFromDict(dict, "textures.minecraft.net"),
                Mojang = getColorFromDict(dict, "mojang.com"),
            };
        }

        private static ServerStatusColor getColorFromDict(Dictionary<string, ServerStatusColor> dict, string key)
        {
            ServerStatusColor color;
            var result = dict.TryGetValue(key, out color);
            if (result)
                return color;
            else
                return ServerStatusColor.Unknown;
        }

        private static ServerStatusColor ToStatusColor(string str)
        {
            switch (str)
            {
                case "green":
                    return ServerStatusColor.Green;
                case "yellow":
                    return ServerStatusColor.Yellow;
                case "red":
                    return ServerStatusColor.Red;
                default:
                    return ServerStatusColor.Unknown;
            }
        }

        public ServerStatusColor Minecraft { get; private set; }
        public ServerStatusColor Session { get; private set; }
        public ServerStatusColor Account { get; private set; }
        public ServerStatusColor Auth { get; private set; }
        public ServerStatusColor Skins { get; private set; }
        public ServerStatusColor AuthServer { get; private set; }
        public ServerStatusColor SessionServer { get; private set; }
        public ServerStatusColor Api { get; private set; }
        public ServerStatusColor Textures { get; private set; }
        public ServerStatusColor Mojang { get; private set; }
    }
}
