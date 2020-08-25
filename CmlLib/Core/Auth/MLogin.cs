using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace CmlLib.Core.Auth
{
    public enum MLoginResult { Success, BadRequest, WrongAccount, NeedLogin, UnknownError, NoProfile }

    public class MLogin
    {
        public static readonly string DefaultLoginSessionFile = Path.Combine(MinecraftPath.GetOSDefaultPath(), "logintoken.json");

        public MLogin() : this(DefaultLoginSessionFile) { }

        public MLogin(string sessionCacheFilePath)
        {
            SessionCacheFilePath = sessionCacheFilePath;
        }

        public string SessionCacheFilePath { get; private set; }
        public bool SaveSession { get; private set; } = true;

        private string CreateNewClientToken()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        private MSession CreateNewSession()
        {
            var session = new MSession();
            if (SaveSession)
            {
                session.ClientToken = CreateNewClientToken();
                WriteSessionCache(session);
            }
            return session;
        }

        private void WriteSessionCache(MSession session)
        {
            if (!SaveSession) return;
            Directory.CreateDirectory(Path.GetDirectoryName(SessionCacheFilePath));

            var json = JsonConvert.SerializeObject(session);
            File.WriteAllText(SessionCacheFilePath, json, Encoding.UTF8);
        }

        public MSession ReadSessionCache()
        {
            if (File.Exists(SessionCacheFilePath))
            {
                var filedata = File.ReadAllText(SessionCacheFilePath, Encoding.UTF8);
                try
                {
                    var session = JsonConvert.DeserializeObject<MSession>(filedata, new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });

                    if (SaveSession && string.IsNullOrEmpty(session.ClientToken))
                        session.ClientToken = CreateNewClientToken();

                    return session;
                }
                catch (JsonReaderException) // invalid json
                {
                    return CreateNewSession();
                }
            }
            else
            {
                return CreateNewSession();
            }
        }

        private HttpWebResponse mojangRequest(string endpoint, string postdata)
        {
            var http = WebRequest.CreateHttp(MojangServer.Auth + endpoint);
            http.ContentType = "application/json";
            http.Method = "POST";
            using (var req = new StreamWriter(http.GetRequestStream()))
            {
                req.Write(postdata);
                req.Flush();
            }

            var res = http.GetResponseNoException();
            return res;
        }

        private MLoginResponse parseSession(string json, string clientToken)
        {
            var job = JObject.Parse(json); //json parse

            var profile = job["selectedProfile"];
            if (profile == null)
                return new MLoginResponse(MLoginResult.NoProfile, null, null, json);
            else
            {
                var session = new MSession()
                {
                    AccessToken = job["accessToken"]?.ToString(),
                    UUID = profile["id"]?.ToString(),
                    Username = profile["name"]?.ToString(),
                    ClientToken = clientToken
                };

                WriteSessionCache(session);
                return new MLoginResponse(MLoginResult.Success, session, null, null);
            }
        }

        private MLoginResponse errorHandle(string json)
        {
            try
            {
                var job = JObject.Parse(json);

                var error = job["error"]?.ToString(); // error type
                var errormsg = job["message"]?.ToString() ?? ""; // detail error message
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

                return new MLoginResponse(result, null, errormsg, json);
            }
            catch (Exception ex)
            {
                return new MLoginResponse(MLoginResult.UnknownError, null, ex.ToString(), json);
            }
        }

        public MLoginResponse Authenticate(string id, string pw)
        {
            var clientToken = ReadSessionCache().ClientToken;
            return Authenticate(id, pw, clientToken);
        }

        public MLoginResponse Authenticate(string id, string pw, string clientToken)
        {
            var req = new JObject
            {
                { "username", id },
                { "password", pw },
                { "clientToken", clientToken },
                { "agent", new JObject
                    {
                        { "name", "Minecraft" },
                        { "version", 1 }
                    }
                }
            };

            var resHeader = mojangRequest("authenticate", req.ToString());

            using (var res = new StreamReader(resHeader.GetResponseStream()))
            {
                var rawResponse = res.ReadToEnd();
                if (resHeader.StatusCode == HttpStatusCode.OK) // ResultCode == 200
                    return parseSession(rawResponse, clientToken);
                else // fail to login
                    return errorHandle(rawResponse);
            }
        }

        public MLoginResponse TryAutoLogin()
        {
            var session = ReadSessionCache();
            return TryAutoLogin(session);
        }

        public MLoginResponse TryAutoLogin(MSession session)
        {
            try
            {
                var result = Validate(session);
                if (result.Result != MLoginResult.Success)
                    result = Refresh(session);
                return result;
            }
            catch (Exception ex)
            {
                return new MLoginResponse(MLoginResult.UnknownError, null, ex.ToString(), null);
            }
        }

        public MLoginResponse Refresh()
        {
            var session = ReadSessionCache();
            return Refresh(session);
        }

        public MLoginResponse Refresh(MSession session)
        {
            var req = new JObject
                {
                    { "accessToken", session.AccessToken },
                    { "clientToken", session.ClientToken },
                    { "selectedProfile", new JObject()
                        {
                            { "id", session.UUID },
                            { "name", session.Username }
                        }
                    }
                };

            var resHeader = mojangRequest("refresh", req.ToString());
            using (var res = new StreamReader(resHeader.GetResponseStream()))
            {
                var rawResponse = res.ReadToEnd();

                if ((int)resHeader.StatusCode / 100 == 2)
                    return parseSession(rawResponse, session.ClientToken);
                else
                    return errorHandle(rawResponse);
            }
        }

        public MLoginResponse Validate()
        {
            var session = ReadSessionCache();
            return Validate(session);
        }

        public MLoginResponse Validate(MSession session)
        {
            JObject req = new JObject
                {
                    { "accessToken", session.AccessToken },
                    { "clientToken", session.ClientToken }
                };

            var resHeader = mojangRequest("validate", req.ToString());
            using (var res = new StreamReader(resHeader.GetResponseStream()))
            {
                if (resHeader.StatusCode == HttpStatusCode.NoContent) // StatusCode == 204
                    return new MLoginResponse(MLoginResult.Success, session, null, null);
                else
                    return new MLoginResponse(MLoginResult.NeedLogin, null, null, null);
            }
        }

        public void DeleteTokenFile()
        {
            if (File.Exists(SessionCacheFilePath))
                File.Delete(SessionCacheFilePath);
        }

        public bool Invalidate()
        {
            var session = ReadSessionCache();
            return Invalidate(session);
        }

        public bool Invalidate(MSession session)
        {
            var job = new JObject
            {
                { "accessToken", session.AccessToken },
                { "clientToken", session.ClientToken }
            };

            var res = mojangRequest("invalidate", job.ToString());
            return res.StatusCode == HttpStatusCode.NoContent; // 204
        }

        public bool Signout(string id, string pw)
        {
            var job = new JObject
            {
                { "username", id },
                { "password", pw }
            };

            var res = mojangRequest("signout", job.ToString());
            return res.StatusCode == HttpStatusCode.NoContent; // 204
        }
    }

    public static class HttpWebResponseExt
    {
        public static HttpWebResponse GetResponseNoException(this HttpWebRequest req)
        {
            try
            {
                return (HttpWebResponse)req.GetResponse();
            }
            catch (WebException we)
            {
                var resp = we.Response as HttpWebResponse;
                if (resp == null)
                    throw;
                return resp;
            }
        }
    }
}
