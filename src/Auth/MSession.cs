using System.Text.Json.Serialization;

namespace CmlLib.Core.Auth;

public class MSession
{
    public MSession() { }

    public MSession(string? username, string? accessToken, string? uuid)
    {
        Username = username;
        AccessToken = accessToken;
        UUID = uuid;
    }

    [JsonPropertyName("username")]
    public string? Username { get; set; }
    [JsonPropertyName("session")]
    public string? AccessToken { get; set; }
    [JsonPropertyName("uuid")]
    public string? UUID { get; set; }
    [JsonPropertyName("clientToken")]
    public string? ClientToken { get; set; }
    [JsonPropertyName("userType")]
    public string? UserType { get; set; }
    [JsonPropertyName("xuid")]
    public string? Xuid { get; set; }

    public bool CheckIsValid()
    {
        return !string.IsNullOrEmpty(Username)
            && !string.IsNullOrEmpty(AccessToken)
            && !string.IsNullOrEmpty(UUID);
    }

    public static MSession CreateLegacyOfflineSession(string username)
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

    // legacy api
    [Obsolete("Use CreateOfflineSession(\"username\") instead.")]
    public static MSession GetOfflineSession(string username) => CreateOfflineSession(username);
}
