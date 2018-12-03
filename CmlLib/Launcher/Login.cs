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
    /// <summary>
    /// 로그인 결과를 알려줍니다. 
    /// </summary>
    public enum MLoginResult { Success, BadRequest, WrongAccount, NeedLogin, UnknownError }

    /// <summary>
    /// 닉네임, 엑세스토큰, UUID 가 저장된 세션 클래스입니다.
    /// </summary>
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

    /// <summary>
    /// 모장 서버에 로그인을 요청합니다.
    /// </summary>
    public class MLogin
    {
        /// <summary>
        /// 로그인 세션이 저장되는 기본 폴더입니다.
        /// </summary>
        public static readonly string DefaultLoginSessionFile = Minecraft.mPath + "\\logintoken.json";

        /// <summary>
        /// DefaultLoginSessionFile 필드의 값으로 로그인 세션을 저장합니다.
        /// </summary>
        public MLogin() : this(DefaultLoginSessionFile) { }

        /// <summary>
        /// 로그인 세션을 저장할 파일을 지정하고 로그인 객체를 생성합니다.
        /// </summary>
        /// <param name="tokenpath">세션 저장할 파일 경로</param>
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

        private void WriteLogin(string us, string se, string id, string ct)
        {
            if (!SaveSession) return;

            //로그인 정보를 저장
            JObject jobj = new JObject(); //json 데이터 작성
            jobj.Add("username", us);
            jobj.Add("session", se);
            jobj.Add("uuid", id);
            jobj.Add("clientToken", ct);

            Directory.CreateDirectory(Path.GetDirectoryName(TokenFile));

            File.WriteAllText(TokenFile,jobj.ToString() , Encoding.UTF8);
        }

        private MSession GetLocalToken()
        {
            Console.WriteLine("GetLocalToken");
            MSession session;

            if (!File.Exists(TokenFile)) //로그인 정보가 없을경우
            {
                Console.WriteLine("No Token Exist");
                var ClientToken = Guid.NewGuid().ToString().Replace("-", "");      //새로운 클라이언트 토큰 생성
                Console.WriteLine("New ClientToken : " + ClientToken);

                session = MSession.createEmpty();
                session.ClientToken = ClientToken;

                WriteLogin(session);
            }
            else
            {   //로그인 정보가 남아있을경우 클라이언트 토큰 불러옴
                Console.WriteLine("Client Token Exist!");
                var filedata = File.ReadAllText(TokenFile, Encoding.UTF8);
                Console.WriteLine(Convert.ToBase64String(Encoding.UTF8.GetBytes(filedata)));

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

        public MSession Authenticate(string id, string pw) //로그인
        {
            try
            {
                MSession result = new MSession();

                string ClientToken = GetLocalToken().ClientToken;
                string Response = "";

                var req = "{ \"agent\" : { \"name\" : \"Minecraft\" , \"version\" : 1 }, \"username\" : \"" + id + "\", \"password\" : \"" + pw + "\", \"clientToken\" : \"" + ClientToken + "\"}";
                var resHeader = mojangRequest("authenticate", req);

                using (var res = new StreamReader(resHeader.GetResponseStream()))
                {
                    Response = res.ReadToEnd(); //받아옴

                    result._RawResponse = Response;

                    if ((int)(resHeader).StatusCode == 200) //ResultCode 가 200 (성공) 일때만
                    {
                        var jObj = JObject.Parse(Response); //json 파싱
                        result.AccessToken = jObj["accessToken"].ToString();
                        result.AccessToken = jObj["accessToken"].ToString();
                        result.UUID = jObj["selectedProfile"]["id"].ToString();
                        result.Username = jObj["selectedProfile"]["name"].ToString();
                        result.ClientToken = ClientToken;

                        WriteLogin(result); //로그인 정보 쓰기
                        result.Result = MLoginResult.Success;
                    }
                    else //로그인이 실패하였을때
                    {
                        var json = JObject.Parse(Response); //에러메세지 불러옴
                        var msg = json["error"]?.ToString();

                        switch (msg) //메세지를 상황에 맞게 바꿈
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
            catch //예외발생
            {
                var o = new MSession();
                o.Result = MLoginResult.UnknownError;
                return o;
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

        public MSession Refresh() //엑세스 토큰 새로가져오기
        {
            var result = new MSession();

            try
            {
                var session = GetLocalToken();

                JObject selectedProfile = new JObject();
                selectedProfile.Add("id", session.UUID); //uuid 추가
                selectedProfile.Add("name", session.Username); //유저네임 추가

                JObject req = new JObject
                    {
                        { "accessToken", session.AccessToken }, //새로고침할 토큰 (오래된 토큰)
                        { "clientToken", session.ClientToken }, //클라이언트 고유 토큰
                        { "selectedProfile", selectedProfile } //////
                    };

                var resHeader = mojangRequest("refresh", req.ToString()); //응답 가져오기
                using (var res = new StreamReader(resHeader.GetResponseStream()))
                {
                    var response = res.ReadToEnd();
                    result._RawResponse = response;
                    JObject job = JObject.Parse(response); //json 파일 파싱

                    result.AccessToken = job["accessToken"].ToString();
                    result.AccessToken = job["accessToken"].ToString();
                    result.UUID = job["selectedProfile"]["id"].ToString();
                    result.Username = job["selectedProfile"]["name"].ToString();
                    result.ClientToken = session.ClientToken;

                    WriteLogin(result); //로그인 정보 쓰기
                    result.Result = MLoginResult.Success;
                }
            }
            catch
            {
                result.Result = MLoginResult.UnknownError;
            }

            return result;
        }

        public MSession Validate() //현재 토큰이 유효한 토큰인지 검사
        {
            //(오래된 토큰은 사용불가능하므로 RefreshLogin 메서드로 새로가져와야함)

            var result = new MSession();
            try
            {
                var session = GetLocalToken();

                JObject job = new JObject();
                job.Add("accessToken", session.AccessToken);
                job.Add("clientToken", session.ClientToken);

                var resHeader = mojangRequest("validate",job.ToString()); //리스폰 데이터 받아오기
                using (var res = new StreamReader(resHeader.GetResponseStream()))
                {
                    int ResultCode = (int)((HttpWebResponse)resHeader).StatusCode;

                    if (ResultCode == 204)
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

        public void Invalidate()
        {
            var result = new MSession();
            var session = GetLocalToken();

            var job = new JObject();
            job.Add("accessToken",session.AccessToken);
            job.Add("clientToken", session.ClientToken);

            mojangRequest("signout", job.ToString());
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
