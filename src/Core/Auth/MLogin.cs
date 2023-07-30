using CmlLib.Utils;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

// use new library:
// https://github.com/CmlLib/MojangAPI

namespace CmlLib.Core.Auth
{
    public enum MLoginResult { Success, BadRequest, WrongAccount, NeedLogin, UnknownError, NoProfile }

    public class MLogin
    {
        public static readonly string DefaultLoginSessionFile
            = Path.Combine(MinecraftPath.GetOSDefaultPath(), "logintoken.json");

        public MLogin() : this(DefaultLoginSessionFile, HttpUtil.HttpClient) { }

        public MLogin(string sessionCacheFilePath, HttpClient client)
        {
            SessionCacheFilePath = sessionCacheFilePath;
            this.httpClient = client;
        }

        private readonly HttpClient httpClient;
        public string SessionCacheFilePath { get; private set; }
        public bool SaveSession { get; set; } = true;

        protected virtual string CreateNewClientToken()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        protected virtual async Task<MSession> createNewSession()
        {
            var session = new MSession();
            if (SaveSession)
            {
                session.ClientToken = CreateNewClientToken();
                await writeSessionCache(session);
            }
            return session;
        }

        private async Task writeSessionCache(MSession session)
        {
            if (!SaveSession) return;
            IOUtil.CreateParentDirectory(SessionCacheFilePath);

            var json = JsonSerializer.Serialize(session);
            await IOUtil.WriteFileAsync(SessionCacheFilePath, json);
        }

        public async Task <MSession> ReadSessionCache()
        {
            if (File.Exists(SessionCacheFilePath))
            {
                var fileData = await IOUtil.ReadFileAsync(SessionCacheFilePath);
                try
                {
                    var session = JsonSerializer.Deserialize<MSession>(fileData, JsonUtil.JsonOptions)
                        ?? new MSession();

                    if (SaveSession && string.IsNullOrEmpty(session.ClientToken))
                        session.ClientToken = CreateNewClientToken();

                    return session;
                }
                catch (JsonException) // invalid json
                {
                    return await createNewSession();
                }
            }
            else
            {
                return await createNewSession();
            }
        }

        protected async Task<HttpResponseMessage> mojangRequest(string endpoint, object postData)
        {
            var json = JsonSerializer.Serialize(postData, JsonUtil.JsonOptions);
            var res = await httpClient.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(MojangServer.Auth + endpoint),
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });
            return res;
        }

        protected async Task<MLoginResponse> mojangRequestHandle(string endpoint, object postdata, string? clientToken=null)
        {
            var res = await mojangRequest(endpoint, postdata);

            var str = await res.Content.ReadAsStringAsync();
            if (res.IsSuccessStatusCode)
                return await parseSession(str, clientToken);
            else
                return errorHandle(str);
        }

        private async Task<MLoginResponse> parseSession(string json, string? clientToken)
        {
            using var jsonDocument = JsonDocument.Parse(json);
            var root = jsonDocument.RootElement;
 
            if (!root.TryGetProperty("selectedProfile", out var profile))
                return new MLoginResponse(MLoginResult.NoProfile, null, null, json);
            else
            {
                var session = new MSession
                {
                    AccessToken = root.GetPropertyValue("accessToken"),
                    UUID = profile.GetPropertyValue("id"),
                    Username = profile.GetPropertyValue("name"),
                    UserType = "Mojang",
                    ClientToken = clientToken
                };

                await writeSessionCache(session);
                return new MLoginResponse(MLoginResult.Success, session, null, null);
            }
        }

        private MLoginResponse errorHandle(string json)
        {
            try
            {
                using var jsonDocument = JsonDocument.Parse(json);
                var root = jsonDocument.RootElement;

                string? error = root.GetPropertyValue("error"); // error type
                string? errorMessage = root.GetPropertyValue("message"); // detail error message
                MLoginResult result;

                switch (error)
                {
                    case "Method Not Allowed":
                    case "Not Found":
                    case "Unsupported Media Type":
                        result = MLoginResult.BadRequest;
                        break;
                    case "IllegalArgumentException":
                    case "ForbiddenOperationException":
                        result = MLoginResult.WrongAccount;
                        break;
                    default:
                        result = MLoginResult.UnknownError;
                        break;
                }

                return new MLoginResponse(result, null, errorMessage, json);
            }
            catch (JsonException ex)
            {
                return new MLoginResponse(MLoginResult.UnknownError, null, ex.ToString(), json);
            }
        }

        public async Task<MLoginResponse> Authenticate(string username, string password)
        {
            var sessionCache = await ReadSessionCache();
            string? clientToken = sessionCache.ClientToken;
            return await Authenticate(username, password, clientToken);
        }

        public Task<MLoginResponse> Authenticate(string username, string password, string? clientToken)
            => mojangRequestHandle("authenticate", new
            {
                username,
                password,
                clientToken,
                agent = new
                {
                    name = "Minecraft",
                    version = 1
                }
            }, clientToken);

        public async Task<MLoginResponse> TryAutoLogin()
        {
            MSession session = await ReadSessionCache();
            return await TryAutoLogin(session);
        }

        public async Task<MLoginResponse> TryAutoLogin(MSession session)
        {
            MLoginResponse result = await Validate(session);
            if (result.Result != MLoginResult.Success)
                result = await Refresh(session);
            return result;
        }

        public async Task<MLoginResponse> Refresh()
        {
            var session = await ReadSessionCache();
            return await Refresh(session);
        }

        public Task<MLoginResponse> Refresh(MSession session)
            => mojangRequestHandle("refresh", new
            {
                accessToken = session.AccessToken,
                clientToken = session.ClientToken,
                selectedProfile = new
                {
                    id = session.UUID,
                    name = session.Username
                }
            }, session.ClientToken);

        public async Task<MLoginResponse> Validate()
        {
            var session = await ReadSessionCache();
            return await Validate(session);
        }

        public async Task<MLoginResponse> Validate(MSession session)
        {
            var res = await mojangRequest("validate", new
            {
                accessToken = session.AccessToken,
                clientToken = session.ClientToken
            });

            if (res.IsSuccessStatusCode)
                return new MLoginResponse(MLoginResult.Success, session, null, null);
            else
                return new MLoginResponse(MLoginResult.NeedLogin, null, null, null);
        }

        public void DeleteTokenFile()
        {
            if (File.Exists(SessionCacheFilePath))
                File.Delete(SessionCacheFilePath);
        }

        public async Task<bool> Invalidate()
        {
            var session = await ReadSessionCache();
            return await Invalidate(session);
        }

        public async Task<bool> Invalidate(MSession session)
        {
            var res = await mojangRequest("invalidate", new
            {
                accessToken = session.AccessToken,
                clientToken = session.ClientToken
            });

            return res.IsSuccessStatusCode;
        }

        public async Task<bool> Signout(string username, string password)
        {
            var res = await mojangRequest("signout", new
            {
                username,
                password
            });

            return res.StatusCode == HttpStatusCode.NoContent; // 204
        }
    }
}
