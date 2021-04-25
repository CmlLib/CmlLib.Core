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

        WebView2 wv;
        #region Create/Remove WebView2 control

        // Show webview on form
        private void CreateWV()
        {
            wv = new WebView2();
            wv.NavigationStarting += WebView21_NavigationStarting;
            wv.Dock = DockStyle.Fill;
            this.Controls.Add(wv);
            this.Controls.SetChildIndex(wv, 0);
        }

        // Remove webview on form
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

        #endregion

        public MSession session;
        public string action = "login";

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

        #region Microsoft token cache

        string microsoftOAuthPath = Path.Combine(MinecraftPath.GetOSDefaultPath(), "cml_msa.json");

        // read microsoft token cache file
        private MicrosoftOAuthResponse readMicrosoft()
        {
            if (!File.Exists(microsoftOAuthPath))
                return null;

            var file = File.ReadAllText(microsoftOAuthPath);
            var response = JsonConvert.DeserializeObject<MicrosoftOAuthResponse>(file);

            return response;
        }

        // write microsoft login cache file
        private void writeMicrosoft(MicrosoftOAuthResponse response)
        {
            var json = JsonConvert.SerializeObject(response);
            File.WriteAllText(microsoftOAuthPath, json);
        }

        #endregion

        #region Minecraft token cache

        string minecraftTokenPath = Path.Combine(MinecraftPath.GetOSDefaultPath(), "cml_token.json");

        // read minecraft login cache file
        private AuthenticationResponse readMinecraft()
        {
            if (!File.Exists(minecraftTokenPath))
                return null;

            var file = File.ReadAllText(minecraftTokenPath);
            var job = JObject.Parse(file);

            this.session = job["session"].ToObject<MSession>();
            return job["auth"].ToObject<AuthenticationResponse>();
        }

        // write minecraft login cache file
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

        #endregion

        // Login
        // 1. read microsoft, minecraft token cache file
        // 2. check if minecraft token is valid
        // 3. try to get microsoft token (check if microsoft token is valid)
        // 4. microsoft browser login
        // 5. get minecraft token
        // 6. get minecraft profile (username, uuid)

        MicrosoftOAuth oauth;

        private void login()
        {
            try
            {
                oauth = new MicrosoftOAuth("00000000402B5328", XboxAuth.XboxScope);

                // 1. Read microsoft, minecraft token cache file
                var msToken = readMicrosoft();
                var mcToken = readMinecraft();

                // 2. Check if minecraft token is valid
                if (mcToken == null || DateTime.Now > mcToken.ExpiresOn) // invalid minecraft token
                {
                    this.session = null;

                    // 3. Try to get microsoft token
                    if (!oauth.TryGetTokens(out msToken, msToken?.RefreshToken))
                    {
                        // 4. Browser login
                        var url = oauth.CreateUrl();
                        CreateWV(); // show webview control
                        wv.Source = new Uri(url);
                    }
                    else // success to get microsoft token
                        successMS(msToken); // goto 5
                }
                else // valid minecraft token
                {
                    if (this.session == null)
                        this.session = getSession(mcToken); // goto 6

                    this.Close(); // success
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                this.session = null;
            }
        }

        private void WebView21_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            if (e.IsRedirected && oauth.CheckLoginSuccess(e.Uri)) // microsoft browser login success
            {
                RemoveWV(); // remove webview control

                new Thread(() =>
                {
                    var result = oauth.TryGetTokens(out MicrosoftOAuthResponse response); // get token
                    Invoke(new Action(() =>
                    {
                        if (result)
                            successMS(response); // goto 5
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
                // 5. get minecraft token
                var mcToken = mcLogin(msToken);
                if (mcToken != null)
                {
                    this.session = getSession(mcToken); // goto 6
                    writeMinecraft(mcToken);
                }

                this.Close(); // success
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
            // 6. get minecraft profile (username, uuid)

            if (mcToken == null)
                throw new ArgumentNullException("mcToken was null");

            if (!MojangAPI.CheckGameOwnership(mcToken.AccessToken))
            {
                MessageBox.Show("You don't have minecraft JE");
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

        private void signout()
        {
            writeMicrosoft(null);
            writeMinecraft(null);

            CreateWV(); // show webview control
            wv.Source = new Uri(MicrosoftOAuth.GetSignOutUrl());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RemoveWV(); // remove webview control
        }
    }
}
