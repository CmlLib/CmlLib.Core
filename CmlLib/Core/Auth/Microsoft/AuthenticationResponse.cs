using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace CmlLib.Core.Auth.Microsoft
{
    public class AuthenticationResponse
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("roles")]
        public string[] Roles { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
