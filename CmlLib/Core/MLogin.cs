using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace CmlLib.Core
{
    public enum MLoginResult { Success, BadRequest, WrongAccount, NeedLogin, UnknownError }

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
        public string Username { get; internal set; } = "";
        [JsonProperty("session")]
        public string AccessToken { get; internal set; } = "";
        [JsonProperty("uuid")]
        public string UUID { get; internal set; } = "";
        [JsonProperty("clientToken")]
        public string ClientToken { get; internal set; } = "";

        [JsonIgnore]
        public MLoginResult Result { get; internal set; }
        [JsonIgnore]
        public string Message { get; internal set; }
        [JsonIgnore]
        public string _RawResponse { get; internal set; }

        public bool CheckIsValid()
        {
            return Result == MLoginResult.Success
                && !string.IsNullOrEmpty(Username)
                && !string.IsNullOrEmpty(AccessToken)
                && !string.IsNullOrEmpty(UUID);
        }

        public static MSession GetOfflineSession(string username)
        {
            var login = new MSession();
            login.Username = username;
            login.AccessToken = "access_token";
            login.UUID = "user_uuid";
            login.Result = MLoginResult.Success;
            login.Message = "";
            login.ClientToken = "";
            return login;
        }
    }

    public class MLogin
    {
        class ErrorMessage
        {
            public string Message { get; set; }
            public MLoginResult Result { get; set; }
        }

        public static readonly string DefaultLoginSessionFile = Path.Combine(Minecraft.GetOSDefaultPath(), "logintoken.json");

        public MLogin() : this(DefaultLoginSessionFile) { }

        public MLogin(string tokenpath)
        {
            TokenFile = tokenpath;
        }

        public string TokenFile;
        public bool SaveSession = true;

        private void WriteLogin(MSession session)
        {
            if (!SaveSession) return;
            Directory.CreateDirectory(Path.GetDirectoryName(TokenFile));

            if (string.IsNullOrEmpty(session.ClientToken))
                session.ClientToken = CreateNewClientToken();

            var json = JsonConvert.SerializeObject(session);
            File.WriteAllText(TokenFile, json, Encoding.UTF8);
        }

        private string CreateNewClientToken()
        {
            return Guid.NewGuid().ToString().Replace("-", ""); // create new clienttoken
        }

        private MSession CreateNewSession()
        {
            var ClientToken = CreateNewClientToken();

            var session = new MSession();
            session.ClientToken = ClientToken;

            WriteLogin(session);
            return session;
        }

        public MSession GetLocalToken()
        {
            if (File.Exists(TokenFile))
            {
                var filedata = File.ReadAllText(TokenFile, Encoding.UTF8);
                try
                {
                    var session = JsonConvert.DeserializeObject<MSession>(filedata, new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });

                    return session;
                }
                catch (JsonReaderException) // if JSON file isn't valid
                {
                    return CreateNewSession();
                }
            }
            else
                return CreateNewSession();
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

        private MSession errorHandle(string json)
        {
            try
            {
                var result = new MSession();
                var job = JObject.Parse(json);

                var error = job["error"]?.ToString(); // error type
                result.Message = job["message"]?.ToString() ?? ""; // detail error message
                result._RawResponse = json;

                switch (error)
                {
                    case "Method Not Allowed":
                    case "Not Found":
                    case "Unsupported Media Type":
                        result.Result = MLoginResult.BadRequest;
                        break;
                    case "IllegalArgumentException":
                    case "ForbiddenOperationException":
                        result.Result = MLoginResult.WrongAccount;
                        break;
                    default:
                        result.Result = MLoginResult.UnknownError;
                        break;
                }

                return result;
            }
            catch (JsonReaderException)
            {
                return new MSession()
                {
                    Result = MLoginResult.UnknownError,
                    Message = json,
                    _RawResponse = json
                };
            }
            catch (Exception ex)
            {
                return new MSession()
                {
                    Result = MLoginResult.UnknownError,
                    Message = ex.Message,
                    _RawResponse = json
                };
            }
        }

        public MSession Authenticate(string id, string pw)
        {
            MSession result = new MSession();

            string ClientToken = GetLocalToken().ClientToken;

            var job = new JObject
            {
                { "username", id },
                { "password", pw },
                { "clientToken", ClientToken },

                { "agent", new JObject
                    {
                        { "name", "Minecraft" },
                        { "version", 1 }
                    }
                }
            };

            var resHeader = mojangRequest("authenticate", job.ToString());

            using (var res = new StreamReader(resHeader.GetResponseStream()))
            {
                var Response = res.ReadToEnd();

                result.ClientToken = ClientToken;

                if (resHeader.StatusCode == HttpStatusCode.OK) // ResultCode == 200
                {
                    var jObj = JObject.Parse(Response); //json parse
                    result.AccessToken = jObj["accessToken"].ToString();
                    result.UUID = jObj["selectedProfile"]["id"].ToString();
                    result.Username = jObj["selectedProfile"]["name"].ToString();

                    WriteLogin(result);
                    result.Result = MLoginResult.Success;
                }
                else // fail to login
                    return errorHandle(Response);

                return result;
            }
        }

        public MSession TryAutoLogin()
        {
            MSession result;
            result = Validate();
            if (result.Result != MLoginResult.Success)
                result = Refresh();

            return result;
        }

        public MSession Refresh()
        {
            var result = new MSession();

            try
            {
                var session = GetLocalToken();

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
                    var response = res.ReadToEnd();

                    if ((int)resHeader.StatusCode / 100 == 2)
                    {
                        JObject job = JObject.Parse(response);

                        result.AccessToken = job["accessToken"].ToString();
                        result.UUID = job["selectedProfile"]["id"].ToString();
                        result.Username = job["selectedProfile"]["name"].ToString();
                        result.ClientToken = session.ClientToken;

                        WriteLogin(result);
                        result.Result = MLoginResult.Success;
                    }
                    else
                        return errorHandle(response);
                }
            }
            catch (Exception ex)
            {
                result.Result = MLoginResult.UnknownError;
            }

            return result;
        }

        public MSession Validate()
        {
            var result = new MSession();
            try
            {
                var session = GetLocalToken();

                JObject job = new JObject
                {
                    { "accessToken", session.AccessToken },
                    { "clientToken", session.ClientToken }
                };

                var resHeader = mojangRequest("validate", job.ToString());
                using (var res = new StreamReader(resHeader.GetResponseStream()))
                {
                    if (resHeader.StatusCode == HttpStatusCode.NoContent) // StatusCode == 204
                    {
                        result.Result = MLoginResult.Success;
                        result.AccessToken = session.AccessToken;
                        result.UUID = session.UUID;
                        result.Username = session.Username;
                        result.ClientToken = session.ClientToken;
                    }
                    else
                        result.Result = MLoginResult.NeedLogin;
                }
            }
            catch (Exception ex)
            {
                throw ex;
                result.Result = MLoginResult.UnknownError;
            }

            return result;
        }

        public void DeleteTokenFile()
        {
            if (File.Exists(TokenFile))
                File.Delete(TokenFile);
        }

        public bool Invalidate()
        {
            var session = GetLocalToken();

            var job = new JObject
            {
                { "accessToken", session.AccessToken },
                { "clientToken", session.ClientToken }
            };

            var res = mojangRequest("invalidate", job.ToString());
            return res.StatusCode == HttpStatusCode.OK; // 200
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
