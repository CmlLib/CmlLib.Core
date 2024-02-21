using System;
using Newtonsoft.Json;

namespace CmlLib.Core.Auth;

public class MSession
{
    public MSession()
    {
    }

    public MSession(string? username, string? accessToken, string? uuid)
    {
        Username = username;
        AccessToken = accessToken;
        UUID = uuid;
    }

    [JsonProperty("username")] public string? Username { get; set; }

    [JsonProperty("session")] public string? AccessToken { get; set; }

    [JsonProperty("uuid")] public string? UUID { get; set; }

    [JsonProperty("clientToken")] public string? ClientToken { get; set; }

    public string? Xuid { get; set; }

    public string? UserType { get; set; }

    public bool CheckIsValid()
    {
        return !string.IsNullOrEmpty(Username)
               && !string.IsNullOrEmpty(AccessToken)
               && !string.IsNullOrEmpty(UUID);
    }

    [Obsolete(
        "Use CreateOfflineSession(\"username\") instead. From 1.20.2, the game would not start up with the session created by this method.")]
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

    // Create offline session with valid UUID. Since 1.20.2, passing invalid UUID crashes the game
    public static MSession CreateOfflineSession(string username)
    {
        return new MSession
        {
            Username = username,
            AccessToken = "access_token",
            UUID = Guid.NewGuid().ToString().Replace("-", ""), // create random valid UUID
            UserType = "msa",
            ClientToken = null
        };
    }
}
