using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CmlLib.Core
{
    public class MSession
    {
        public MSession()
        {

        }

        public MSession(string username, string accesstoken, string uuid)
        {
            this.Username = username;
            this.AccessToken = accesstoken;
            this.UUID = uuid;
        }

        [JsonProperty("username")]
        public string Username { get; internal set; }
        [JsonProperty("session")]
        public string AccessToken { get; internal set; }
        [JsonProperty("uuid")]
        public string UUID { get; internal set; }
        [JsonProperty("clientToken")]
        public string ClientToken { get; internal set; }

        public bool CheckIsValid()
        {
            return !string.IsNullOrEmpty(Username)
                && !string.IsNullOrEmpty(AccessToken)
                && !string.IsNullOrEmpty(UUID);
        }

        public static MSession GetOfflineSession(string username)
        {
            var login = new MSession();
            login.Username = username;
            login.AccessToken = "access_token";
            login.UUID = "user_uuid";
            login.ClientToken = null;
            return login;
        }
    }
}
