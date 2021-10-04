using Newtonsoft.Json;

namespace CmlLib.Core.Auth
{
    public class MSession
    {
        public MSession() { }

        public MSession(string? username, string? accessToken, string? uuid)
        {
            Username = username;
            AccessToken = accessToken;
            UUID = uuid;
        }

        [JsonProperty("username")]
        public string? Username { get; set; }
        [JsonProperty("session")]
        public string? AccessToken { get; set; }
        [JsonProperty("uuid")]
        public string? UUID { get; set; }
        [JsonProperty("clientToken")]
        public string? ClientToken { get; set; }
        
        public string? UserType { get; set; }

        public bool CheckIsValid()
        {
            return !string.IsNullOrEmpty(Username)
                && !string.IsNullOrEmpty(AccessToken)
                && !string.IsNullOrEmpty(UUID);
        }

        public static MSession GetOfflineSession(string username)
        {
            return new MSession
            {
                Username = username, 
                AccessToken = "access_token", 
                UUID = "user_uuid",
                UserType = "Mojang",
                ClientToken = null
            };
        }
    }
}
