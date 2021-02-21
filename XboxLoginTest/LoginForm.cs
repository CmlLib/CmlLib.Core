using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft;
using CmlLib.Core.Mojang;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using XboxAuthNet.OAuth;
using XboxAuthNet.XboxLive;

namespace XboxLoginTest
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void CreateWV()
        {
            wv = new WebView2();
            wv.NavigationStarting += WebView21_NavigationStarting;
            wv.Dock = DockStyle.Fill;
            this.Controls.Add(wv);
            this.Controls.SetChildIndex(wv, 0);
        }

        private void RemoveWV()
        {
            if (wv != null)
            {
                try
                {
                    this.Controls.Remove(wv);
                    //wv.Dispose();
                    wv = null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        WebView2 wv;

        MicrosoftOAuth oauth;
        public MSession session;

        public string action = "login";

        string microsoftOAuthPath = Path.Combine(MinecraftPath.GetOSDefaultPath(), "cml_msa.json");
        string minecraftTokenPath = Path.Combine(MinecraftPath.GetOSDefaultPath(), "cml_token.json");

        private void Window_Loaded(object sender, EventArgs e)
        {
            if (action == "login")
            {
                login();
            }
            else if (action == "signout")
            {
                signout();
            }
        }

        private MicrosoftOAuthResponse readMicrosoft()
        {
            if (!File.Exists(microsoftOAuthPath))
                return null;

            var file = File.ReadAllText(microsoftOAuthPath);
            var response = JsonConvert.DeserializeObject<MicrosoftOAuthResponse>(file);

            return response;
        }

        private void writeMicrosoft(MicrosoftOAuthResponse response)
        {
            var json = JsonConvert.SerializeObject(response);
            File.WriteAllText(microsoftOAuthPath, json);
        }

        private AuthenticationResponse readMinecraft()
        {
            if (!File.Exists(minecraftTokenPath))
                return null;

            var file = File.ReadAllText(minecraftTokenPath);
            var job = JObject.Parse(file);

            this.session = job["session"].ToObject<MSession>();
            return job["auth"].ToObject<AuthenticationResponse>();
        }

        private void writeMinecraft(AuthenticationResponse mcToken)
        {
            var obj = new
            {
                auth = mcToken,
                session = session
            };
            var json = JsonConvert.SerializeObject(obj);
            File.WriteAllText(minecraftTokenPath, json);
        }

        private void login()
        {
            try
            {
                oauth = new MicrosoftOAuth("00000000402B5328", XboxAuth.XboxScope);
                var msToken = readMicrosoft();
                var mcToken = readMinecraft();

                //if (true)
                if (mcToken == null || DateTime.Now > mcToken.ExpiresOn) // expired
                {
                    this.session = null;

                    //if (true)
                    if (oauth.TryGetTokens(out msToken, msToken?.RefreshToken)) // try ms login
                        successMS(msToken);
                    else // failed to refresh ms token
                    {
                        var url = oauth.CreateUrl();
                        CreateWV();
                        wv.Source = new Uri(url);
                    }
                }
                else // valid minecraft session
                {
                    if (this.session == null)
                        this.session = getSession(mcToken);

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                this.session = null;
            }
        }

        private void signout()
        {
            writeMicrosoft(null);
            writeMinecraft(null);

            CreateWV();
            wv.Source = new Uri(MicrosoftOAuth.GetSignOutUrl());
        }

        private void WebView21_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            if (e.IsRedirected && oauth.CheckLoginSuccess(e.Uri)) // login success
            {
                RemoveWV();

                new Thread(() =>
                {
                    var result = oauth.TryGetTokens(out MicrosoftOAuthResponse response); // get token
                    Invoke(new Action(() =>
                    {
                        if (result)
                            successMS(response);
                        else
                            msLoginFail(response);
                    }));
                }).Start();
            }
        }

        private void successMS(MicrosoftOAuthResponse msToken)
        {
            try
            {
                writeMicrosoft(msToken);
                var mcToken = mcLogin(msToken);
                if (mcToken != null)
                {
                    this.session = getSession(mcToken);
                    writeMinecraft(mcToken);
                }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                this.Close();
            }
        }

        private AuthenticationResponse mcLogin(MicrosoftOAuthResponse msToken)
        {
            try
            {
                if (msToken == null)
                    throw new ArgumentNullException("msToken was null");

                var xbox = new XboxAuth();
                var rps = xbox.ExchangeRpsTicketForUserToken(msToken.AccessToken);
                var xsts = xbox.ExchangeTokensForXSTSIdentity(rps.Token, null, null, XboxMinecraftLogin.RelyingParty, null);

                if (!xsts.IsSuccess)
                {
                    var msg = "";
                    if (xsts.Error == XboxAuthResponse.ChildError)
                        msg = "Child error";
                    else if (xsts.Error == XboxAuthResponse.NoXboxAccountError)
                        msg = "No Xbox Account";

                    MessageBox.Show($"Failed to xbox login : {xsts.Error}\n{xsts.Message}\n{msg}");
                    return null;
                }

                var mclogin = new XboxMinecraftLogin();
                var mcToken = mclogin.LoginWithXbox(xsts.UserHash, xsts.Token);
                return mcToken;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

        private void msLoginFail(MicrosoftOAuthResponse res)
        {
            MessageBox.Show(
    $"Failed to microsoft login : {res.Error}\n" +
    $"ErrorDescription : {res.ErrorDescription}\n" +
    $"ErrorCodes : {string.Join(",", res.ErrorCodes)}");

            this.Close();
        }

        private MSession getSession(AuthenticationResponse mcToken)
        {
            if (mcToken == null)
                throw new ArgumentNullException("mcToken was null");

            if (!MojangAPI.CheckGameOwnership(mcToken.AccessToken))
            {
                MessageBox.Show("로그인 실패 : 게임 구매를 하지 않았습니다.");
                this.Close();
            }

            var profile = MojangAPI.GetProfileUsingToken(mcToken.AccessToken);
            return new MSession
            {
                AccessToken = mcToken.AccessToken,
                UUID = profile.UUID,
                Username = profile.Name
            };
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RemoveWV();
        }
    }
}
