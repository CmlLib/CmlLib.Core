using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;

namespace CmlLib.Launcher
{
    public enum MLoginResult { Success, BadRequest, WrongAccount, NeedLogin, UnknownError }

    public class MSession
    {
        public string Username { get; internal set; }
        public string AccessToken { get; internal set; }
        public string UUID { get; internal set; }
        public string ClientToken { get; internal set; }

        public MLoginResult Result { get; internal set; }
        public string Message { get; internal set; }

        public string _RawResponse { get; internal set; }

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

        internal static MSession createEmpty()
        {
            var session = new MSession();
            session.Username = "";
            session.AccessToken = "";
            session.UUID = "";
            session.ClientToken = "";
            return session;
        }
    }

    public class MLogin
    {

        public static readonly string DefaultLoginSessionFile = Minecraft.DefaultPath + "\\logintoken.json";

        public MLogin() : this(DefaultLoginSessionFile) { }

        public MLogin(string tokenpath)
        {
            TokenFile = tokenpath;
        }

        public string TokenFile;
        public bool SaveSession = true;

        private void WriteLogin(MSession result)
        {
            WriteLogin(result.Username, result.AccessToken, result.UUID, result.ClientToken);
        }

        // Save Login Session
        private void WriteLogin(string us, string se, string id, string ct)
        {
            if (!SaveSession) return;

            JObject jobj = new JObject(); // create session json
            jobj.Add("username", us);
            jobj.Add("session", se);
            jobj.Add("uuid", id);
            jobj.Add("clientToken", ct);

            Directory.CreateDirectory(Path.GetDirectoryName(TokenFile));

            File.WriteAllText(TokenFile,jobj.ToString() , Encoding.UTF8);
        }

        public MSession GetLocalToken()
        {
            MSession session;

            if (!File.Exists(TokenFile)) // no session data
            {
                var ClientToken = Guid.NewGuid().ToString().Replace("-", ""); // create new clienttoken

                session = MSession.createEmpty();
                session.ClientToken = ClientToken;

                WriteLogin(session);
            }
            else // exists session data
            {
                var filedata = File.ReadAllText(TokenFile, Encoding.UTF8);
                try
                {
                    var job = JObject.Parse(filedata);
                    session = new MSession();
                    session.AccessToken = job["session"]?.ToString();
                    session.UUID = job["uuid"]?.ToString();
                    session.Username = job["username"]?.ToString();
                    session.ClientToken = job["clientToken"]?.ToString();
                }
                catch (Newtonsoft.Json.JsonReaderException) // if JSON file isn't vaild
                {
                    DeleteTokenFile();
                    session = GetLocalToken();
                }
            }

            return session;
        }

        private HttpWebResponse mojangRequest(string endpoint, string postdata)
        {
            var http = WebRequest.CreateHttp("https://authserver.mojang.com/" + endpoint);
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
                {
                    var json = JObject.Parse(Response); 

                    var error = json["error"]?.ToString(); // error type
                    result._RawResponse = Response;
                    result.Message = json["message"]?.ToString() ?? ""; // detail error message

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
                }

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
                    result._RawResponse = response;
                    JObject job = JObject.Parse(response);

                    result.AccessToken = job["accessToken"].ToString();
                    result.AccessToken = job["accessToken"].ToString();
                    result.UUID = job["selectedProfile"]["id"].ToString();
                    result.Username = job["selectedProfile"]["name"].ToString();
                    result.ClientToken = session.ClientToken;

                    WriteLogin(result);
                    result.Result = MLoginResult.Success;
                }
            }
            catch
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
            catch
            {
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
