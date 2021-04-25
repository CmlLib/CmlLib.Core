using CmlLib.Core.Auth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace CmlLib.Core.MojangLauncher
{
    public class MojangAccount
    {
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }

        [JsonProperty("accessTokenExpiresAt")]
        public DateTime AccessTokenExpiresAt { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("eligibleForMigration")]
        public bool EligibleForMigration { get; set; }

        [JsonProperty("hasMultipleProfiles")]
        public bool HasMultipleProfiles { get; set; }

        [JsonProperty("legacy")]
        public bool Legacy { get; set; }

        [JsonProperty("localId")]
        public string LocalId { get; set; }

        [JsonProperty("minecraftProfile")]
        public JObject MinecraftProfile { get; set; }

        public string MinecraftProfileId
            => MinecraftProfile?["id"]?.ToString();

        public string MinecraftProfileName
            => MinecraftProfile?["name"]?.ToString();

        [JsonProperty("persistent")]
        public bool Persistent { get; set; }

        [JsonProperty("remoteId")]
        public string RemoteId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("userProperites")]
        public List<object> UserProperites { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        public MSession ToSession()
        {
            return new MSession
            {
                Username = this.MinecraftProfileName,
                UUID = this.MinecraftProfileId,
                AccessToken = this.AccessToken
            };
        }
    }
}
