using Newtonsoft.Json;

namespace CmlLib.Core.Auth
{
    public class MSession
    {
        public MSession() { }

        public MSession(string username, string accesstoken, string uuid)
        {
            Username = username;
            AccessToken = accesstoken;
            UUID = uuid;
        }

        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("session")]
        public string AccessToken { get; set; }
        [JsonProperty("uuid")]
        public string UUID { get; set; }
        [JsonProperty("clientToken")]
        public string ClientToken { get; set; }

        public bool CheckIsValid()
        {
            return !string.IsNullOrEmpty(Username)
                && !string.IsNullOrEmpty(AccessToken)
                && !string.IsNullOrEmpty(UUID);
        }

        public static MSession GetOfflineSession(string username)
        {
            MSession login = new MSession();
            login.Username = username;
            login.AccessToken = "access_token";
            login.UUID = "user_uuid";
            login.ClientToken = null;
            return login;
        }
    }
}
